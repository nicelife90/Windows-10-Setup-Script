using System;
using System.IO;
using System.Security.Cryptography;

namespace Windows10SetupScript.Classes
{
    internal class FileHash
    {
        internal static bool SHA256Compare(string filePath, string settings_Json_Sha256)
        {
            bool result = false;

            using (SHA256 sha = SHA256.Create())
            {
                try
                {
                    FileStream fileStream = new FileStream(filePath, FileMode.Open)
                    {
                        Position = 0
                    };

                    string hash = BitConverter.ToString(sha.ComputeHash(fileStream)).Replace("-", "");
                    result = hash == CONST.Settings_Json_Sha256 ? true : false;
                }
                catch (Exception)
                {
                    return result;
                }
            }           

            return result;
        }
    }
}