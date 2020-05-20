using Power_App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Power_App.Controls
{
    /// <summary>
    /// Логика взаимодействия для HamburgerMenu.xaml
    /// </summary>
    public partial class HamburgerMenu : UserControl
    {
        public HamburgerMenu()
        {
            InitializeComponent();         
        }        

        private void HamburgerButton_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {            
            AnimationHelper.ShowDoubleAnimation(storyboardName: "Animation.HamburgerMenuButton.MouseLeftButtonDown",
                                                 animationTo: Hamburger.ActualWidth == ResourceHelper.HamburgerMaxWidthValue ? ResourceHelper.HamburgerMinWidthValue : ResourceHelper.HamburgerMaxWidthValue,
                                                 animatedElement: Hamburger,
                                                 dispatcher: Dispatcher);            
        }        

        private void Hamburger_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            RoutedEventHelper.StopEvent(e);
            ShowHamburgerSelectorAnimation(e.OriginalSource);
        }

        private void ShowHamburgerSelectorAnimation(dynamic clickedButton)
        {
            AnimationHelper.ShowDoubleAnimation(storyboardName: "Animation.HamburgerSelector.Move",
                                                 animationTo: (ControlHelper.GetMainWindowRelativePoint(clickedButton)).Y,
                                                 animatedElement: HamburgerSelector,
                                                 dispatcher: Dispatcher);           
        }

        private void Hamburger_MouseEnter(object sender, RoutedEventArgs e)
        {
            RoutedEventHelper.StopEvent(e);
            ControlHelper.SetToolTipContent(clickedButton: e.OriginalSource, 
                                            condition: Hamburger.ActualWidth == ResourceHelper.HamburgerMinWidthValue);
        }
    }
}
