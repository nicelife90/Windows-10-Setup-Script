using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Power_App.Helpers
{
    internal static class ResourceHelper
    {
        internal static double HamburgerMaxWidthValue = (double)Application.Current.Resources["Controls.Hamburger.MaxWidth"];
        internal static double HamburgerMinWidthValue = (double)Application.Current.Resources["Controls.Hamburger.MinWidth"];
    }
}
