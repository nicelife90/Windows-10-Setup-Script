using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W10SS_GUI;

namespace Windows10SetupScript.Classes
{
    class Test
    {
        public bool HasError { get; set; }
        public string Description { get; set; }
        internal static Test OsVersion()
        {
            return new Test()
            {
                HasError = Convert.ToInt32(Environment.OSVersion.Version.Build) >= Convert.ToInt32(CONST.Win10_Build)
                             && Convert.ToInt32(Environment.OSVersion.Version.Major) == Convert.ToInt32(CONST.Win10_Major)
                             ? false : true,

                Description = CONST.Error_OsVersionNotSupported
            };
        }

        internal static Test SettingsJsonExist(string filePath)
        {
            return new Test()
            {
                HasError = File.Exists(filePath) ? false : true,
                Description = CONST.Error_SettingsFileNotExist
            };
        }

        internal static Test SettingsJsonHash(string filePath)
        {
            return new Test()
            {
                HasError = FileHash.SHA256Compare(filePath, CONST.Settings_Json_Sha256) ? false : true,
                Description = CONST.Error_SettingsFileModified
            };
        }

        internal static Test SettingsJsonReadData(MainWindow MainWindow)
        {
            bool result;

            try
            {                
                string jsonData = File.ReadAllText(Path.Combine(MainWindow.AppBaseDir, CONST.Settings_Json_File));
                MainWindow.PowerScriptsData = JsonConvert.DeserializeObject<List<PowerScript>>(jsonData);
                result = false;
            }
            catch (Exception)
            {
                result = true;
            }

            return new Test()
            {
                HasError = result,
                Description = CONST.Error_SettingsFileNotRead
            };
        }

        internal static Test NewtonsoftJsonExist(string filePath)
        {
            return new Test()
            {
                HasError = File.Exists(filePath) ? false : true,
                Description = CONST.Error_NewtonsoftJsonFileNotExist
            };
        }
    }

    
}
