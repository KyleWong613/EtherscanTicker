using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

namespace EtherscanConsole
{
    class Program
    {
        private static Timer intimer;

        static void Main(string[] args)
        {
            //System.Timers.Timer 5mintimer = new System.Timers.Timer();
            //5mintimer.Interval = 30000;
            var conenction = ConfigurationManager.ConnectionStrings["mysqlConnection"].ConnectionString;

            MySqlConnection conn = new MySqlConnection(conenction);
            try
            {
                conn.Open();

                string sql = "select * from etherscan.token;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0] + "-" + rdr[1]);
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



    }
}
