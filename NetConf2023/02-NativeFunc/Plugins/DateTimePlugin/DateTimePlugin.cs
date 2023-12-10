using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace MyPlugins;

internal class DataTimePlugin
{
    [KernelFunction("GetCurrentDateTime")]
    [Description("Get the current date and time.'")]
    public string GetCurrentDateTime()
    {
        return DateTime.Now.ToString();
    }
}