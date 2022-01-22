using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using EtherscanTest.Models;
using Newtonsoft.Json;

namespace EtherscanTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var connection = ConfigurationManager.AppSettings["mysqlConnection"].ToString();

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
                        td.RankID = i;
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
                string sql2 = "select SUM(total_supply) AS 'total_supply' from token";
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
           
            return View();

        }

        public ActionResult Detail()
        {
            ViewBag.Title = "Detail";

            string id = "";

            if (Request.QueryString["id"] != null)
            {
                id = Request.QueryString["id"];
                ViewBag.IsDetail = true;
            }
            else
            {
                ViewBag.IsDetail = false;
            }


            var connection = ConfigurationManager.AppSettings["mysqlConnection"].ToString();

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
        public bool UpdateTokenData(string name, string symbol , string cont_add, string tot_supply, string tot_holders)
        {
            bool isSuccess = false;






            return isSuccess;
        }
    }
}
