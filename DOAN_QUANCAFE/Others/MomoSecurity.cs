using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DOAN_QUANCAFE.Others
{
    public class MomoSecurity
    {
        // Hàm này dùng để tạo Chữ ký điện tử (Signature) bằng thuật toán HMACSHA256
        public string signSHA256(string message, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
                return hex;
            }
        }
    }
}