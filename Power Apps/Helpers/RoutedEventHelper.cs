using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Power_App.Helpers
{
    internal static class RoutedEventHelper
    {
        internal static void StopEvent(RoutedEventArgs e) => e.Handled = true;
    }
}
