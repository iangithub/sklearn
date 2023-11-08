using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Plugins.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAndRagUseSemanticFun.Plugins.QAPlugin
{
    public class KmSearch
    {
        private const string deploy_Model = "gpt4demo";
        private const string embedding_Model = "textembedding";
        private const string aoai_Endpoint = "https://demo0222.openai.azure.com";
        private const string api_Key = "ece107c84324497e9987d15f916845e4";
        private const string embedding_CollectionName = "Law";

        [SKFunction, Description("從知識庫中取得參考資料")]
        public async Task<string> GetKm(string input, SKContext skContext)
        {

            var memoryWithCustomDb = new MemoryBuilder()
           .WithAzureTextEmbeddingGenerationService(embedding_Model, aoai_Endpoint, api_Key)
           .WithMemoryStore(new VolatileMemoryStore())
           .Build();

            //RAG Search
            var searchResult = memoryWithCustomDb
                .SearchAsync(embedding_CollectionName, input, minRelevanceScore: 0.8);

            var ans = string.Empty;

            await foreach (var kmResult in searchResult)
            {
                ans += $"{kmResult.Metadata.Text}\n\n";
            }

            if (string.IsNullOrEmpty(ans))
            {
                ans = "很抱歉，知識庫沒有相關資料可以提供";
            }

            return ans;
        }
    }
}
