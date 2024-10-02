using Microsoft.EntityFrameworkCore;
using MyFanc.BusinessObjects;

namespace MyFanc.DAL
{
    public class MyFancDbContext : DbContext
    {
        public MyFancDbContext(DbContextOptions<MyFancDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wizard> Wizards { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<HistoryMail> HistoryMails { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Nacabel> Nacabels { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<NacabelsEntityMap> NacabelsEntityMap { get; set; }
        public DbSet<FormNodes> FormNodes { get; set; }
        public DbSet<FormNodeFields> FormNodeFields { get; set; }
        public DbSet<FormValueFields> FormValueFields { get; set; }
        public DbSet<FormConditionals> FormConditionals { get; set; }
        public DbSet<FormDataSource> FormDataSources { get; set; }
        public DbSet<FormDocument> FormDocuments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<FormCategory> FormCategory { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<PersonaCategories> PersonaCategories { get; set; }
        public DbSet<UserPersonas> UserPersonas { get; set; }
        public DbSet<CompanyPersonas> CompanyPersonas { get; set; }
        public DbSet<UserPersonaCategories> UserPersonaCategories { get; set; }
        public DbSet<CompanyPersonaCategories> CompanyPersonaCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<User>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Wizard>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Wizard>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Wizard>()
                .HasMany(q => q.IntroductionTexts)
                .WithMany(t => t.WizardsTexts);

            modelBuilder.Entity<Wizard>()
                .HasMany(q => q.Titles)
                .WithMany(t => t.WizardsTitles)
                .UsingEntity("WizardTitlesTranslations");

            modelBuilder.Entity<Wizard>()
                .HasMany(q => q.Questions)
                .WithOne(a => a.Wizard)
                .HasForeignKey(a => a.WizardId);



            modelBuilder.Entity<Question>()
                .HasKey(q=>q.Id);

            modelBuilder.Entity<Question>()
                .Property(q=>q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Texts)
                .WithMany(t => t.QuestionsTexts)
                .UsingEntity("QuestionsTextsTranslations");

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Titles)
                .WithMany(t => t.QuestionsTitles)
                .UsingEntity("QuestionsTitlesTranslations");

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Breadcrumbs)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.BreadcrumbsItems)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.ClientCascade);



            modelBuilder.Entity<Answer>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Answer>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Answer>()
                .HasMany(q => q.Labels)
                .WithMany(t => t.AnswersLabels)
                .UsingEntity("AnswersLabelsTranslations");

            modelBuilder.Entity<Answer>()
                .HasMany(q => q.Details)
                .WithMany(t => t.AnswersDetails)
                .UsingEntity("AnswersDetailsTranslations");

            modelBuilder.Entity<Answer>()
                .HasMany(q => q.Links)
                .WithMany(t => t.AnswersLinks)
                .UsingEntity("AnswersLinksTranslations");

            modelBuilder.Entity<Answer>()
                .HasMany(q => q.FinalAnswerTexts)
                .WithMany(t => t.AnswersFinalAnswerTexts)
                .UsingEntity("AnswersFinalAnswerTextsTranslations");

            modelBuilder.Entity<Answer>()
                .HasOne(q => q.LinkedQuestion)
                .WithMany(a => a.LinkedAnswer)
                .HasForeignKey(a => a.LinkedQuestionId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<QuestionBreadcrumb>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<QuestionBreadcrumb>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<QuestionBreadcrumb>()
                .HasMany(q => q.Items)
                .WithOne(a => a.Breadcrumb)
                .HasForeignKey(a => a.BreadcrumbId);



            modelBuilder.Entity<QuestionBreadcrumbItem>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<QuestionBreadcrumbItem>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRoles>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<UserRoles>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Roles>()
                .HasKey(q => q.Id);
            modelBuilder.Entity<Roles>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();
                
            modelBuilder.Entity<HistoryMail>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<Nacabel>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<Nacabel>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Nacabel>()
                .HasMany(n => n.NacabelTranslation)
                .WithOne(t => t.Nacabel)
                .HasForeignKey(t => t.NacabelId);

            modelBuilder.Entity<Nacabel>()
              .HasMany(q => q.NacabelsEntityMap)
              .WithOne(a => a.Nacabel)
              .HasForeignKey(a => a.NacabelId);


            modelBuilder.Entity<NacabelTranslation>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<NacabelTranslation>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<NacabelsEntityMap>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<NacabelsEntityMap>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            
            modelBuilder.Entity<Form>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Form>()
                .HasAlternateKey(q => new { q.ExternalId, q.Version, q.Type });

            modelBuilder.Entity<Form>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Form>()
                .HasMany(q => q.Labels)
                .WithMany(t => t.FormsLabels)
                .UsingEntity("FormsLabelsTranslations");

            modelBuilder.Entity<Form>()
                .HasMany(q => q.Descriptions)
                .WithMany(t => t.FormsDescriptions)
                .UsingEntity("FormsDescriptionsTranslations");

            modelBuilder.Entity<Form>()
                .HasMany(q => q.Urls)
                .WithMany(t => t.FormsUrls)
                .UsingEntity("FormsUrlsTranslations");
            
            modelBuilder.Entity<Form>()
               .HasMany(q => q.FormNodes)
               .WithOne(a => a.Form)
               .HasForeignKey(a => a.FormId);

            modelBuilder.Entity<Form>()
               .HasMany(q => q.FormDocuments)
               .WithOne(a => a.Form)
               .HasForeignKey(a => a.FormId);

            modelBuilder.Entity<Form>()
                .HasMany(q => q.Nacabels)
                .WithMany(t => t.Forms)
                .UsingEntity("FormsNacabels");

            modelBuilder.Entity<Form>()
             .HasMany(q => q.FormSubmissions)
             .WithOne(a => a.Form)
             .HasForeignKey(a => a.FormId);


            modelBuilder.Entity<FormNodes>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormNodes>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<FormNodes>()
                .HasMany(q => q.Labels)
                .WithMany(t => t.FormNodesLabels)
                .UsingEntity("FormNodesLabelsTranslations");

            modelBuilder.Entity<FormNodes>()
            .HasOne(p => p.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(p => p.ParentId)
            .HasConstraintName("FK_FormNodes_FormNodes_ParentId");

            modelBuilder.Entity<FormNodes>()
               .HasMany(q => q.FormNodeFields)
               .WithOne(a => a.FormNodes)
               .HasForeignKey(a => a.FormNodeId);

            modelBuilder.Entity<FormNodes>()
               .HasMany(q => q.FormConditionals)
               .WithOne(a => a.FormNode)
               .HasForeignKey(a => a.FormNodeFieldId);



            modelBuilder.Entity<FormNodeFields>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormNodeFields>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<FormNodeFields>()
                .HasMany(q => q.Labels)
                .WithMany(t => t.FormNodeFieldsLabels)
                .UsingEntity("FormNodeFieldsLabelsTranslations");

            modelBuilder.Entity<FormNodeFields>()
               .HasMany(q => q.FormValueFields)
               .WithOne(a => a.FormNodeFields)
               .HasForeignKey(a => a.FormNodeFieldId);

            modelBuilder.Entity<FormNodeFields>()
               .HasMany(q => q.FormConditionals)
               .WithOne(a => a.FormNodeField)
               .HasForeignKey(a => a.FormNodeFieldId);



            modelBuilder.Entity<FormValueFields>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormValueFields>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<FormValueFields>()
                .HasMany(q => q.Labels)
                .WithMany(t => t.FormValueFieldsLabels)
                .UsingEntity("FormValueFieldsLabelsTranslations");



            modelBuilder.Entity<FormConditionals>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormConditionals>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");


            modelBuilder.Entity<FormDataSource>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormDataSource>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");


            modelBuilder.Entity<FormDocument>()
              .HasKey(q => q.Id);

            modelBuilder.Entity<FormDocument>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<FormDocument>()
              .HasMany(q => q.Documents)
              .WithOne(a => a.FormDocument)
              .HasForeignKey(a => a.FormDocumentId);


            modelBuilder.Entity<Document>()
             .HasKey(q => q.Id);

            modelBuilder.Entity<Document>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");


            modelBuilder.Entity<FormCategory>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormCategory>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FormCategory>()
              .HasMany(q => q.Forms)
              .WithOne(a => a.FormCategory)
              .HasForeignKey(a => a.FormCategoryId);


            modelBuilder.Entity<FormSubmission>()
               .HasKey(q => q.Id);

            modelBuilder.Entity<FormSubmission>()
                .Property(q => q.Id)
                .HasDefaultValueSql("NEWID()");


            modelBuilder.Entity<PersonaCategories>()
                .HasOne(d => d.Nacebel)
                .WithMany(p => p.PersonaCategories)
                .HasForeignKey(d => d.NacabelId);
            
            modelBuilder.Entity<PersonaCategories>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<PersonaCategories>()
                .HasOne(d => d.Nacebel)
                .WithMany(p => p.PersonaCategories)
                .HasForeignKey(d => d.NacabelId)
                .HasConstraintName("FK_PersonaCategories_Nacebels_NacebelId");

            modelBuilder.Entity<PersonaCategories>()
                .HasOne(d => d.Parent)
                    .WithMany(p => p.Children)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_PersonaCategories_PersonaCategories_ParentId");

            modelBuilder.Entity<PersonaCategories>()
                .HasIndex(e => e.NacabelId, "IX_PersonaCategories_NacebelId");

            modelBuilder.Entity<PersonaCategories>()
                .HasIndex(e => e.ParentId, "IX_PersonaCategories_ParentId");

            modelBuilder.Entity<PersonaCategories>(entity =>
            {
                entity.HasIndex(e => e.NacabelId, "IX_PersonaCategories_NacebelId");

                entity.HasIndex(e => e.ParentId, "IX_PersonaCategories_ParentId");

                entity.HasOne(d => d.Nacebel)
                    .WithMany(p => p.PersonaCategories)
                    .HasForeignKey(d => d.NacabelId)
                    .HasConstraintName("FK_PersonaCategories_Nacebels_NacebelId");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Children)
                    .HasForeignKey(d => d.ParentId);

                entity.HasMany(d => d.Labels)
                    .WithMany(p => p.PersonaCategoriesLabel)
                    .UsingEntity<Dictionary<string, object>>(
                        "PersonaCategoryLabelsTranslation",
                        l => l.HasOne<Translation>().WithMany().HasForeignKey("LabelsId"),
                        r => r.HasOne<PersonaCategories>().WithMany().HasForeignKey("PersonaCategoryId"),
                        j =>
                        {
                            j.HasKey("PersonaCategoryId", "LabelsId");

                            j.ToTable("PersonaCategoryLabelsTranslations");

                            j.HasIndex(new[] { "LabelsId" }, "IX_PersonaCategoryLabelsTranslations_LabelId");

                            j.HasIndex(new[] { "PersonaCategoryId" }, "IX_PersonaCategoryLabelsTranslations_PersonaCategoryId");
                        });
            });

            modelBuilder.Entity<UserPersonas>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<UserPersonas>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserPersonas>()
                .HasOne(d => d.User)
                .WithMany(p => p.UserPersonas).HasForeignKey(d => d.UserId);

            modelBuilder.Entity<UserPersonas>()
                .Property(e => e.InamiNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("INAMINumber");

            modelBuilder.Entity<CompanyPersonas>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<CompanyPersonas>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserPersonaCategories>()
                .HasKey(q => q.UserPersonaId);

            modelBuilder.Entity<UserPersonaCategories>()
                .HasOne(d => d.UserPersonas).WithMany().HasForeignKey("UserPersonaId");

            modelBuilder.Entity<UserPersonaCategories>()
                .HasKey(q => q.PersonaCategoryId);

            modelBuilder.Entity<UserPersonaCategories>()
                .HasOne(d => d.PersonaCategory).WithMany().HasForeignKey("PersonaCategoryId");

            modelBuilder.Entity<CompanyPersonaCategories>()
                .HasKey(q => q.CompanyPersonaId);

            modelBuilder.Entity<CompanyPersonaCategories>()
                .HasKey(q => q.PersonaCategoryId);

            modelBuilder.Entity<CompanyPersonaCategories>()
                .HasOne(d => d.CompanyPersonas).WithMany().HasForeignKey("CompanyPersonaId");

            modelBuilder.Entity<CompanyPersonaCategories>()
                .HasOne(d => d.PersonaCategory).WithMany().HasForeignKey("PersonaCategoryId");
        }
    }
}
