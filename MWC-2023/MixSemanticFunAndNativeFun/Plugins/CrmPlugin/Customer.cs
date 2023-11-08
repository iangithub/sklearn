using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixSemanticFunAndNativeFun.Plugins.CrmPlugin
{
    public class Customer
    {
        [SKFunction, Description("取得客戶連絡人")]
        public string GetCustomerContact(string cusCode)
        {
            //撰寫從CRM DataBase 取得客戶連絡人資料邏輯
            //do something
            return $"陳伊恩({cusCode})";
        }

        [SKFunction, Description("根據客戶代碼及區域，取得客戶連絡人")]
        public string GetCustomerContactV2(SKContext context)
        {
            //撰寫從CRM DataBase 取得客戶連絡人資料邏輯
            //do something
            return $"{context.Variables["CusCode"]}-{context.Variables["Area"]} 陳伊恩";
        }
    }
}
