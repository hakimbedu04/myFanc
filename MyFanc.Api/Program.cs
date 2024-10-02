using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyFanc.BLL;
using MyFanc.BLL.Utility;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Core.Utility;
using MyFanc.DAL;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using Serilog;
using System.Net;
using System.Net.Mime;
using static MyFanc.Core.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.WithCorrelationId()
    .CreateLogger();

FancRADApiConfiguration radApiConfiguration = new ();
configuration.GetSection(nameof(FancRADApiConfiguration))
                     .Bind(radApiConfiguration);

builder.Logging.AddSerilog();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authConfig = configuration.GetSection("Authentication");

        options.Authority = authConfig["Authority"];
        options.RequireHttpsMetadata = bool.Parse(authConfig["RequireHttpsMetadata"] ?? "false");
        options.Audience = authConfig["Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            //ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authConfig["SigningKey"] ?? string.Empty)),

            ValidateIssuer = true,
            ValidIssuer = authConfig["Authority"],

            ValidateAudience = true,
            ValidAudience = authConfig["Audience"],

            ValidTypes = new[] { "at+jwt" },
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };

        // Optional: Configure backchannel validation for access tokens
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });


// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MyFancDbContext>();

// Add services to the container.
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Use the custom enum converter
                    options.JsonSerializerOptions.Converters.Add(new EnumNameConverter<AuthRedirection>());
                    options.JsonSerializerOptions.Converters.Add(new EnumNameConverter<ValidationStatus>());
                });


builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyFanc API", Version = "v1" });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement() 
    {
	    {
		    new OpenApiSecurityScheme {
			    Reference = new OpenApiReference {
				    Type = ReferenceType.SecurityScheme,
				    Id = "Bearer"
		        }
	        },
		    new List<string> ()
	    }
    });
});

// Add Entity Framework Core
builder.Services.AddDbContext<MyFancDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyFancDbConnection")));

//DAL injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IFancRADApiConfiguration>(radApiConfiguration);
builder.Services.Configure<TokenConfiguration>(configuration.GetSection(nameof(TokenConfiguration)));
builder.Services.Configure<StorageAccount>(configuration.GetSection(nameof(StorageAccount)));
builder.Services.AddSingleton<ITokenConfiguration>(sp => sp.GetRequiredService<IOptions<TokenConfiguration>>().Value);
builder.Services.Configure<IdentityProviderConfiguration>(configuration.GetSection(nameof(IdentityProviderConfiguration)));
builder.Services.AddSingleton<IIdentityProviderConfiguration>(sp => sp.GetRequiredService<IOptions<IdentityProviderConfiguration>>().Value);
builder.Services.Configure<CacheStorageDurations>(configuration.GetSection(nameof(CacheStorageDurations)));
builder.Services.AddSingleton<ICacheStorageDurations>(sp => sp.GetRequiredService<IOptions<CacheStorageDurations>>().Value);
builder.Services.AddSingleton<ISharedDataCache, SharedDataCache>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuditingService, AuditingService>();
builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHepler>();
builder.Services.AddScoped<IBreadCrumbService, BreadCrumbService>();
builder.Services.AddScoped<IAESEncryptService, AESEncryptService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient(typeof(IDataProcessingService), typeof(DataProcessingService));
builder.Services.AddScoped<IAuthTokenStore, FancAuthTokenStore>();
builder.Services.AddScoped(typeof(AuthHeaderHandler));
builder.Services.AddScoped<INacabelHelper, NacabelHelper>();
builder.Services.AddScoped<IFileStorage, BlobFileStorage>();

//BLL injection
builder.Services.AddScoped<IBll, Bll>();

// Add other services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddRefitClient<IFancRADApi>(new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer(
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Converters = { new StringEnumConverter(), },
            }
        )
}).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(radApiConfiguration.BasePath);
}).AddHttpMessageHandler<AuthHeaderHandler>();


builder.Services.AddRefitClient<IFancAuthentication>(new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer(
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters = { new StringEnumConverter(), },
            }
        )    
}).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(radApiConfiguration.AuthenticationPath);    
});


var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyFanc API v1");
    });
//}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = JsonConvert.SerializeObject(
            new { status = report.Status.ToString() }
        );
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    }
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        // Retrieve the error details
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Handle the exception and return an error response
        var errorResponse = new
        {
            error = "An unexpected error occurred.",
            details = exception?.Message
        };

        if (exception is KeyNotFoundException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        // Log Exception
        app.Logger.LogError(exception, "An unexpected error occurred.");

        // Serialize the error response to JSON
        var jsonErrorResponse = JsonConvert.SerializeObject(errorResponse);

        // Write the JSON response to the HTTP response
        await context.Response.WriteAsync(jsonErrorResponse);
    });
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
