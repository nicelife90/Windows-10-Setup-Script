using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using W10SS_GUI;
using Windows10SetupScript.Controls;

namespace Windows10SetupScript.Classes
{
    class ErrorsHelper
    {
        private MainWindow MainWindow;
        public bool HasErrors { get; private set; } = false;
                        
        public Action FixErrors { get; private set;}

        private void WatchDog(Test result)
        {
            HasErrors = result.Result;

            if (result.Description == CONST.Error_OsVersionNotSupported)
            {
                FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_WarningRobot] as string, 
                    InfoPanelText: CONST.Error_OsVersionNotSupported, 
                    ParentElement: MainWindow.gridContainer,
                    ChildrenClear: true);
            }

            else if (result.Description == CONST.Error_SettingsFileNotExist)
            {
                FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_Magnifier] as string,
                    InfoPanelText: CONST.Error_SettingsFileNotExist,
                    ParentElement:MainWindow.gridContainer,
                    ChildrenClear:true);
            }

            else if (result.Description == CONST.Error_SettingsFileModified)
            {
                FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_Modify] as string,
                    InfoPanelText: CONST.Error_SettingsFileModified,
                    ParentElement: MainWindow.gridContainer,
                    ChildrenClear: true);
            }
        }

        private void AddInfoPanelControls(string InfoPanelIcon, string InfoPanelText, Panel ParentElement, bool ChildrenClear)
        {
            InfoPanel infoPanel = new InfoPanel();
            infoPanel.Icon = InfoPanelIcon;
            infoPanel.SetResourceReference(InfoPanel.TextProperty, InfoPanelText);            

            if (ChildrenClear)
                ParentElement.Children.Clear();

            ParentElement.Children.Add(infoPanel);            
        }
        
        public ErrorsHelper(MainWindow mainWindow)            
        {
            MainWindow = mainWindow;
            Queue<Func<Test>> funcQueue = new Queue<Func<Test>>();
            funcQueue.Enqueue(() => TestOsVersion());
            funcQueue.Enqueue(() => TestSettingsFileExist());
            funcQueue.Enqueue(() => TestSettingsFileHash(Path.Combine(MainWindow.AppBaseDir, CONST.Settings_Json_File)));

            int countQueue = funcQueue.Count;
            for (int i = 0; i < countQueue; i++)
            {
                WatchDog(funcQueue.Dequeue().Invoke());

                if (HasErrors)
                    break;
            }
        }

        private Test TestSettingsFileHash(string filePath)
        {
            return new Test()
            {
                Result = FileHash.SHA256Compare(filePath, CONST.Settings_Json_Sha256) ? false : true,
                Description = CONST.Error_SettingsFileModified
            };
        }

        private Test TestSettingsFileExist()
        {
            return new Test()
            {
                Result = File.Exists(Path.Combine(MainWindow.AppBaseDir, CONST.Settings_Json_File)) ? false :true,
                Description = CONST.Error_SettingsFileNotExist            
            };
        }

        private Test TestOsVersion()
        {
            return new Test()
            {
                Result = Convert.ToInt32(Environment.OSVersion.Version.Build) >= Convert.ToInt32(CONST.Win10_Build)
                             && Convert.ToInt32(Environment.OSVersion.Version.Major) == Convert.ToInt32(CONST.Win10_Major)
                             ? false : true,

                Description = CONST.Error_OsVersionNotSupported
            };            
        }        

        
    }
}


