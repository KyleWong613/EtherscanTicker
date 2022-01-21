using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.WebUtilities;

namespace EtherscanConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.Timers.Timer 5mintimer = new System.Timers.Timer();
            //5mintimer.Interval = 30000;
            Thread printer = new Thread(new ThreadStart(FiveMinuteTicker));
            printer.Start();

            FiveMinuteTicker();
        }

        static void FiveMinuteTicker()
        {
            while (true)
            {
                CryptoCompare();
                Thread.Sleep(1000 * 60 * 5); // 5 minutes

            }
        }

        static void CryptoCompare()
        {
            string apiendpoint = "https://min-api.cryptocompare.com/data/price";


            var connection = ConfigurationManager.ConnectionStrings["mysqlConnection"].ConnectionString;
            string token = "";
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
                    Console.WriteLine(rdr[0].ToString() + tokenprice);

                    Console.WriteLine(uri);
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

        //Token Model
        public class Token
        {
            public string symbol { get; set; }
            public int price { get; set; }
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
            string result = string.Empty;
            result = sr.ReadToEnd();
            res.Close();

            return result;
        }
    }
}
