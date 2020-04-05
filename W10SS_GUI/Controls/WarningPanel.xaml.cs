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

namespace Windows10SetupScript.Controls
{
    /// <summary>
    /// Логика взаимодействия для WarningPanel.xaml
    /// </summary>
    public partial class WarningPanel : UserControl
    {
        public WarningPanel()
        {
            InitializeComponent();
            Application.Current.MainWindow.SizeChanged += OnMainWindow_SizeChanged;
            Application.Current.MainWindow.StateChanged += OnMainWindow_SizeChanged;
        }

        private void OnMainWindow_SizeChanged(object sender, EventArgs e)
        {
            SetActualWidthAndHeight();
        }

        private void SetActualWidthAndHeight()
        {
            gridWarningPanel.Width = Application.Current.MainWindow.ActualWidth;
            gridWarningPanel.Height = Application.Current.MainWindow.ActualHeight;            
        }
    }
}
