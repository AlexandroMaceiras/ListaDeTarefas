using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ListaDeTarefas.Models
{
    public class HashCode
    {
        public string Codificar(string str)
        {
            string rethash = "";
            try
            {
                SHA1 hash = SHA1.Create();
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] combined = encoder.GetBytes(str);
                hash.ComputeHash(combined);
                rethash = Convert.ToBase64String(hash.Hash);
            }
            catch (Exception ex)
            {
                string strerr = "Error in HashCode : " + ex.Message;
            }
            return rethash;
        }
    }
}