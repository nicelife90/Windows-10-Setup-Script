using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using W10SS_GUI.Controls;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using Windows10SetupScript.Classes;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows10SetupScript.Controls;
using Microsoft.Win32;

namespace W10SS_GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private HamburgerCategoryButton lastclickedbutton;        
        private Dictionary<string, StackPanel> togglesCategoryAndPanels = new Dictionary<string, StackPanel>();
        private List<ToggleSwitch> TogglesSwitches = new List<ToggleSwitch>();
        private uint TogglesCounter = default(uint);
        private uint activeToggles = default(uint);

        private OpenFileDialog ofd = new OpenFileDialog
        {
            DefaultExt = CONST.Ofd_Ext,
            Filter = CONST.Ofd_Filter,
            FileName = CONST.Ofd_FileName,
            Multiselect = false
        };

        private SaveFileDialog sfd = new SaveFileDialog
        {
            DefaultExt = CONST.Ofd_Ext,
            Filter = CONST.Ofd_Filter,
            FileName = CONST.Sfd_FileName
        };


        internal string LastClickedButtonName => lastclickedbutton.Name as string;        
                
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeVariables()
        {
            foreach (var tagValue in Application.Current.Resources.MergedDictionaries.Where(r => r.Source.LocalPath == CONST.Resources_Dictionaries_TAGS).First().Values)
            {
                togglesCategoryAndPanels.Add(tagValue.ToString(), panelTogglesCategoryContainer.Children.OfType<StackPanel>().Where(p => p.Tag == tagValue).First());
            }
        }

        private void SetUiLanguage()
        {
            Resources.MergedDictionaries.Add(Culture.GetCultureDictionary());
        }        

        private void InitializeToggles()
        {
            for (int i = 0; i < togglesCategoryAndPanels.Keys.Count; i++)
            {
                string categoryName = togglesCategoryAndPanels.Keys.ToList()[i];
                StackPanel categoryPanel = togglesCategoryAndPanels[categoryName];
                string psScriptsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, categoryName);

                List<ToggleSwitch> togglesSwitches = Directory.Exists(psScriptsDir)
                    && Directory.EnumerateFiles(psScriptsDir, "*.*", SearchOption.AllDirectories)
                                .Where(f => f.EndsWith(CONST.PS_EXTENSION))
                                .Count() > 0

                    ? Directory.EnumerateFiles(psScriptsDir, "*.*", SearchOption.AllDirectories)
                               .Where(f => f.EndsWith(CONST.PS_EXTENSION))
                               .Select(f => CreateToogleSwitchFromPsFiles(f))
                               .ToList()
                    : null;

                togglesSwitches?.Where(s => s.IsValid == true).ToList().ForEach(s =>
                {
                    s.IsSwitched += SetButtonsStateIfToggleIsSwitched;
                    categoryPanel.Children.Add(s);
                });

                SetInfoPanelElement(categoryPanel);
            }
        }

        private void SetInfoPanelElement(StackPanel panel)
        {
            if (panel.Children.Count == 0)
            {
                InfoPanel info = new InfoPanel();
                info.Height = Window.Height / 2;
                panel.Children.Add(info);                
            }
        }

        private void SetButtonsStateIfToggleIsSwitched(object IsChecked, EventArgs e)
        {
            if ((bool)IsChecked) activeToggles++;            
            else activeToggles--;
            HamburgerApplySettings.IsEnabled = activeToggles > 0 ? true : false ;
            HamburgerSaveSettings.IsEnabled = activeToggles > 0 ? true : false;
        }

        private ToggleSwitch CreateToogleSwitchFromPsFiles(string scriptPath)
        {
            string dictionaryHeaderID = $"TH_{TogglesCounter}";
            string dictionaryDescriptionID = $"TD_{TogglesCounter}";
            string[] arrayLines = new string[4];

            ToggleSwitch toggleSwitch = new ToggleSwitch()
            {
                ScriptPath = scriptPath
            };

            try
            {
                using (StreamReader streamReader = new StreamReader(scriptPath, Encoding.UTF8))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string textLine = streamReader.ReadLine();
                        toggleSwitch.IsValid = textLine.StartsWith("#") ? true : false;
                        arrayLines[i] = textLine.Replace("# ", "");
                    }
                }
            }
            catch (Exception)
            {
                // Do Nothing
            }

            string[] keys = { dictionaryHeaderID, dictionaryDescriptionID };
            string[] valuesEN = { arrayLines[0], arrayLines[1] };
            string[] valuesRU = { arrayLines[2], arrayLines[3]};

            Culture.SetCultureDictionaryKeyValue(CONST.LANG_EN, keys, valuesEN);
            Culture.SetCultureDictionaryKeyValue(CONST.LANG_RU, keys, valuesRU);            

            toggleSwitch.SetResourceReference(ToggleSwitch.HeaderProperty, dictionaryHeaderID);
            toggleSwitch.SetResourceReference(ToggleSwitch.DescriptionProperty, dictionaryDescriptionID);

            TogglesCounter++;
            TogglesSwitches.Add(toggleSwitch);
            return toggleSwitch;
        }

        internal void SetActivePanel(HamburgerCategoryButton button)
        {
            lastclickedbutton = button;

            foreach (KeyValuePair<string, StackPanel> kvp in togglesCategoryAndPanels)
            {
                kvp.Value.Visibility = kvp.Key == button.Tag as string ? Visibility.Visible : Visibility.Collapsed;
            }

            textTogglesHeader.Text = Resources[button.Name] as string;
            scrollTogglesCategoryPrivacy.ScrollToTop();
        }        

        internal void SetHamburgerWidth()
        {            
            panelHamburger.MaxWidth = Culture.IsRU
                ? Convert.ToDouble(TryFindResource(CONST.Hamburger_MaxWidth_RU))
                : Convert.ToDouble(TryFindResource(CONST.Hamburger_MaxWidth_EN));            
        }

        private void ButtonHamburger_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(sender as HamburgerCategoryButton);
        }

        private void ButtonHamburgerLanguageSettings_Click(object sender, MouseButtonEventArgs e)
        {
            Resources.MergedDictionaries.Add(Culture.ChangeCultureDictionary());            
            HamburgerReOpenAnimation();
            SetHamburgerWidth();
            SetTogglesStateTextFormCurrentCulture();
            textTogglesHeader.Text = Convert.ToString(Resources[LastClickedButtonName]);            
        }

        private void SetTogglesStateTextFormCurrentCulture()
        {
            TogglesSwitches.ForEach(t => t.SetToggleStateText());
        }

        private void HamburgerReOpenAnimation()
        {
            if (panelHamburger.ActualWidth == panelHamburger.MaxWidth)
                ShowAnimation(storyboardName: CONST.Animation_Hamburger_REOPEN, animationTo: panelHamburger.MinWidth, element: panelHamburger);            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            InitializeVariables();            
            SetMainWindowHeight();
            SetUiLanguage();
            InitializeToggles();
            SetHamburgerWidth();
            SetActivePanel(HamburgerPrivacy);
        }        

        private void SetMainWindowHeight()
        {
            Window.Height = SystemParameters.WorkArea.Height > CONST.Window_MinHeight
                ? SystemParameters.WorkArea.Height
                : Window.MinHeight;
        }

        private void ButtonHamburgerMenu_Click(object sender, MouseButtonEventArgs e)
        {
            Double animationTo = panelHamburger.ActualWidth == panelHamburger.MaxWidth
                ? panelHamburger.MinWidth
                : panelHamburger.MaxWidth;
            ShowAnimation(storyboardName: CONST.Animation_Hamburger, animationTo: animationTo, element: panelHamburger);
        }

        private void ShowAnimation(String storyboardName, Double animationTo, FrameworkElement element)
        {
            Storyboard storyboard = TryFindResource(storyboardName) as Storyboard;
            DoubleAnimation animation = storyboard.Children.First() as DoubleAnimation;
            animation.To = animationTo;            
            storyboard.Begin(element);
        }
        
        private void ButtonHamburgerOpenGithub_Click(object sender, MouseButtonEventArgs e)
        {
            Task.Run(() => Process.Start(CONST.W10SS_GitHub));
        }

        private void ButtonHamburgerLoadSettings_Click(object sender, MouseButtonEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                //TODO Load Settings Logics !!!
                throw new NotImplementedException();
            }
        }

        private void PanelHamburger_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonHamburgerSaveSettings_Click(object sender, MouseButtonEventArgs e)
        {
            if (sfd.ShowDialog() == true)
            {
                //TODO Save Settings Logics !!!
                throw new NotImplementedException();
            }
        }
    }
}
