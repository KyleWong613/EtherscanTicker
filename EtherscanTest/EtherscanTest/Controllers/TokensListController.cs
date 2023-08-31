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
        public ActionResult SaveToken()
        {

            string id = "";
            var connection = "";

            if (Request.QueryString["id"] != null)
            {
                id = Request.QueryString["id"];
                ViewBag.IsDetail = true;
            }
            else
            {
                ViewBag.IsDetail = false;
            }

            if (dbtype == "mssql")
            {
                connection = ConfigurationManager.AppSettings["mssqlConnection"].ToString();
                string sql = "select * from token WHERE symbol=@symbol";

            }
            else
            {
                connection = ConfigurationManager.AppSettings["mysqlConnection"].ToString();
                MySqlConnection conn = new MySqlConnection(connection);
                try
                {
                    conn.Open();

                    //Retrieve all token details from db based on symbol value
                    string sql = "select * from token WHERE symbol=@symbol";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@symbol", id);

                    MySqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        var tokendetail = new Models.TokenDetail();
                        foreach (DataRow record in dt.Rows)
                        {
                            tokendetail.contract_address = record["contract_address"].ToString();
                            tokendetail.symbol = record["symbol"].ToString();
                            tokendetail.price = record["Price"].ToString();
                            tokendetail.total_supply = record["Total_supply"].ToString();
                            tokendetail.total_holders = record["Total_holders"].ToString();
                            tokendetail.name = record["name"].ToString();
                        }
                        ViewBag.ContractAddress = tokendetail.contract_address;
                        ViewBag.Symbol = tokendetail.symbol;
                        ViewBag.Price = tokendetail.price;
                        ViewBag.TotalSupply = tokendetail.total_supply;
                        ViewBag.Total_Holders = tokendetail.total_holders;
                        ViewBag.Name = tokendetail.name;
                    }
                    dr.Dispose();
                    cmd.Dispose();


                    //select all symbols from db
                    string sql2 = "select symbol from token";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                    List<string> tokensymbols = new List<string>();

                    MySqlDataReader dr2 = cmd2.ExecuteReader();
                    if (dr2.HasRows)
                    {
                        var dt2 = new DataTable();
                        dt2.Load(dr2);
                        int i = 0;
                        var tokensymbol = new Models.TokenSymbols();
                        foreach (DataRow record in dt2.Rows)
                        {
                            tokensymbols.Add(record["symbol"].ToString());
                        }
                        ViewBag.TokenSymbols = tokensymbols;
                    }
                    dr2.Dispose();
                    cmd2.Dispose();
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                conn.Close();
                conn.Dispose();
            }
            
            return View();
        }

    }
}
