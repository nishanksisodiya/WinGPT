namespace WinGPT.Entities.OpenAIResponse;

public class CreateCompletionRoot
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public List<CreateCompletion> choices { get; set; }
    public CreateCompletionUsage usage { get; set; }
}

public class CreateCompletion
{
    public string text { get; set; }
    public int index { get; set; }
    public object logprobs { get; set; }
    public string finish_reason { get; set; }
}

public class CreateCompletionUsage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}
