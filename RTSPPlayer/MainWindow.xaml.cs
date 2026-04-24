using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RTSPPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Media.Close();
        }


        public void ShowHandle()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                //    label.Content = "Processing completed";
                Show();
            }));
        }
        public void HideHandle()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                //    label.Content = "Processing completed";
                Hide();
            }));
        }
    }
}