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

        bool Record =false;
        Draw drawing;
        Draw drawing2;
        SkelIO io;
        SkeletonList sList;
        SkeletonList sList2;
        
        int currentFile = 1;


        public MainWindow()
        {
            InitializeComponent();
            gridPlayback.IsEnabled = false;
            drawing = new Draw(Surface);
            io = new SkelIO();
            btnRecord.IsEnabled = false;
            drawing2 = new Draw(Surface2);
            btnStart.IsEnabled = false;
            btnSetEnd.IsEnabled = false;
            cmbFile.Items.Add("File 1");
            cmbFile.Items.Add("File 2");
            cmbFile.Items.Add("Both");
            cmbFile.Text = "File 1";
            playbackSlider.IsEnabled = false;
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            var tempskel = io.ReadFile();

            if (tempskel == null)
            {
                return;
            }

            switch (currentFile)
            {
                case 1:
                    {

                        sList = new SkeletonList(tempskel);
                        lblFrame.Content = sList.GetSkelCount().ToString();
                        drawing.DrawFrame(sList.GetCurrentSkel());
                        playbackSlider.Maximum = sList.GetSkelCount();
                        lblCurrentFrame.Content = sList.GetCurrentSkelIndex();
                    } break;
                case 2:
                    {
                        sList2 = new SkeletonList(tempskel);
                        lblFrame.Content = sList2.GetSkelCount().ToString();
                        drawing.DrawFrame(sList2.GetCurrentSkel());
                        playbackSlider.Maximum = sList2.GetSkelCount();
                        lblCurrentFrame.Content = sList2.GetCurrentSkelIndex();
                    } break;
                case 3:
                    {
                        MessageBox.Show("You need to select either File 1 or File 2");
                    } break;
            }


            btnSetEnd.IsEnabled = true;
            btnStart.IsEnabled = true;
            playbackSlider.Value = 0;
            gridPlayback.IsEnabled = true;
            playbackSlider.IsEnabled = true;

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

        private void draw(Skeleton skel, SkeletonList skellist, Draw drawin)
        {
            drawin.DrawFrame(skel);
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
            
            switch (currentFile)
            { 
                case 1:
                    {
                        if (sList == null)
                        {
                            
                            return;
                        }

                        if (sList.isEnd())
                        {
                            dispatcherTimer.Stop();
                        }
                        sList.setCurrentSkel(Convert.ToInt32(playbackSlider.Value)); 
                        draw(sList.GetCurrentSkel(), sList, drawing);
                    } break;
                case 2:
                    {
                        if (sList2 == null)
                        {
                            return;
                        }
                        if (sList2.isEnd())
                        {
                            dispatcherTimer.Stop();
                        }
                        sList2.setCurrentSkel(Convert.ToInt32(playbackSlider.Value)); 
                        draw(sList2.GetCurrentSkel(), sList2, drawing);
                    } break;
                case 3:
                    {
                        
                        if(sList.isEnd())
                        {
                            dispatcherTimer.Stop();
                            return;
                        }
                        if (sList2.isEnd())
                        {
                            dispatcherTimer.Stop();
                            return;
                        }
                         
                            int list1 = sList.GetSkelCount(); 
                            int list2 = sList2.GetSkelCount();

                            if ( list1 < list2 ) 
                            {
                                
                                float temp = list2 / 100;
                                float MultiplyPercent = list1 / temp / 100;
                                double x = playbackSlider.Value * MultiplyPercent;
                                int y = Convert.ToInt32(x);
                                
                                if (sList.GetCurrentSkelIndex() != y)
                                {
                                    draw(sList.NextSkel(),sList,drawing);
                                }

	                        	draw(sList2.NextSkel(),sList2, drawing2);
	                        	

	                            

                            }
                            if (list1 > list2)
                            {
                                float temp = list1 / 100;
                                float MultiplyPercent = list2 / temp / 100;
                                double x = playbackSlider.Value * MultiplyPercent;
                                int y = Convert.ToInt32(x);
                                
                                if (sList2.GetCurrentSkelIndex() != y)
                                {
                                    draw(sList2.NextSkel(), sList2, drawing2);
                                }

                                draw(sList.NextSkel(), sList, drawing);




                            }
    
　                    } break;
            }
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            switch (currentFile)
            {
                case 1:
                    {
                        sList.setStart(Convert.ToInt32(playbackSlider.Value));
                        if (sList.getStart() > sList.getEnd() && sList.getEnd() != 0)
                        {
                            MessageBox.Show("The start point needs to be before the end point.");
                            return;
                        }
                        
                    } break;
                case 2:
                    {
                        sList2.setStart(Convert.ToInt32(playbackSlider.Value));
                        if (sList2.getStart() > sList2.getEnd() && sList2.getEnd() != 0)
                        {
                            MessageBox.Show("The start point needs to be before the end point.");
                            return;
                        }
                        
                    } break;
                case 3:
                    {
                        MessageBox.Show("You need to use either File 1 or File 2");
                    } break;
            }
        }

        private void btnSetEnd_Click(object sender, RoutedEventArgs e)
        {
            switch (currentFile)
            {
                case 1:
                    {
                        sList.setEnd(Convert.ToInt32(playbackSlider.Value));
                        if (sList.getEnd() < sList.getStart())
                        {
                            MessageBox.Show("End point needs to be later in time than the start point.");
                            return;
                        }
                        
                    } break;
                case 2:
                    {
                        sList2.setEnd(Convert.ToInt32(playbackSlider.Value));
                        if (sList2.getEnd() < sList2.getStart())
                        {
                            MessageBox.Show("End point needs to be later in time than the start point.");
                            return;
                        }
                        
                    } break;
                case 3:
                    {
                        MessageBox.Show("You need to use either File 1 or File 2");
                    } break;
            }
        }

        private void btnSetFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (cmbFile.Text)
                {
                    case "File 1":
                        {
                            playbackSlider.Value = 0;
                            currentFile = 1;
                            if (sList == null)
                            {
                                btnStart.IsEnabled = false;
                                btnSetEnd.IsEnabled = false;
                                playbackSlider.IsEnabled = false;
                                return;
                            }
                            playbackSlider.IsEnabled = true;
                            sList.Reset();
                            draw(sList.GetCurrentSkel(), sList, drawing);
                        } break;
                    case "File 2":
                        {
                            currentFile = 2;

                            playbackSlider.Value = 0;
                            
                            if (sList2 == null)
                            {
                                btnStart.IsEnabled = false;
                                btnSetEnd.IsEnabled = false;
                                playbackSlider.IsEnabled = false;
                                return;

                            }
                            playbackSlider.IsEnabled = true;
                            sList2.Reset();
                            draw(sList2.GetCurrentSkel(), sList2, drawing);
                        } break;
                    case "Both":
                        {
                            currentFile = 3;
                            if (sList == null)
                            {
                                MessageBox.Show("You need import file 1");
                                return;
                            }
                            if (sList2 == null)
                            {
                                MessageBox.Show("You need to import file 2");
                                return;
                            }
                            if (sList.getEnd() == 0 || sList.getStart() == 0 || sList2.getEnd() == 0 || sList2.getStart() == 0)
                            {
                                MessageBox.Show("Make sure you have set the start and end on each file.");
                                return;
                            }

                            
                            sList.actNewSkellist();
                            sList2.actNewSkellist();

                            
                            sList.setCurrentSkel(0);
                            sList2.setCurrentSkel(0);
                            playbackSlider.Value = 0;
                            
                            if (sList.GetSkelCount() > sList2.GetSkelCount())
                            {
                                playbackSlider.Maximum = Convert.ToDouble(sList.GetSkelCount());
                            }
                            else
                            {
                                playbackSlider.Maximum = Convert.ToDouble(sList2.GetSkelCount());
                            }

                            
                        } break;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }
        }
    }
}
