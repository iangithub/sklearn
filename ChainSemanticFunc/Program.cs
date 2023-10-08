using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel;

namespace ChainSemanticFunc
{
    internal class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxx";

        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                 .WithAzureChatCompletionService(
                  deploy_model,   // Azure OpenAI Deployment Name
                  aoai_Endpoint, // Azure OpenAI Endpoint
                  api_Key  // Azure OpenAI Key
                 ).Build();

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var plugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");
            
            //叫用GPT模型等得生成結果
            var result = await kernel.RunAsync(new ContextVariables("ChatGPT對校園教育的衝擊")
                , plugin["FacebookPoster"]
                , plugin["Translate"]);

            Console.WriteLine(result);

        }
    }
}