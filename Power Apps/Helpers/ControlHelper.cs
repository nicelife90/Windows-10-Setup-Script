using Power_App.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Power_App.Helpers
{
    internal static class ControlHelper
    {
        internal static void SetToolTipContent(dynamic clickedButton, bool condition)
        {
            clickedButton.ToolTip = condition ? clickedButton.Text : null;            
        }
        
        internal static Point GetMainWindowRelativePoint(dynamic control)
        {
            return control.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);
        }
    }
}
