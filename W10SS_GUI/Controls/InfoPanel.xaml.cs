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
    /// Логика взаимодействия для InfoPanel.xaml
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        public InfoPanel()
        {
            InitializeComponent();            
            Application.Current.MainWindow.SizeChanged += OnMainWindow_SizeChanged;
            Application.Current.MainWindow.StateChanged += OnMainWindow_SizeChanged;
        }

        private void SetActualHeight()
        {            
            gridInfoPanel.Height = Application.Current.MainWindow.ActualHeight / 2;
        }

        private void OnMainWindow_SizeChanged(object sender, EventArgs e)
        {
            SetActualHeight();
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(InfoPanel), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InfoPanel), new PropertyMetadata(default(string)));
    }
}
