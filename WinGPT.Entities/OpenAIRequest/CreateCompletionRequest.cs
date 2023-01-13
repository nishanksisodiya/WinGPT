using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinGPT.Entities.OpenAIRequest;

public class CreateCompletionRequest
{
    public string model { get; set; }
    public string prompt { get; set; }
    public int max_tokens { get; set; }
    public double temperature { get; set; }
    public double top_p { get; set; }
    public int n { get; set; }
    public bool stream { get; set; }
    public object logprobs { get; set; }
    public string stop { get; set; }
}
