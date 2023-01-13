using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WinGPT.Entities.OpenAIRequest;
using WinGPT.Entities.OpenAIResponse;
using WinGPT.Services.Interface;

namespace WinGPT.Services.Services;

public class OpenApiService : IOpenApiService
{
    private readonly IConfiguration configuration;

    public OpenApiService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<List<string>> GetModel(string model)
    {
        var modelsList = new List<string>();

        using (var httpClient = new HttpClient())
        {
            string openAIAPIKey = configuration.GetSection("Values").GetSection("OpenAIAPIKey").Value;
            string openAIOrgId = configuration.GetSection("Values").GetSection("OpenAIOrgId").Value;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.openai.com/v1/models"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openAIAPIKey);
                requestMessage.Headers.Add("OpenAI-Organization", openAIOrgId);

                var res = await httpClient.SendAsync(requestMessage);
                var result = await res.Content.ReadAsStringAsync();

                var models = JsonConvert.DeserializeObject<ListModelsRoot>(result);
                modelsList = models!.data.Select(m => m.id).ToList();
            }
        }

        return modelsList;
    }

    public async Task<string> GetTextCompletion(string message)
    {
        string responseMessage = string.Empty;
        try
        {
            using (var httpClient = new HttpClient())
            {
                string openAIAPIKey = configuration.GetSection("Values").GetSection("OpenAIAPIKey").Value;
                string openAIOrgId = configuration.GetSection("Values").GetSection("OpenAIOrgId").Value;
                string model = configuration.GetSection("Values").GetSection("Model").Value;
                int maxTokens = Convert.ToInt32(configuration.GetSection("Values").GetSection("MaxToken").Value);

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions"))
                {
                    var request = new CreateCompletionRequest()
                    {
                        model = model,
                        max_tokens = maxTokens,
                        n = 1,
                        top_p = 1,
                        temperature = 0.1,
                        stream = false,
                        logprobs = null!,
                        prompt = message
                    };

                    requestMessage.Content = JsonContent.Create(request);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openAIAPIKey);
                    requestMessage.Headers.Add("OpenAI-Organization", openAIOrgId);

                    var res = await httpClient.SendAsync(requestMessage);
                    var result = await res.Content.ReadAsStringAsync();

                    var completionResult = JsonConvert.DeserializeObject<CreateCompletionRoot>(result);

                    responseMessage = completionResult!.choices[0].text.Trim();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }


        return responseMessage;
    }

    public Dictionary<string, string> GetConfigurations()
    {
        Dictionary<string, string> configurations = new Dictionary<string, string>();

        if (File.Exists("Configurations.json"))
        {
            using (StreamReader file = File.OpenText("Configurations.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                configurations = (Dictionary<string, string>)serializer.Deserialize(file, typeof(Dictionary<string, string>))!;
            }
        }

        return configurations!;
    }
}
