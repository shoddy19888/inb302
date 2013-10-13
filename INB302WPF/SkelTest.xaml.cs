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
using SkelFileIO;
using Microsoft.Kinect;
using System.Threading;

namespace INB302WPF
{
    /// <summary>
    /// Interaction logic for SkelTest.xaml
    /// </summary>
    public partial class SkelTest : Window
    {
        Skeleton[] skellist;
        List<List<List<String>>> data;
        
        private static readonly JointType[][] SkeletonSegmentRuns = new JointType[][]
        {
            new JointType[] 
            { 
                JointType.Head, JointType.ShoulderCenter, JointType.HipCenter 
            },
            new JointType[] 
            { 
                JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft, JointType.ShoulderLeft,
                JointType.ShoulderCenter,
                JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight
            },
            new JointType[]
            {
                JointType.FootLeft, JointType.AnkleLeft, JointType.KneeLeft, JointType.HipLeft,
                JointType.HipCenter,
                JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight
            }
        };

        SkelFileIO.SkelFileIO io = new SkelFileIO.SkelFileIO();        
        public SkelTest()
        {
            
            InitializeComponent();
            

        }


        private void DrawStickMan(List<List<String>> skeleton, Brush brush, int thickness)
        {
            foreach (var run in SkeletonSegmentRuns)
            {
                var next = GetJointPoint(skeleton, run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = GetJointPoint(skeleton, run[i]);

                    var line = new System.Windows.Shapes.Line
                    {
                        Stroke = brush,
                        StrokeThickness = thickness,
                        X1 = prev.X,
                        Y1 = prev.Y,
                        X2 = next.X,
                        Y2 = next.Y,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    StickMen.Children.Add(line);
                    
                }
            }
        }


        private Point GetJointPoint(List<List<String>> skeleton, JointType jointType)
        {
            
            List<string> joint = new List<string>();
            foreach (List <string> s in skeleton)
            {
                if (s[0].ToString() == jointType.ToString())
                {
                    joint = s;
                    break;
                }
            }

            // Points are centered on the StickMen canvas and scaled according to its height allowing
            // approximately +/- 1.5m from center line.
            var point = new Point
            {
                X = (StickMen.Width / 2) + (StickMen.Height * double.Parse(joint[1]) / 3),
                Y = (StickMen.Width / 2) - (StickMen.Height * double.Parse(joint[2]) / 3)
            };

            return point;
        }
        int count = 0;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void test()
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,40);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (count == data.Count())
            {
                dispatcherTimer.Stop();
                return;
            }
            StickMen.Children.Clear();
           
                DrawStickMan(data[count], Brushes.Black, 7);

            
            count++;
        }
        System.Windows.Threading.DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
        private void test1()
        {
            dispatcherTimer1.Tick += new EventHandler(dispatcherTimer1_Tick);
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, 40);
            
            dispatcherTimer1.Start();
        }

        int count1 = 0;
        private void dispatcherTimer1_Tick(object sender, EventArgs e)
        {
            if (count1 == skellist.Count())
            {
                dispatcherTimer1.Stop();
                return;
            }
            StickMen.Children.Clear();
            
            DrawStickMan1(skellist[count1], Brushes.Black, 7);

            count1++;
        } 


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            data = io.ReadKinLog();
            test();
        }
      
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
            skellist = io.converttoskeleton(io.ReadKinLog());
            MessageBox.Show("Count of Frames to Play: " + skellist.Count().ToString());
            test1();

        }

        private Point GetJointPoint1(Skeleton skeleton, JointType jointType)
        {
            var joint = skeleton.Joints[jointType];

            var point = new Point
            {
                X = (StickMen.Width / 2) + (StickMen.Height * joint.Position.X / 3),
                Y = (StickMen.Width / 2) - (StickMen.Height * joint.Position.Y / 3)
            };

            return point;
        }

        private void DrawStickMan1(Skeleton skeleton, Brush brush, int thickness)
        {
            foreach (var run in SkeletonSegmentRuns)
            {
                var next = this.GetJointPoint1(skeleton, run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = this.GetJointPoint1(skeleton, run[i]);

                    var line = new System.Windows.Shapes.Line
                    {
                        Stroke = brush,
                        StrokeThickness = thickness,
                        X1 = prev.X,
                        Y1 = prev.Y,
                        X2 = next.X,
                        Y2 = next.Y,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    StickMen.Children.Add(line);
                }
            }
        }


    }
}
