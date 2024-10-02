namespace MyFanc.Contracts.Services
{
	public interface IFileStorage
	{
		Task<bool> ExistsAsync(string filePath);

		Task<byte[]> GetAsync(string filePath);

		Task<string> SaveAsync(byte[] fileData, string folder, string filename);
		
		Task<bool> DeleteAsync(string filePath);
	}
}
