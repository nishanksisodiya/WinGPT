using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinGPT.Entities.Configurations;

public class AppConfigurations
{
    public string Model { get; set; }
    public string MaxToken { get; set; }
    public string OpenAIAPIKey { get; set; }
    public string OpenAIOrgId { get; set; }
}

