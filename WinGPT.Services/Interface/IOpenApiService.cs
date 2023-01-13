namespace WinGPT.Services.Interface
{
    public interface IOpenApiService
    {
        Dictionary<string, string> GetConfigurations();
        Task<List<string>> GetModel(string model);
        Task<string> GetTextCompletion(string message);
    }
}