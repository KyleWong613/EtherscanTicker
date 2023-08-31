using EtherscanTest.DTO;
using EtherscanTest.Helpers.Interfaces;
using EtherscanTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Reflection.Emit;

namespace EtherscanTest.Helpers
{
    public class TokenService 
    { 
        public List<TokenDTO> GetTokenList(List<TokenDTO> Tokens)
        {
            var connection = ConfigurationManager.AppSettings["mssqlConnection"].ToString();

            using (SqlConnection conn = new SqlConnection(connection))
            {

                //set stored procedure name
                string spName = @"SelectTokens";

                //define the SqlCommand object
                SqlCommand cmd = new SqlCommand(spName, conn);

                //open connection
                conn.Open();

                //set the SqlCommand type to stored procedure and execute
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                Console.WriteLine(Environment.NewLine + "Retrieving data from database..." + Environment.NewLine);
                TokenDTO dto = new TokenDTO();

                //check if there are records
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        dto.ID = dr.GetInt32(0);
                        dto.symbol = dr.GetString(1);
                        dto.price = dr.GetString(2);
                        dto.TotalSupply = dr.GetDecimal(3);
                        dto.TotalHolders = dr.GetDecimal(4);
                        dto.name = dr.GetString(5);

                        Tokens.Add(dto);
                        //display retrieved record
                        Console.WriteLine("{0},{1},{2},{3},{4},{5}", dto.ID.ToString(), dto.symbol, dto.price, dto.TotalSupply, dto.TotalHolders, dto.name);
                    }
                }
                else
                {
                    Console.WriteLine("No data found.");
                }

            }

            return Tokens;
        }
    }
}
  
 
 