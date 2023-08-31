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

namespace EtherscanTest.Controllers
{

    public class HomeController : Controller
    {
        private string dbtype = ConfigurationManager.AppSettings["sqltype"].ToString();
        private ITokenService itokenService;
        private TokenService tokenService;

        public HomeController() : this(new TokenService())
        {
        }
        public HomeController(ITokenService tokenService)
        {
            this.itokenService = tokenService;
        }

        public HomeController(TokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var connection = "";
            List<TokenDTO> tokens = new List<TokenDTO>();
            if (dbtype == "mssql")
            {
                connection = ConfigurationManager.AppSettings["mssqlConnection"].ToString();

                List<TokenDTO> tokenList = tokenService.GetTokenList(tokens);
                var res = new List<EtherscanTest.DTO.TokenDTO>();
                int i = 1;

                foreach (TokenDTO allTokens in tokenList)
                {
                    var td = new DTO.TokenDTO();
                    td.ID = i;
                     
                    td.contract_address = allTokens.contract_address ?? "";
                    td.symbol = allTokens.symbol;
                    td.price = allTokens.price;
                    td.TotalSupply = allTokens.TotalSupply; 
                    td.TotalHolders = allTokens.TotalHolders;
                    td.name = allTokens.name;
                    res.Add(td);
                    ViewBag.AllSupply = td.TotalSupply;
                    i++;
                }

                ViewBag.TD = res;
            }  
            else
            {
                connection = ConfigurationManager.AppSettings["mysqlConnection"].ToString();

                MySqlConnection conn = new MySqlConnection(connection);
                try
                {
                    conn.Open();

                    //Retrieve all token details from db descending order based on Total Supply
                    string sql = "SELECT * FROM token order by total_supply desc";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    var res = new List<EtherscanTest.Models.TokenDetail>();

                    MySqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        int i = 1;
                        foreach (DataRow record in dt.Rows)
                        {
                            var td = new Models.TokenDetail();
                            td.ID = i;
                            td.contract_address = record["contract_address"].ToString();
                            td.symbol = record["symbol"].ToString();
                            td.price = record["Price"].ToString();
                            td.total_supply = record["Total_supply"].ToString();
                            td.total_holders = record["Total_holders"].ToString();
                            td.name = record["name"].ToString();
                            res.Add(td);
                            i++;
                        }
                        ViewBag.TD = res;
                    }

                    //retrieve sum of all token supply
                    string sql2 = "select SUM(TotalSupply) AS 'TotalSupply' from token";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                    MySqlDataReader dr2 = cmd2.ExecuteReader();
                    if (dr2.HasRows)
                    {
                        var dt2 = new DataTable();
                        dt2.Load(dr2);
                        foreach (DataRow record in dt2.Rows)
                        {
                            var td = new Models.TokenDetail();

                            td.allsupply = Convert.ToDecimal(record["total_supply"]);
                            ViewBag.AllSupply = td.allsupply;
                        }
                    }

                    dr.Dispose();
                    dr2.Dispose();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
           
            return View();

        }

        public ActionResult Detail()
        {
            ViewBag.Title = "Detail";

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

        public string GetPieChartData(List<string> gData)
        {
            List<TokenPieChart> t = new List<TokenPieChart>();

            var connection = ConfigurationManager.AppSettings["mysqlConnection"].ToString();

            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();

                //Retrieve name and total supply of token
                string sql = "SELECT name, total_supply FROM token order by total_supply desc";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    var dt = new DataTable();
                    dt.Load(dr);
                    foreach (DataRow record in dt.Rows)
                    {
                        var td = new Models.TokenPieChart();
                        td.total_supply = record["Total_supply"].ToString();
                        td.name = record["name"].ToString();
                        t.Add(td);
                    }
                }

                dr.Dispose();
                cmd.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            var items = JsonConvert.SerializeObject(t);

            return items;
        }

        public bool UpdateTokenData()
        {
            bool isSuccess = false;

            string name = "";
            string symbol = "";
            string cont_add = "";
            string tot_supply = "";
            string tot_holders = "";

            string json;
            Stream req = Request.InputStream;
            req.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(req))
            {
                json = sr.ReadToEnd();
            }
            TokenDetail tokendet = JsonConvert.DeserializeObject<TokenDetail>(json);

            name = tokendet.name;
            symbol = tokendet.symbol;
            cont_add = tokendet.contract_address;
            tot_supply = tokendet.total_supply;
            tot_holders = tokendet.total_holders;
            var connection = "";

            if (dbtype == "mssql")
            {
                 connection = ConfigurationManager.AppSettings["mssqlConnection"].ToString();
            }
            else
            {
                try
                {
                    connection = ConfigurationManager.AppSettings["mysqlConnection"].ToString();

                    MySqlConnection conn = new MySqlConnection(connection);
                    conn.Open();

                    //Check if token exists in db
                    string sql = "SELECT symbol FROM token where symbol=@symbol";

                    string symbolexists = "";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@symbol", symbol);

                    MySqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        foreach (DataRow record in dt.Rows)
                        {
                            symbolexists = record["symbol"].ToString();
                        }
                    }
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                    if (symbolexists != "")
                    {
                        MySqlConnection conn2 = new MySqlConnection(connection);
                        conn2.Open();
                        //Retrieve all token details from db based on symbol value
                        string sql2 = "update token SET name=@name , contract_address=@contract_address, total_supply=@total_supply, total_holders=@total_holders where symbol=@symbol";
                        MySqlCommand cmd2 = new MySqlCommand(sql2, conn2);
                        cmd2.Parameters.AddWithValue("@name", name);
                        cmd2.Parameters.AddWithValue("@symbol", symbol);
                        cmd2.Parameters.AddWithValue("@contract_address", cont_add);
                        cmd2.Parameters.AddWithValue("@total_supply", tot_supply);
                        cmd2.Parameters.AddWithValue("@total_holders", tot_holders);

                        cmd2.ExecuteNonQuery();

                        cmd2.Dispose();
                        conn2.Close();
                        conn2.Dispose();
                    }
                    else
                    {
                        MySqlConnection conn3 = new MySqlConnection(connection);
                        conn3.Open();
                        // if token not exists in db, insert new token into db
                        string sql3 = "INSERT INTO token (symbol, name, total_supply, contract_address, total_holders) Values(@symbol, @name, @total_supply , @contract_address, @total_holders)";
                        MySqlCommand cmd3 = new MySqlCommand(sql3, conn3);
                        cmd3.Parameters.AddWithValue("@name", name);
                        cmd3.Parameters.AddWithValue("@symbol", symbol);
                        cmd3.Parameters.AddWithValue("@contract_address", cont_add);
                        cmd3.Parameters.AddWithValue("@total_supply", tot_supply);
                        cmd3.Parameters.AddWithValue("@total_holders", tot_holders);

                        cmd3.ExecuteNonQuery();
                        cmd3.Dispose();
                        conn3.Close();
                        conn3.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
                
                

                

            return isSuccess;

        }
    }
}
