using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Donkey.Administration.WebApi
{
    internal class Sha265Encoder : IHashEncoder
    {
        public string GetHash(string data)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                byte[] hashedBytes = sha256.ComputeHash(dataBytes);

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
