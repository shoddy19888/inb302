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
        SkeletonList tempsList1, tempsList2;
        int currentFile = 1;
        int countForSkel= 1;

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

        }


        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            switch (currentFile)
            {
                case 1:
                    {
                        if (sList.isEnd())
                        {
                            dispatcherTimer.Stop();
                            return;
                        }
                        if (rewind)
                        {
                            playbackSlider.Value--;
                            draw(sList.PreviousSkel(), sList, drawing);
                        }
                        else
                        {
                            playbackSlider.Value++;
                            draw(sList.NextSkel(), sList, drawing);
                        }
                    } break;
                case 2:
                    {
                        if (sList2.isEnd())
                        {
                            dispatcherTimer.Stop();
                            return;
                        }
                        if (rewind)
                        {
                            playbackSlider.Value--;
                            draw(sList2.PreviousSkel(), sList2, drawing);
                        }
                        else
                        {
                            playbackSlider.Value++;
                            draw(sList2.NextSkel(), sList2, drawing);
                        }
                    } break;
                case 3:
                    {
                    } break;
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

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            playbackSlider.Value = 0;
            switch (currentFile)
            {
                case 1:
                    {
                        sList.Reset();
                        draw(sList.GetCurrentSkel(), sList, drawing);
                    } break;
                case 2:
                    {
                        sList2.Reset();
                        draw(sList2.GetCurrentSkel(), sList2, drawing);
                    } break;
                case 3:
                    {
                        sList.Reset();
                        sList2.Reset();
                        draw(sList.GetCurrentSkel(), sList, drawing);

                    } break;
            }
        }

        private void btnFF_Click(object sender, RoutedEventArgs e)
        {
            dispatchTimerStart(false, 1);
        }

        private void draw(Skeleton skel, SkeletonList skellist, Draw drawin)
        {
            
            if (skellist.GetCurrentSkelIndex() == sList.GetSkelCount())
            {
                dispatcherTimer.Stop();
                return;
            }
            if (skellist.GetCurrentSkelIndex() < 0)
            {
                dispatcherTimer.Stop();
                return;
            }
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
            switch (currentFile)
            {
                case 1:
                    {
                        draw(sList.PreviousSkel(), sList, drawing);
                    } break;
                case 2:
                    {
                        draw(sList2.PreviousSkel(), sList2, drawing);
                    } break;
                case 3:
                    {
                        if (sList.GetSkelCount() > sList2.GetSkelCount())
                        {
                            draw(sList.PreviousSkel(), sList, drawing);
                            
                            if (playbackSlider.Value / playbackSlider.Value == 0)
                            {
                                draw(sList2.PreviousSkel(), sList2, drawing);
                            }
                        }
                        else
                        {
                            draw(sList2.PreviousSkel(), sList2, drawing);

                            if (playbackSlider.Value / playbackSlider.Value == 0)
                            {
                                draw(sList.PreviousSkel(), sList, drawing);
                            }
                        }
                    } break;
            }
          
        }
        private void btnSF_Click(object sender, RoutedEventArgs e)
        {
            playbackSlider.Value++;
            
            switch (currentFile)
            {
                case 1:
                    {
                        draw(sList.NextSkel(), sList, drawing);
                    } break;
                case 2:
                    {
                        draw(sList2.NextSkel(), sList2, drawing);
                    } break;
                case 3:
                    {
                        if (sList.GetSkelCount() > sList2.GetSkelCount())
                        {
                            draw(sList.NextSkel(), sList, drawing);
                            
                            if (playbackSlider.Value / playbackSlider.Value == 0)
                            {
                                draw(sList2.NextSkel(), sList2, drawing);
                            }
                        }
                        else
                        {
                            draw(sList2.NextSkel(), sList2, drawing);

                            if (playbackSlider.Value / playbackSlider.Value == 0)
                            {
                                draw(sList.NextSkel(), sList, drawing);
                            }
                        }
                    } break;
            }
        }

        private void playbackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            switch (currentFile)
            { 
                case 1:
                    {
                        sList.setCurrentSkel(Convert.ToInt32(playbackSlider.Value)); 
                        draw(sList.GetCurrentSkel(), sList, drawing);
                    } break;
                case 2:
                    {
                        sList2.setCurrentSkel(Convert.ToInt32(playbackSlider.Value)); 
                        draw(sList2.GetCurrentSkel(), sList2, drawing);
                    } break;
                case 3:
                    {
                        if (sList.GetSkelCount() > sList2.GetSkelCount())
                        {
                            /*
int list1 = slist1.getSleton Num; 200
int list2 = slist1.getSletonNum; 300

if ( list1 < list2 ) {

	int x = 0;

	float MultiplyPercent = ((list1/(list2/100))/100);
	
	while(x < list2) {

		list1.drawSkeleton(Round( x  *MultiplyPercent ));

		list2.drawSkeleton(x);
		x++;

	}

}

if( list2 < list1) {

	int x = 0;

	float MultiplyPercent = ((list2/(list1/100))/100);

	while(x < list1) {

		list2.drawSkeleton(Round( x * MultiplyPercent ));
		list1.drawSkeleton( x );
		x++;

	}

}
　

*/
                        }
                        else
                        {
                        }
                        // here needs code to play both skeletons... 
                        //ie: draw(slist1.nextskel()
                        // if (frame is a draw frame) 
                        //{
                        // draw(slist2.nextskel()
                        //}
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
                            currentFile = 1;
                            if (sList == null)
                            {
                                btnStart.IsEnabled = false;
                                btnSetEnd.IsEnabled = false;
                                return;
                            }
                            
                            sList.Reset();
                            draw(sList.GetCurrentSkel(), sList, drawing);
                        } break;
                    case "File 2":
                        {
                            currentFile = 2;
                            if (sList2 == null)
                            {
                                btnStart.IsEnabled = false;
                                btnSetEnd.IsEnabled = false;
                                return;
                            }
                            
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
                            if (sList.GetSkelCount() > sList2.GetSkelCount())
                            {
                                playbackSlider.Maximum = Convert.ToDouble(sList.GetSkelCount());
                                countForSkel = sList.GetSkelCount() / sList2.GetSkelCount();
                            }
                            else
                            {
                                playbackSlider.Maximum = Convert.ToDouble(sList2.GetSkelCount());
                                countForSkel = sList2.GetSkelCount() / sList.GetSkelCount();
                            }
                            playbackSlider.Value = 0;
                            draw(sList.GetCurrentSkel(), sList, drawing);
                            draw(sList2.GetCurrentSkel(), sList2, drawing2);
                            
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
