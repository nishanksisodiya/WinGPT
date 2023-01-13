namespace WinGPT.Services.Interface
{
    public interface IOpenApiService
    {
        Task<List<string>> GetModel(string model);
        Task<string> GetTextCompletion(string message);
    }
}