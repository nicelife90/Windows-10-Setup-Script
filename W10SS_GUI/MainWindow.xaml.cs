using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using W10SS_GUI.Controls;
using Windows10SetupScript.Classes;
using Windows10SetupScript.Controls;

namespace W10SS_GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HamburgerCategoryButton Lastclickedbutton;        
        private ErrorsHelper ErrorsHelper;
        private List<ToggleSwitch> TogglesSwitches = new List<ToggleSwitch>();        
        private uint ActiveToggles = default(uint);
        private List<StackPanel> CategoryPanels { get; set; }
        internal List<PowerScript> PowerScriptsData { get; set; }

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

        internal string LastClickedButtonName => Lastclickedbutton?.Name as string;

        internal string AppBaseDir { get; } = AppDomain.CurrentDomain.BaseDirectory;

        public MainWindow()
        {
            ErrorsHelper = new ErrorsHelper(this);            
            InitializeComponent();            
        }

        private void InitializeVariables()
        {
            CategoryPanels = panelTogglesCategoryContainer.Children.OfType<StackPanel>().ToList();            
        }

        private void SetUiLanguage()
        {
            Resources.MergedDictionaries.Add(Culture.GetCultureDictionary());
        }        

        private void InitializeToggles()
        {
            CategoryPanels.ForEach(panel =>
            {
                PowerScriptsData.Where(script => script.Path.StartsWith(panel.Tag as string))
                                .ToList()
                                .ForEach(script => CreateToogleSwitch(toggleData: script, parentPanel: panel));

                //if (panel.Children.Count == 0)
                //    ErrorsHelper.SetInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_WarningTriangle] as string,
                //                                      InfoPanelText: CONST.Error_NoPsFilesFound,
                //                                      ParentElement: panel,
                //                                      ChildrenClear: true);
            });            
        }

        private void CreateToogleSwitch(PowerScript toggleData, StackPanel parentPanel)
        {
            ToggleSwitch toggleSwitch = new ToggleSwitch()
            {
                Id = toggleData.Id,
                ScriptPath = File.Exists(toggleData.Path) ? toggleData.Path : null
            };

            if (toggleData.DescriptionEn.Length == 0 || toggleData.DescriptionRu.Length == 0)
            {
                toggleSwitch.DescriptionVisibility = Visibility.Collapsed;
            }

            if (toggleSwitch.ScriptPath != null)
            {
                string dictionaryHeaderId = $"TH_{toggleSwitch.Id}";
                string dictionaryDescriptionId = $"TD_{toggleSwitch.Id}";

                Culture.SetToAllResourceDictionaryKeyValue(resourceID: dictionaryHeaderId,
                                                           valueEN: toggleData.HeaderEn,
                                                           valueRU: toggleData.HeaderRu);

                Culture.SetToAllResourceDictionaryKeyValue(resourceID: dictionaryDescriptionId,
                                                           valueEN: toggleData.DescriptionEn,
                                                           valueRU: toggleData.DescriptionRu);

                toggleSwitch.SetResourceReference(ToggleSwitch.HeaderProperty, dictionaryHeaderId);
                toggleSwitch.SetResourceReference(ToggleSwitch.DescriptionProperty, dictionaryDescriptionId);

                toggleSwitch.IsSwitched += SetButtonsStateIfToggleIsSwitched;                
                parentPanel.Children.Add(toggleSwitch);
                TogglesSwitches.Add(toggleSwitch);
            }           
        }        

        private void SetButtonsStateIfToggleIsSwitched(object IsChecked, EventArgs e)
        {
            if ((bool)IsChecked)
                ActiveToggles++;            

            else
                ActiveToggles--;

            HamburgerApplySettings.IsEnabled = ActiveToggles > 0 ? true : false ;
            HamburgerSaveSettings.IsEnabled = ActiveToggles > 0 ? true : false;
        }        

        internal void SetActivePanel(HamburgerCategoryButton button)
        {
            Lastclickedbutton = button;
            CategoryPanels.ForEach(p => p.Visibility = p.Tag == button.Tag ? Visibility.Visible : Visibility.Collapsed);
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
            ShowHamburgerReOpenAnimation();
            SetHamburgerWidth();
            SetTogglesStateTextFormCurrentCulture();
            textTogglesHeader.Text = Convert.ToString(Resources[LastClickedButtonName]);            
        }

        private void SetTogglesStateTextFormCurrentCulture()
        {
            TogglesSwitches.ForEach(t => t.SetToggleStateText());
        }

        private void ShowHamburgerReOpenAnimation()
        {
            if (panelHamburger.ActualWidth == panelHamburger.MaxWidth)
                ShowAnimation(storyboardName: CONST.Animation_Hamburger_REOPEN, animationTo: panelHamburger.MinWidth, element: panelHamburger);            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            InitializeVariables();            
            SetUiLanguage();

            if (ErrorsHelper.HasErrors)
            {
                ErrorsHelper.FixErrors();                
            }

            else
            {
                InitializeToggles();
                SetHamburgerWidth();
                SetActivePanel(HamburgerPrivacy);
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryPanels.ForEach(panel =>
            {
                if (panel.Children.Count == 0)
                    ErrorsHelper.SetInfoPanelControls(InfoPanelIcon: Application.Current.Resources[CONST.InfoPanel_WarningTriangle] as string,
                                                      InfoPanelText: CONST.Error_NoPsFilesFound,
                                                      ParentElement: panel,
                                                      ChildrenClear: true);
            });
        }
    }
}
