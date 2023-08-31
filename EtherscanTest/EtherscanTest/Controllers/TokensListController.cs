using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using EtherscanTest.Models;
using Newtonsoft.Json;
using System.Net.Http;
using EtherscanTest.Helpers;
using EtherscanTest.Helpers.Interfaces;
using EtherscanTest.DTO;
using System.Net;
using System.Threading.Tasks;
using System.Security.Policy;
using MySqlX.XDevAPI.Common;
using System.Web.Http.Results;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto.Tls;

namespace EtherscanTest.Controllers
{
    public class TokensListController : Controller
    {
        private string dbtype = ConfigurationManager.AppSettings["sqltype"].ToString();
        private ITokenService itokenService;
        private TokenService tokenService;
        private static string API_KEY = "ddcd29a9-e00d-4eb0-8625-3cd8f257c7d5";

        public TokensListController() : this(new TokenService())
        {
        }
        public TokensListController(ITokenService tokenService)
        {
            this.itokenService = tokenService;
        }

        public TokensListController(TokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpGet]
        public async Task<JsonResult> GetTokens()
        {
            string URL = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";
            string res = string.Empty;


            using (HttpClient client = new HttpClient())
            {
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", API_KEY);
                HttpResponseMessage response = await client.GetAsync(URL);
                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
            }

            return Json(Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(res));

        }

        [HttpPost]
        public ActionResult SaveToken(TokenDTO NewToken)
        {
            var user = new TokenDTO()
            {
                symbol = NewToken.symbol,
                price = NewToken.price,
                ContractAddress = NewToken.ContractAddress,
                TotalSupply = NewToken.TotalSupply,
                TotalHolders = NewToken.TotalHolders,
                name = NewToken.name,
            };



            return RedirectToAction("Index");

        }
    }
}
