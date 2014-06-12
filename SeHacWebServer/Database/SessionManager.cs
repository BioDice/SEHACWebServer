using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SeHacWebServer.Model;
using SeHacWebServer.Properties;

namespace SeHacWebServer.Database
{
    public sealed class SessionManager
    {
        /// <summary>
        /// Volatile omdat hij dan pas kan worden aangeroepen als er een instantie van is gemaakt
        /// </summary>
        private static List<Session> sessionList = new List<Session>();
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

        /// <summary>
        /// check if the current session token exists
        /// </summary>
        /// <param name="sessionid"></param>
        /// <returns></returns>
        public static bool SessionExists(String sessionid,String clientIp)
        {
            if (sessionid != null)
            {
                string _cookies = sessionid.Split(new char[] { '=', ';' })[1];
                return sessionList.Exists(x => x.SessionId == _cookies && x.ClientIp == clientIp);
            }
            return false;
        }

        /// <summary>
        /// gets the sessioniD from the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string getSessionId(String user)
        {
            var positiveIntRegex = new Regex(@"^\w+$");
            if (!positiveIntRegex.IsMatch(user))
            {
                return null;
            }
            return sessionList.Find(x=>x.User == user).SessionId;
        }

        /// <summary>
        /// deletes te session with the given sessionId
        /// </summary>
        /// <param name="sessionId">the session id from the cookie</param>
        public static void deleteSession(String sessionId)
        {
            sessionList.RemoveAll(x=>x.SessionId.Equals(sessionId));
        }

        /// <summary>
        /// Dat pro code
        /// </summary>
        /// <param name="sessionId">the session id from the cookie</param>
        public static void deleteSession(Session s)
        {
            sessionList.RemoveAll(x => x.Equals(s));
        }

        /// <summary>
        /// check if the current session has admin rights
        /// </summary>
        /// <param name="sessionid"></param>
        /// <returns></returns>
        public static bool isAdmin(String sessionid)
        {
            string _cookies = sessionid.Split(new char[] { '=', ';' })[1];
            return sessionList.Exists(x=>x.Role.Equals("Administrator"));
        }

        /// <summary>
        /// OWASP TOP 10 - A1 - Geen SQL injection door het gebruik van parameters en een whitelist
        /// </summary>
        /// <param name="user"></param>
        public static string addSession(String user,String clientip)
        {
            var positiveIntRegex = new Regex(@"^\w+$");
            if (!positiveIntRegex.IsMatch(user))
            {
                return null;
            }

            string constr = Settings.Default.UserDbConnectionString;
            SqlConnection con = new SqlConnection(constr);
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.CommandText = "SELECT UserType FROM Users Where Name = @UserName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue(@"UserName",user);
            con.Open();

            string type = "";
            if (command.ExecuteScalar() != null)
                type = command.ExecuteScalar().ToString();
            con.Close();
            Session s = new Session(generateSessionId(),type,user,clientip);
            sessionList.Add(s);

            return s.SessionId;
        }

        /// <summary>
        /// generates a unique GUID
        /// </summary>
        /// <returns>a numeric GUID</returns>
        private static string generateSessionId()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }
}
