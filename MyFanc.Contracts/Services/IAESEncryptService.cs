namespace MyFanc.Contracts.Services
{
    public interface IAESEncryptService
    {
        string EncryptString(string text);
        string DecryptString(string cipherTextCombinedString);
    }
}
