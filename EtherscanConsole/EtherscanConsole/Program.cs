using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Threading;

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



            Console.ReadKey();
        }

        static void FiveMinuteTicker()
        {
            var connection = ConfigurationManager.ConnectionStrings["mysqlConnection"].ConnectionString;

            while (true)
            {
                CryptoCompare();
                Thread.Sleep(1000 * 60 * 5); // 5 minutes

            }
        }

        static async Task<Token> GetTokenPriceAsync(string token)
        {
            Token token = null;
        }


        static void CryptoCompare()
        {
            var connection = ConfigurationManager.ConnectionStrings["mysqlConnection"].ConnectionString;

            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();

                string sql = "select symbol from etherscan.token;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            conn.Close();
            conn.Dispose();
        }

    }
}
