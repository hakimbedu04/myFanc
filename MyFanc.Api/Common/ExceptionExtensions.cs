using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;
using System.Net;

namespace MyFanc.Api.Common
{
	public static class ExceptionExtensions
	{
		public static async Task<IActionResult> ToActionResultAsync(this Exception exception)
		{
			if (exception == null)
			{
				return new OkResult();
			}

			if (exception is ApiException apiException) 
			{
				var content = await apiException.GetContentAsAsync<Dictionary<string, string>>();
				var message = content?.FirstOrDefault(pair => pair.Key == "detail").Value;

				return new ObjectResult(message) { StatusCode = (int)apiException.StatusCode };
			}

			if (exception is DbUpdateException)
			{
				var errorMessage = $"Unable to save the data. Error on the database process and no change has been commited";
				
				return new ObjectResult(errorMessage) { StatusCode = (int)HttpStatusCode.BadRequest };
			}

			return new ObjectResult(exception.Message) { StatusCode = (int)HttpStatusCode.BadRequest };
		}
	}
}
