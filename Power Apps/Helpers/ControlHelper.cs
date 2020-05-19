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
        internal static ToolTip SetToolTipContent(string tooltipText)
        {
            return new ToolTip()
            {
                Content = tooltipText
            };
        }
    }
}
