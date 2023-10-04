using Microsoft.SemanticKernel.SkillDefinition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeFunctionSample.Plugins.CrmPlugin
{
    public class Customer
    {
        [SKFunction, Description("取得客戶連絡人")]
        public string GetCustomerContact(string cusCode)
        {
            //撰寫從CRM DataBase 取得客戶連絡人資料邏輯
            //do something
            return $"居匹踢";
        }
    }
}
