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

namespace W10SS_GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal class Culture
        {
            internal const string EN = "en";
            internal const string RU = "ru";
        }

        private static string culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == Culture.RU ? Culture.RU : Culture.EN;
        private HamburgerCategoryButton lastclickedbutton;        
        private Dictionary<string, StackPanel> togglesCategoryAndPanels = new Dictionary<string, StackPanel>();
        private List<ToggleSwitch> TogglesSwitches = new List<ToggleSwitch>();
        private uint TogglesCounter = default(uint);
        private uint activeToggles = default(uint);
        private static ResourceDictionary resourceDictionaryEn = new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localized/EN.xaml", UriKind.Absolute) };
        private static ResourceDictionary resourceDictionaryRu = new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localized/RU.xaml", UriKind.Absolute) };       

        internal string LastClickedButtonName => lastclickedbutton.Name as string;        
                
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeVariables()
        {
            foreach (var tagValue in Application.Current.Resources.MergedDictionaries.Where(r => r.Source.LocalPath == "/Resource/tags.xaml").First().Values)
            {
                togglesCategoryAndPanels.Add(tagValue.ToString(), panelTogglesCategoryContainer.Children.OfType<StackPanel>().Where(p => p.Tag == tagValue).First());
            }
        }

        private void SetUiLanguage()
        {
            Resources.MergedDictionaries.Add(GetCurrentCulture());
        }

        internal static ResourceDictionary GetCurrentCulture() => culture == Culture.RU ? resourceDictionaryRu : resourceDictionaryEn;

        internal static string GetCurrentCultureName() => culture == Culture.RU ? Culture.RU : Culture.EN;

        internal static ResourceDictionary ChangeCulture()
        {
            culture = culture == Culture.RU ? Culture.EN : Culture.RU;
            return culture == Culture.RU ? resourceDictionaryRu : resourceDictionaryEn;
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
                                .Where(f => f.EndsWith(".ps1"))
                                .Count() > 0

                    ? Directory.EnumerateFiles(psScriptsDir, "*.*", SearchOption.AllDirectories)
                               .Where(f => f.EndsWith(".ps1"))
                               .Select(f => CreateToogleSwitchFromPsFiles(f))
                               .ToList()
                    : null;

                togglesSwitches?.Where(s => s.IsValid == true).ToList().ForEach(s =>
                {
                    s.IsSwitched += SetApplyButtonState;
                    categoryPanel.Children.Add(s);
                });
            }
        }

        private void SetApplyButtonState(object senderIsChecked, EventArgs e)
        {
            if ((bool)senderIsChecked) activeToggles++;            
            else activeToggles--;
            HamburgerApplySettings.IsEnabled = activeToggles > 0 ? true : false ;                
        }

        private ToggleSwitch CreateToogleSwitchFromPsFiles(string scriptPath)
        {
            string dictionaryHeaderID = $"ToggleHeader-{TogglesCounter}";
            string dictionaryDescriptionID = $"ToggleDescription-{TogglesCounter}";
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

            }

            resourceDictionaryEn[dictionaryHeaderID] = arrayLines[0];
            resourceDictionaryEn[dictionaryDescriptionID] = arrayLines[1];
            resourceDictionaryRu[dictionaryHeaderID] = arrayLines[2];
            resourceDictionaryRu[dictionaryDescriptionID] = arrayLines[3];

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
        }

        internal void RefreshHamburgerWidth()
        {
            MouseEventArgs mouseLeaveArgs = new MouseEventArgs(Mouse.PrimaryDevice, 0)
            {
                RoutedEvent = Mouse.MouseLeaveEvent
            };

            MouseEventArgs mouseEnterArgs = new MouseEventArgs(Mouse.PrimaryDevice, 0)
            {
                RoutedEvent = Mouse.MouseEnterEvent
            };

            panelHamburger.RaiseEvent(mouseLeaveArgs);
            panelHamburger.RaiseEvent(mouseEnterArgs);
        }

        internal void SetHamburgerWidth(string cultureName)
        {
            Storyboard hamburgerOpen = this.TryFindResource("animationHamburgerOpen") as Storyboard;
            DoubleAnimation animation = hamburgerOpen.Children[0] as DoubleAnimation;
            animation.To = cultureName == "ru"
                ? Convert.ToDouble(this.TryFindResource("panelHamburgerRuMaxWidth"))
                : Convert.ToDouble(this.TryFindResource("panelHamburgerEnMaxWidth"));
        }

        private void ButtonHamburger_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(sender as HamburgerCategoryButton);
        }

        private void ButtonHamburgerLanguageSettings_Click(object sender, MouseButtonEventArgs e)
        {
            Resources.MergedDictionaries.Add(ChangeCulture());
            SetHamburgerWidth(GetCurrentCultureName());
            RefreshHamburgerWidth();
            textTogglesHeader.Text = Convert.ToString(Resources[LastClickedButtonName]);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            InitializeVariables();
            SetUiLanguage();
            InitializeToggles();
            SetHamburgerWidth(GetCurrentCultureName());
            SetActivePanel(HamburgerPrivacy);
        }

        private void ButtonHamburgerMenu_Click(object sender, MouseButtonEventArgs e)
        {
            Storyboard storyboard = panelHamburger.ActualWidth == panelHamburger.MinWidth
                ? Window.TryFindResource("animationHamburgerOpen")
                : Window.TryFindResource("animationHamburgerClose");
        }
    }
}
