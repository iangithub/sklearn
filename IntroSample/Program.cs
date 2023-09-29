using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace IntroSample
{
    internal class Program
    {
        private const string deploy_model = "xxxx";
        private const string aoai_Endpoint = "https://xxxx.openai.azure.com";
        private const string api_Key = "xxxxx";

        static async Task Main(string[] args)
        {
            Console.WriteLine("bot: 你想聽什麼主題的故事呢? \n");
            Console.Write("you: ");
            string storySubject = Console.ReadLine();
            Console.Write("\n");
            Console.WriteLine("bot: 故事的角色是什麼呢? \n");
            Console.Write("you: ");
            string storyRole = Console.ReadLine();
            Console.Write("\n");

            var kernel = new KernelBuilder()
                .WithAzureChatCompletionService(
                 deploy_model,   // Azure OpenAI Deployment Name
                 aoai_Endpoint, // Azure OpenAI Endpoint
                 api_Key  // Azure OpenAI Key
                ).Build();

           
            //Use OpenAI Service
            //var kernel = new KernelBuilder()
            //.WithOpenAIChatCompletionService(
            //    OpenAIModelId,              // The name of your deployment (e.g., "gpt-3.5-turbo")
            //    OpenAIApiKey,               // The API key of your Azure OpenAI service
            //    OpenAIOrgId                 // The endpoint of your Azure OpenAI service
            //)
            //.Build();

            //接上Plugins
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var writerPlugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

            //Prompt Template 參數
            var variables = new ContextVariables
            {
                ["story_subject"] = storySubject,
                ["story_role"] = storySubject
            };

            //叫用GPT模型等得生成結果
            var result = (await kernel.RunAsync(variables, writerPlugin["FairyTales"])).Result;

            Console.WriteLine(result);

            Console.ReadLine();
        }
    }
}