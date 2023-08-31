using EtherscanTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace EtherscanTest.DTO
{
    public class TokenDTO
    { 
        public List<TokenDetail> tokenDetail { get; set; }

        public int ID { get; set; }
        public string price { get; set; }
        public string ContractAddress { get; set; }
        public decimal TotalSupply { get; set; }
        public decimal TotalHolders { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public decimal allsupply { get; set; }
    }
}