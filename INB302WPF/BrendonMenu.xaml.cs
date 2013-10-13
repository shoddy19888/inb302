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
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Drawing;
using SkelFileIO;

namespace INB302WPF
{
    /// <summary>
    /// Interaction logic for BrendonMenu.xaml
    /// </summary>
    public partial class BrendonMenu : Window
    {
        List<List<List<string>>> data;
        SkelFileIO.SkelFileIO io = new SkelFileIO.SkelFileIO();

        public BrendonMenu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DepthSenseBren win = new DepthSenseBren();
            win.Show();
            
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            data = io.ReadKinLog();
            if (data != null)
            {
                MessageBox.Show("Data import Complete");
                
                lblCount.Content = "Frame Count: " + data.Count.ToString();
                
            }
            else
            {
                MessageBox.Show("You need to select a KinLog file to import");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (data != null)
            {

                lblresult.Content = "Joint: " + data[int.Parse(txtFrame.Text)][int.Parse(txtJoint.Text)][0] + " X:" + data[int.Parse(txtFrame.Text)][int.Parse(txtJoint.Text)][1] + " Y:" + data[int.Parse(txtFrame.Text)][int.Parse(txtJoint.Text)][2] + " Z:" + data[int.Parse(txtFrame.Text)][int.Parse(txtJoint.Text)][3];
            }
            else
            {
                MessageBox.Show("You need to import some data first");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SkelTest win = new SkelTest();
            win.Show();
        }
        




    }
}
