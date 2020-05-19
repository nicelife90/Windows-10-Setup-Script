using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Power_App.Controls
{
    /// <summary>
    /// Логика взаимодействия для HamburgerButton.xaml
    /// </summary>
    public partial class HamburgerButton : UserControl
    {
        public static readonly RoutedEvent MenuClickEvent = EventManager.RegisterRoutedEvent("MenuClick", RoutingStrategy.Bubble,
                                                              typeof(RoutedEventHandler), typeof(HamburgerButton));

        public event RoutedEventHandler MenuClick
        {
            add { AddHandler(MenuClickEvent, value); }
            remove { RemoveHandler(MenuClickEvent, value); }
        }
        public HamburgerButton()
        {
            InitializeComponent();            
        }        

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(HamburgerButton), new PropertyMetadata(default(string)));

        private void HamburgerButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs routedArgs = new RoutedEventArgs(MenuClickEvent, this);
            RaiseEvent(routedArgs);
        }        
    }
}
