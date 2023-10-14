using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

namespace KernelChatCompletion
{
    internal class Program
    {
        private const string deploy_Model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxxx";

        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                .WithAzureChatCompletionService(
                 deploy_Model,   // Azure OpenAI Deployment Name
                 aoai_Endpoint, // Azure OpenAI Endpoint
                 api_Key  // Azure OpenAI Key
                ).Build();

            var requestSettings = new OpenAIRequestSettings()
            {
                MaxTokens = 1024,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                Temperature = 0.2,
                TopP = 0.5
            };

            var chatCompletion = kernel.GetService<IChatCompletion>();
            Console.WriteLine($"chatCompletion Object:{chatCompletion.GetType()}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("---------------------------");
            var chatHistory = chatCompletion.CreateNewChat("你是一位自然語言人工智慧研究專家");
            chatHistory.AddUserMessage("ChatGPT與過去的自然語言模型有什麼不同，舉個簡單的例子說明");


            foreach (IChatResult chatCompletionResult in await chatCompletion.GetChatCompletionsAsync(chatHistory, requestSettings))
            {
                ChatMessageBase chatMessage = await chatCompletionResult.GetChatMessageAsync();
                chatHistory.Add(chatMessage); //加入對話歷程
                Console.WriteLine($"Completions : {chatMessage.Content.ToString()}"); //生成結果
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("---- chatHistory ------");
            foreach (var message in chatHistory.Messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }
            Console.WriteLine();

            Console.WriteLine("Hello, World!");
        }
    }
}