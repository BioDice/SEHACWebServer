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

namespace SeHacWebServer.Database
{
    static class UserAuthentication
    {
        /// <summary>
        /// Authenticates the user
        /// </summary>
        /// <param name="user">user from login form</param>
        /// <param name="password">password from login form</param>
        public static void Authenticate(String user,String password)
        {
            String encryptedPass = Encrypt(password);
            string constr = Settings.Default.UserDbConnectionString;
            SqlConnection con = new SqlConnection(constr);
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.CommandText = "SELECT Password FROM Users WHERE Name = @Username" ;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@Username", user);
            con.Open();
            string _password = command.ExecuteScalar().ToString();
            if (encryptedPass.Equals(_password))
            {
                //woop woop login bitch Session alles
            }
            con.Close();
        }

        /// <summary>
        /// Method to encrypt a password
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
