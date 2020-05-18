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
        private Storyboard storyboardHamburgerWidth;
        private DoubleAnimation animationHamburgerWidth;

        private double hamburgerMaxWidth = (double) Application.Current.Resources["Controls.Hamburger.MaxWidth"];
        private double hamburgerMinWidth = (double)Application.Current.Resources["Controls.Hamburger.MinWidth"];

        public HamburgerMenu()
        {
            InitializeComponent();
            Initializevariables();
        }

        private void Initializevariables()
        {
            storyboardHamburgerWidth = TryFindResource("Animation.HamburgerMenuButton.MouseLeftButtonDown") as Storyboard;                
            animationHamburgerWidth = storyboardHamburgerWidth.Children.First() as DoubleAnimation;
        }

        private void Hamburger_MenuClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Dispatcher.Invoke(() =>
            {
                animationHamburgerWidth.To = Hamburger.ActualWidth == hamburgerMinWidth ? hamburgerMaxWidth : hamburgerMinWidth;
                storyboardHamburgerWidth.Begin(Hamburger);
            }));
        }
    }
}
