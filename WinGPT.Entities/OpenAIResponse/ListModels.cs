using System.Collections.Generic;

namespace WinGPT.Entities.OpenAIResponse;

public class ListModels
{
    public string id { get; set; }
    public string @object { get; set; }
    public string owned_by { get; set; }
    public List<object> permission { get; set; }
}

public class ListModelsRoot
{
    public List<ListModels> data { get; set; }
    public string @object { get; set; }
}