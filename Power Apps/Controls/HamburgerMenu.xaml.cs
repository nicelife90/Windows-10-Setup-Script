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

        private void Hamburger_MenuClick(object sender, RoutedEventArgs e)
        {
            AnimationHelper.ShowDoubleAnimation(storyboardName: "Animation.HamburgerMenuButton.MouseLeftButtonDown",
                                                 animationTo: Hamburger.ActualWidth == ResourceHelper.HamburgerMaxWidthValue ? ResourceHelper.HamburgerMinWidthValue : ResourceHelper.HamburgerMaxWidthValue,
                                                 animatedElement: Hamburger,
                                                 dispatcher: Dispatcher);
        }

        private void HamburgerIconButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            HamburgerIconButton sourceButton = e.OriginalSource as HamburgerIconButton;
            sourceButton.ToolTip = Hamburger.ActualWidth == ResourceHelper.HamburgerMinWidthValue
                                   ? ControlHelper.SetToolTipContent(sourceButton.Text)
                                   : null;
        }        

        private void HamburgerPathButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            HamburgerPathButton sourceButton = e.OriginalSource as HamburgerPathButton;
            sourceButton.ToolTip = Hamburger.ActualWidth == ResourceHelper.HamburgerMinWidthValue
                                   ? ControlHelper.SetToolTipContent(sourceButton.Text)
                                   : null;
        }
    }
}
