namespace WinGPT.Services.Interface
{
    public interface IOpenApiService
    {
        Dictionary<string, string> GetConfigurations();
        List<string> GetModel();
        Task<string> GetTextCompletion(string message);
    }
}