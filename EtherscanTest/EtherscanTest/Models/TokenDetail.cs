using System;
using System.Collections.Generic;
using System.Net;

namespace EtherscanTest.Models
{
    public class TokenDetail
    {
        public string contract_address { get; set; }
        public string price { get; set; }
        public string total_supply { get; set; }
        public string total_holders { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string symbol { get; set; }
    }
    public class TokenSymbols
    {
        public string symbols { get; set; }
    }
}