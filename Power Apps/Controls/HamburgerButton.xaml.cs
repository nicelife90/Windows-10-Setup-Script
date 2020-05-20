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
        public static new readonly RoutedEvent MouseLeftButtonDownEvent = EventManager.RegisterRoutedEvent("MouseLeftButtonDown", RoutingStrategy.Bubble,
                                                              typeof(RoutedEventHandler), typeof(HamburgerButton));        

        public new event RoutedEventHandler MouseLeftButtonDown
        {
            add { AddHandler(MouseLeftButtonDownEvent, value); }
            remove { RemoveHandler(MouseLeftButtonDownEvent, value); }
        }

        private void HamburgerButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs routedArgs = new RoutedEventArgs(MouseLeftButtonDownEvent, this);
            RaiseEvent(routedArgs);
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
    }
}
