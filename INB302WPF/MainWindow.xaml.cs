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

namespace INB302WPF
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RyanMenu window = new RyanMenu();
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BrendonMenu window = new BrendonMenu();
            window.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DepthSenor form = new DepthSenor();
            form.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SkelWin win = new SkelWin();
            win.Show();
        }
    }
}
