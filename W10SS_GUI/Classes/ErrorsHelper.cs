using Newtonsoft.Json;
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

        private Queue<Func<Test>> FuncQueue = new Queue<Func<Test>>();

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
            FillingQueue();
            FuncDequeue();            
        }

        private void FillingQueue()
        {
            FuncQueue.Enqueue(() => Test.OsVersion());
            FuncQueue.Enqueue(() => Test.SettingsJsonExist(Path.Combine(MainWindow.AppBaseDir, CONST.Settings_Json_File)));
            FuncQueue.Enqueue(() => Test.SettingsJsonHash(Path.Combine(MainWindow.AppBaseDir, CONST.Settings_Json_File)));
            FuncQueue.Enqueue(() => Test.SettingsJsonReadData(MainWindow));
        }

        private void FuncDequeue()
        {
            int countQueue = FuncQueue.Count;

            for (int i = 0; i < countQueue; i++)
            {
                Test test = FuncQueue.Dequeue().Invoke();
                HasErrors = test.HasError;

                if (test.HasError && test.Description == CONST.Error_OsVersionNotSupported)
                {
                    FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_WarningRobot] as string,
                        InfoPanelText: CONST.Error_OsVersionNotSupported, ParentElement: MainWindow.gridContainer,
                        ChildrenClear: true);
                    break;
                }

                else if (test.HasError && test.Description == CONST.Error_SettingsFileNotExist)
                {
                    FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_Magnifier] as string,
                        InfoPanelText: CONST.Error_SettingsFileNotExist, ParentElement: MainWindow.gridContainer,
                        ChildrenClear: true);
                    break;
                }

                else if (test.HasError && test.Description == CONST.Error_SettingsFileModified)
                {
                    FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_Modify] as string,
                        InfoPanelText: CONST.Error_SettingsFileModified, ParentElement: MainWindow.gridContainer,
                        ChildrenClear: true);
                    break;
                }

                else if (test.HasError && test.Description == CONST.Error_SettingsFileNotRead)
                {
                    FixErrors = () => AddInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_OpenBook] as string,
                        InfoPanelText: CONST.Error_SettingsFileNotRead, ParentElement: MainWindow.gridContainer,
                        ChildrenClear: true);
                    break;
                }
            }
        }        
    }
}


