using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyFanc.Contracts.Services;

namespace MyFanc.Services
{
	public class BlobFileStorage : IFileStorage
	{
		private readonly StorageAccount _configuration;
		private readonly BlobContainerClient _blobClient;
		private readonly ILogger<BlobFileStorage> _logger;

		public BlobFileStorage(ILogger<BlobFileStorage> logger, IOptions<StorageAccount> configuration)
        {
			this._configuration = configuration.Value;

			this._blobClient = this.InitBlobClient();

			this._logger = logger;
        }

        public async Task<bool> DeleteAsync(string filePath)
		{
			var success = false;

			if (string.IsNullOrEmpty(filePath))
			{
				this._logger.LogError($"Parameter `{nameof(filePath)}` cannot be empty");
				
				return success;
			}

			try
			{
				var blobClient = this._blobClient.GetBlobClient(this.ExtractFilenameFromPath(filePath));

				var response = await blobClient.DeleteIfExistsAsync();

				success = response.Value;
			}
			catch (Exception ex)
			{
				this._logger.LogError($"Error on deleting file: `{nameof(filePath)}`, message: {ex}");
			}

			return success;
		}

		public async Task<bool> ExistsAsync(string filePath)
		{
			var success = false;

			if (string.IsNullOrEmpty(filePath))
			{
				this._logger.LogError($"Parameter `{nameof(filePath)}` cannot be empty");
				
				return false;
			}

			try
			{
				var blobClient = this._blobClient.GetBlobClient(this.ExtractFilenameFromPath(filePath));

				var response = await blobClient.ExistsAsync();

				success = response.Value;
			}
			catch (Exception ex)
			{
				this._logger.LogError($"Error on checking file: `{nameof(filePath)}`, message: {ex}");
			}

			return success;
		}

		public async Task<byte[]> GetAsync(string filePath)
		{
			byte[] data = null;

			if (string.IsNullOrEmpty(filePath))
			{
				this._logger.LogError($"Parameter `{nameof(filePath)}` cannot be empty");

				return data;
			}

			try
			{
				var blobClient = this._blobClient.GetBlobClient(this.ExtractFilenameFromPath(filePath));

				if (await blobClient.ExistsAsync())
				{
					using (var stream = new MemoryStream())
					{
						await blobClient.DownloadToAsync(stream);

						stream.Seek(0, SeekOrigin.Begin);
						
						data = stream.ToArray();
					}
				}
			}
			catch (Exception ex)
			{
				this._logger.LogError($"Error on getting file: `{nameof(filePath)}`, message: {ex}");
			}

			return data;
		}

		public async Task<string> SaveAsync(byte[] fileContent, string folder, string filename)
		{
			var filePath = string.Empty;

			if (!fileContent.Any())
			{
				this._logger.LogError($"Parameter `{nameof(fileContent)}` cannot be null");

				return filePath;
			}

			if (this._blobClient == null)
			{
				this._logger.LogError($"Blob client is not initialized.");

				return filePath;
			}

			try
			{
				var blobClient = this._blobClient.GetBlobClient($"{folder}/{filename}");

				var stream = new MemoryStream(fileContent);

				await blobClient.UploadAsync(stream, true);

				filePath = blobClient.Uri.ToString();
			}
			catch (Exception ex)
			{
				this._logger.LogError($"Error on saving file: `{nameof(filename)}`, message: {ex}");
			}

			return filePath;
		}

		private string ExtractFilenameFromPath(string filePath)
		{
			return filePath.Replace(this._blobClient.Uri.ToString(), string.Empty);
		}

		private BlobContainerClient? InitBlobClient()
		{
			if (string.IsNullOrEmpty(_configuration.ConnectionString))
			{
				return null;
			}

			var client = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);
			client.CreateIfNotExists(PublicAccessType.Blob);

			return client;
		}
	}
}
