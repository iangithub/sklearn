using Azure.AI.OpenAI;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;

namespace MultiChatCompletionSample
{
    internal class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxxx";


        static async Task Main(string[] args)
        {
            var chatCompletion = new AzureChatCompletion(
            deploy_model, // Azure OpenAI Deployment Name
            aoai_Endpoint, // Azure OpenAI Endpoint
            api_Key // Azure OpenAI Key
            ); 

            var requestSettings = new OpenAIRequestSettings()
            {
                MaxTokens = 1024,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                Temperature = 0.2,
                TopP = 0.5
            };

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
        }
    }
}