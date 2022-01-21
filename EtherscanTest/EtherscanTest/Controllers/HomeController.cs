using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace EtherscanTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

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

                //Retrieve all token details from db
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
                string[] tokensymbols = { };
                MySqlDataReader dr2 = cmd2.ExecuteReader();
                if (dr2.HasRows)
                {
                    var dt2 = new DataTable();
                    dt2.Load(dr2);
                    int i = 0;
                    var tokensymbol = new Models.TokenSymbols();
                    foreach (DataRow record in dt2.Rows)
                    {
                        tokensymbols[i] = record["symbol"].ToString();
                        
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

    }
}
