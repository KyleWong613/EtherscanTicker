using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace EtherscanConsole
{

    class Program
    {
        public static void Main()
        {
            TimerCallback tc = new TimerCallback(CryptoCompare);
            Timer timer = new Timer(CryptoCompare, null, 0, 300000); // 300000 5 minute interval

            Console.ReadLine();
        }

        public static void CryptoCompare(object obj)
        {
            string apiendpoint = "https://min-api.cryptocompare.com/data/price";

            var connection = ConfigurationManager.ConnectionStrings["mysqlConnection"].ConnectionString;
            string tokenprice = "";

            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();

                string sql = "select symbol from token;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var query = new Dictionary<string, string>()
                    {
                        ["fsym"] = rdr[0].ToString(),
                        ["tsyms"] = "USD" //tsyms fixed USD
                    };

                    var uri = QueryHelpers.AddQueryString(apiendpoint, query);
                    tokenprice = GetTokenPrice(uri);
                    var jobj = JsonConvert.DeserializeObject<TokenPrice>(tokenprice);
                    decimal token_price = Convert.ToDecimal(jobj.USD);

                    //Console.WriteLine(jobj.Response);
                    //Console.WriteLine(rdr[0].ToString() + jobj.USD);

                    //coin pair Error does not exist update price as 0
                    if (jobj.Response == "Error")
                    {
                        UpdateTokenPrice(rdr[0].ToString(), 0);
                    }
                    else
                    {
                        UpdateTokenPrice(rdr[0].ToString(), token_price);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            conn.Close();
            conn.Dispose();

            Console.ReadKey();
        }

        public static void UpdateTokenPrice(string symbol, decimal price)
        {
            string token = "";
            try
            {
                var con = ConfigurationManager.ConnectionStrings["mysqlConnection"].ConnectionString;
                MySqlConnection conn = new MySqlConnection(con);
                conn.Open();

                var query = "Update token SET price=@price WHERE symbol=@symbol";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@symbol", symbol);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Token Model
        public class Token
        {
            public string symbol { get; set; }
            public int price { get; set; }
        }
        //Results from API
        public class TokenPrice
        {
            public string USD { get; set; }
            public string Response { get; set; }
            public string Message { get; set; }
        }
        //Calling the API with GET request for respective token price
        public static string GetTokenPrice(string url)
        {
            //Request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "";
            //Response
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader sr = new StreamReader(res.GetResponseStream(), encoding);
            string json = sr.ReadToEnd();
            res.Close();

            return json;
        }
    }
}
