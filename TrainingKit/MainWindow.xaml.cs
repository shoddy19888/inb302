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
using Microsoft.Kinect;

namespace TrainingKit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool rewind;
        bool rewind1;
        bool Record =false;
        Draw drawing;
        Draw drawing1;
        SkelIO io;
        SkeletonList sList;
        SkeletonList sList1;
  
        public MainWindow()
        {
            InitializeComponent();
            gridPlayback.IsEnabled = false;
            gridPlayback1.IsEnabled = false;
            drawing = new Draw(Surface);
            io = new SkelIO();
            btnRecord.IsEnabled = false;
            drawing1 = new Draw(Surface2);
            btnStart.IsEnabled = false;
            btnSetEnd.IsEnabled = false;
            btnSetEnd1.IsEnabled = false;
            btnStart1.IsEnabled = false;
            playbackSlider.IsEnabled = false;
  
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            var tempskel = io.ReadFile();

            if (tempskel == null)
            {
                return;
            }

            sList = new SkeletonList(tempskel);
            lblFrame.Content = sList.GetSkelCount().ToString();
            draw(sList.GetCurrentSkel(), sList, drawing, Brushes.Black);
            playbackSlider.Maximum = sList.GetSkelCount();
            lblCurrentFrame.Content = sList.GetCurrentSkelIndex();

            btnSetEnd.IsEnabled = true;
            btnStart.IsEnabled = true;
            playbackSlider.Value = 0;
            gridPlayback.IsEnabled = true;
            playbackSlider.IsEnabled = true;

        }
        private void btnRead1_Click(object sender, RoutedEventArgs e)
        {
            var tempskel = io.ReadFile();

            if (tempskel == null)
            {
                return;
            }

            sList1 = new SkeletonList(tempskel);
            lblFrame1.Content = sList1.GetSkelCount().ToString();
            draw(sList1.GetCurrentSkel(), sList1, drawing1, Brushes.Black);
            playbackSlider1.Maximum = sList1.GetSkelCount();
            lblCurrentFrame1.Content = sList1.GetCurrentSkelIndex();

            btnSetEnd1.IsEnabled = true;
            btnStart1.IsEnabled = true;
            playbackSlider1.Value = 0;
            gridPlayback1.IsEnabled = true;
            playbackSlider1.IsEnabled = true;

        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (rewind)
            {
                playbackSlider.Value--;
            }
            else
            {
                playbackSlider.Value++;
            }
 
        }
        private void dispatchTimerStart(bool rewinds, int speed)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, speed);
        
            rewind = rewinds;
            dispatcherTimer.Start();
            
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
        private void dispatcherTimer1_Tick(object sender, EventArgs e)
        {
            if (rewind)
            {
                playbackSlider1.Value--;
            }
            else
            {
                playbackSlider1.Value++;
            }

        }
        private void dispatchTimerStart1(bool rewinds, int speed)
        {
            dispatcherTimer1.Tick += new EventHandler(dispatcherTimer1_Tick);
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, speed);

            rewind1 = rewinds;
            dispatcherTimer1.Start();

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart(false, 40);
        }

        KinectStarter kinect = new KinectStarter();

        private void btnKinect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnKinect.Content.ToString() == "Start Kinect")
                {
                    btnKinect.Content = "Stop Kinect";
                    kinect = new KinectStarter(KinectSensor.KinectSensors[0], imgColour, Surface);
                    kinect.StartSensor();
                    btnRecord.IsEnabled = true;

                }
                else
                {
                    btnKinect.Content = "Start Kinect";
                    kinect.StopKinect();
                    btnRecord.IsEnabled = false;
                }
            }
            catch
            {
                MessageBox.Show("You need to Connect a Kinect");
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            playbackSlider.Value = 0;
        }

        private void btnFF_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart(false, 1);
        }

        private void draw(Skeleton skel, SkeletonList skellist, Draw drawin, SolidColorBrush brush)
        {
            drawin.DrawFrame(skel, brush);
            lblCurrentFrame.Content = skellist.GetCurrentSkelIndex().ToString();
        }

        private void btnRew_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart(true, 40);
        }
        private void btnSRew_Click(object sender, RoutedEventArgs e)
        {
            playbackSlider.Value--;
        }
        private void btnSF_Click(object sender, RoutedEventArgs e)
        {
            playbackSlider.Value++;
        }

        private void playbackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (sList == null)
                {

                    return;
                }

                if (sList.isEnd())
                {
                    dispatcherTimer.Stop();
                }
                lblCurrentFrame.Content = playbackSlider.Value.ToString();
                sList.setCurrentSkel(Convert.ToInt32(playbackSlider.Value));
                draw(sList.GetCurrentSkel(), sList, drawing, Brushes.Black);
            }
            catch (IndexOutOfRangeException)
            {
                dispatcherTimer.Stop();
            }
        }

        private void math()
        {/*
            if (list1 > list2)
            {
                float temp = list1 / 100;
                float MultiplyPercent = list2 / temp / 100;
                double x = playbackSlider.Value * MultiplyPercent;
                int y = Convert.ToInt32(x);

                if (sList2.GetCurrentSkelIndex() != y)
                {
                    draw(sList2.NextSkel(), sList2, drawing2, Brushes.Blue);
                }

                draw(sList.NextSkel(), sList, drawing, Brushes.Black);

            }
          */ 
        }
        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Record)
            {
                kinect.StartRecord();
            }
            else
            {
                Record = false;
                kinect.StopRecord();
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {    
            if (sList.getStart() > sList.getEnd() && sList.getEnd() != 0)
            {
                MessageBox.Show("The start point needs to be before the end point.");
                return;
            }
            sList.setStart(Convert.ToInt32(playbackSlider.Value));
        }

        private void btnSetEnd_Click(object sender, RoutedEventArgs e)
        {
            if (sList.getEnd() < sList.getStart())
            {
                MessageBox.Show("End point needs to be later in time than the start point.");
                return;
            }
            sList.setEnd(Convert.ToInt32(playbackSlider.Value));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.motorDown(1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            kinect.motorUp(1);
        }

        private void btnStart1_Click(object sender, RoutedEventArgs e)
        {
            
            if (Convert.ToInt32(playbackSlider1.Value) > sList.getEnd() && sList.getEnd() != 0)
            {
                MessageBox.Show("The start point needs to be before the end point.");
                return;
            }
            sList.setStart(Convert.ToInt32(playbackSlider1.Value));
        }

        private void btnSetEnd1_Click(object sender, RoutedEventArgs e)
        {
            
            if (Convert.ToInt32(playbackSlider1.Value) < sList1.getStart())
            {
                MessageBox.Show("End point needs to be later in time than the start point.");
                return;
            }
            sList1.setEnd(Convert.ToInt32(playbackSlider1.Value));
        }

        private void btnRew1_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart1(true, 40);
        }

        private void btnPlay1_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart1(false, 40);
        }

        private void btnFF1_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart1(false, 5);
        }

        private void btnSF1_Click(object sender, RoutedEventArgs e)
        {
            playbackSlider1.Value++;
        }

        private void btnPause1_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer1.Stop();
        }

        private void btnSRew1_Click(object sender, RoutedEventArgs e)
        {
            playbackSlider1.Value--;
        }

        private void btnStop1_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer1.Stop();
            sList1.Reset();
        }

        private void playbackSlider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (sList1 == null)
                {

                    return;
                }

                if (sList1.isEnd())
                {
                    dispatcherTimer1.Stop();
                }
                lblCurrentFrame1.Content = playbackSlider1.Value.ToString();
                sList1.setCurrentSkel(Convert.ToInt32(playbackSlider1.Value));
                draw(sList1.GetCurrentSkel(), sList1, drawing1, Brushes.Black);
            }
            catch (IndexOutOfRangeException)
            {
                dispatcherTimer1.Stop();
            }
        }
    }
}
