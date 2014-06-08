using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SeHacWebServer.Properties;

namespace SeHacWebServer.Database
{
    public sealed class SessionManager
    {
        /// <summary>
        /// Volatile omdat hij dan pas kan worden aangeroepen als er een instantie van is gemaakt
        /// </summary>
        private static volatile SessionManager instance;
        private static object syncRoot = new Object();
        private SessionManager() { }
        
        /// <summary>
        /// met lock voor multithreaded en dubbele instance == null check zodat threads elkaar niet kunnen blokeren
        /// </summary>
        public static SessionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SessionManager();
                        }
                    }
                }
                return instance;
            }
        }

        public static bool SessionExists(String user)
        {
            //string constr = Settings.Default.UserDbConnectionString;
            //SqlConnection con = new SqlConnection(constr);
            //SqlCommand command = new SqlCommand();
            //command.Connection = con;
            //command.CommandText = "SELECT Id FROM Session Where Name = @UserName";
            //command.CommandType = CommandType.Text;
            //command.Parameters.AddWithValue(@"UserName", user);
            //con.Open();
            //int userId = Convert.ToInt32(command.ExecuteScalar().ToString());
            //con.Close();
            return false;
        }

        /// <summary>
        /// OWASP TOP 10 - A1 - Geen SQL injection door het gebruik van parameters en een whitelist
        /// </summary>
        /// <param name="user"></param>
        public static void addSession(String user)
        {
            var positiveIntRegex = new Regex(@"^\w+$");
            if (!positiveIntRegex.IsMatch(user))
            {
                return;
            }

            string constr = Settings.Default.UserDbConnectionString;
            SqlConnection con = new SqlConnection(constr);
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.CommandText = "SELECT Id FROM Users Where Name = @UserName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue(@"UserName",user);
            con.Open();
            int userId = Convert.ToInt32(command.ExecuteScalar().ToString());
            con.Close();

            command.CommandText = "INSERT INTO Session VALUES(@id)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@id", userId);
            con.Open();
            command.ExecuteNonQuery();
            con.Close();
        }
    }
}
