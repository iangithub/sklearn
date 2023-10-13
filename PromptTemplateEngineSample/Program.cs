using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.TemplateEngine.Basic;

namespace PromptTemplateEngineSample
{
    public class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxxx";

        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                 .WithAzureChatCompletionService(
                  deploy_model,   // Azure OpenAI Deployment Name
                  aoai_Endpoint, // Azure OpenAI Endpoint
                  api_Key  // Azure OpenAI Key
                 ).Build();

            //掛載TextPlugin
            kernel.ImportFunctions(new TextPlugin(), "textplugin");

            var context = kernel.CreateNewContext();
            context.Variables["subject"] = "愛惡作劇";
            context.Variables["major_character"] = "耿鬼";

            var prompt = @"寫一個則童話故事，有關於: {{textplugin.Concat input=$subject input2=$major_character}}";

            //建立BasicPromptTemplateEngine物件並調用RenderAsync方法
            var promptRenderer = new BasicPromptTemplateEngine();
            var renderedPrompt = await promptRenderer.RenderAsync(prompt, context);
            Console.WriteLine("--- Rendered Prompt ---");
            Console.WriteLine(renderedPrompt);

            //建立SemanticFunction
            var semanticFunc = kernel.CreateSemanticFunction(prompt, requestSettings: new OpenAIRequestSettings() { MaxTokens = 1000 });

            Console.WriteLine("--- Semantic Function result ---");
            //取得生成結果
            var result = await kernel.RunAsync(context.Variables, semanticFunc);
            Console.WriteLine(result.GetValue<string>());

        }
    }
}