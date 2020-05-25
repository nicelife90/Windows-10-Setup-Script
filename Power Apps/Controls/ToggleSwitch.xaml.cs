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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Power_App.Controls
{
    /// <summary>
    /// Логика взаимодействия для ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                                                typeof(RoutedEventHandler), typeof(ToggleSwitch));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { AddHandler(ClickEvent, value); }
        }

        public ToggleSwitch()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routedArgs = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(routedArgs);
        }
    }
}
