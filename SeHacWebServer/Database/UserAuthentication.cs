using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SeHacWebServer.Properties;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;

namespace SeHacWebServer.Database
{
    static class UserAuthentication
    {
        /// <summary>
        /// Authenticates the user
        /// </summary>
        /// <param name="user">user from login form</param>
        /// <param name="password">password from login form</param>
        public static bool Authenticate(String user,String password)
        {
            var positiveIntRegex = new Regex(@"^\w+$");
            if (!positiveIntRegex.IsMatch(user))
            {
                return false;
            }
            if (!positiveIntRegex.IsMatch(password))
            {
                return false;
            }

            String encryptedPass = Encrypt(password);
            string constr = Settings.Default.UserDbConnectionString;
            SqlConnection con = new SqlConnection(constr);
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.Parameters.AddWithValue("@Username", user);
            command.CommandText = "SELECT Password FROM Users WHERE Name = @Username";
            command.CommandType = CommandType.Text;
            
            con.Open();
            string _password = "";
            if (command.ExecuteScalar() != null)
                _password = command.ExecuteScalar().ToString();
            else
                return false;
            con.Close();
            if (encryptedPass.Equals(_password))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to encrypt a password with the EncryptionKey
        /// </summary>
        /// <param name="clearText">password to Encrypt</param>
        /// <returns></returns>
        private static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}
