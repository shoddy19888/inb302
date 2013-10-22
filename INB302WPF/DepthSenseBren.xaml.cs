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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using KinectMeasurementsLib;
using System.Windows.Media.Animation;
using System.Threading;
using SkelFileIO;

namespace INB302WPF
{
    /// <summary>
    /// Interaction logic for DepthSenseBren.xaml
    /// </summary>
    public partial class DepthSenseBren : Window
    {
       
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        List<Skeleton> SkelRec = new List<Skeleton>();
        SkelFileIO.SkelFileIO writer = new SkelFileIO.SkelFileIO();

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

        private DateTime highlightTime = DateTime.MinValue;
        private int highlightId = -1;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        Bitmap Depthbitmap;
        KinectSensor sensor;
        
        bool record = false;

        private int nearestId = -1;

        public DepthSenseBren()
        {
            InitializeComponent();
        }

        void Window_Loaded(object sender, EventArgs e)
        {
            lblRecord.Opacity = 0;
            
            if(KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("There are no Kinect Sensors Connected");
                //this.Close();
                return;
            }
            sensor = KinectSensor.KinectSensors[0];
            KinectChangedEventArgs f = new KinectChangedEventArgs(null,sensor);
            StartSensor(sender, f);
        }



        void StartSensor(object sender, KinectChangedEventArgs e)
        {
            var sensor = e.NewSensor;
            if (sensor == null)
            {
                MessageBox.Show("There is no sensor Connected");
                return;
            }
            
            var Skelparameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            sensor.SkeletonStream.Enable(Skelparameters);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            sensor.AllFramesReady += SensorAllFramesReady;

            try
            {
                sensor.Start();
                txtStatus.Text = "Kinect Started";
            }
            catch
            {
                txtStatus.Text = "Kinect Not Started";
            }
        }

        void OnSkelFrameReady(object sender, AllFramesReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    allSkeletons = new Skeleton[frame.SkeletonArrayLength];
                }

                try
                {
                    frame.CopySkeletonDataTo(allSkeletons);
                }catch
                {
                    return;
                }
                var newNearistId = -1;
                var nearestDistance2 = double.MaxValue;

                foreach (var skeleton in allSkeletons)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        var distance2 = (skeleton.Position.X * skeleton.Position.X) + (skeleton.Position.Y * skeleton.Position.Y) + (skeleton.Position.Z * skeleton.Position.Z);
                        Logger.Content = skeleton.AngleBetweenJoints(JointType.HandLeft, JointType.ElbowLeft, JointType.ShoulderLeft).ToString();

                        if (record)
                        {
                            SkelRec.Add(skeleton);
                        }
                        

                        txtXCoord.Text = (skeleton.Joints[JointType.HipCenter].Position.X * 100).ToString();
                        txtYCoord.Text = (skeleton.Joints[JointType.HipCenter].Position.X * 100).ToString();
                        txtZCoord.Text = (skeleton.Joints[JointType.HipCenter].Position.X * 100).ToString();

                        if (distance2 < nearestDistance2)
                        {
                            newNearistId = skeleton.TrackingId;
                            nearestDistance2 = distance2;
                        }
                        
                    }
                }

                if (this.nearestId != newNearistId)
                {
                    this.nearestId = newNearistId;
                }

                DrawStickMen(allSkeletons);

            }
        }

        void Window_Closed(object sender, EventArgs e)
        {
            StopKinect(sensor);
            try
            {
                
            }
            catch
            {
                //no file recorded
            }
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    sensor.AudioSource.Stop();
                }
            }
        }

        void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SensorDepthFrameReady(e);

            OnSkelFrameReady(sender, e);
            ColorImageReady(sender, e);

            video.Source = LoadBit(Depthbitmap);
            colourVid.Source = CameraSource;
        }

        void SensorDepthFrameReady(AllFramesReadyEventArgs e)
        {
            if (!this.WindowState.Equals(WindowState.Minimized))
            {
                using (var frame = e.OpenDepthImageFrame())
                {
                    Depthbitmap = CreateBitMapFromDepthFrame(frame);
                }
            }
        }



        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private byte[] pixelData;
        private WriteableBitmap CameraSource;
        
        private void ColorImageReady(object sender, AllFramesReadyEventArgs e)
        {
            bool receivedData = false;
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {

                    if (pixelData == null)
                    {
                        this.pixelData = new byte[imageFrame.PixelDataLength];
                    }

                    imageFrame.CopyPixelDataTo(this.pixelData);
                    receivedData = true;

                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    if (receivedData)
                    {
                        
                        this.CameraSource = new WriteableBitmap(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                        this.CameraSource.WritePixels(
                        new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height),
                        this.pixelData,
                        imageFrame.Width * Bgr32BytesPerPixel,
                        0);

                    }

                }
            }
        }



        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        BitmapSource LoadBit(System.Drawing.Bitmap source)
        {

            if (source != null)
            {

                IntPtr ip = source.GetHbitmap();
                BitmapSource bs = null;

                try
                {
                    bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    DeleteObject(ip);
                }
                return bs;
            }
            
            return null;
            
                  
        }

        Bitmap CreateBitMapFromDepthFrame(DepthImageFrame frame)
        {
            if (frame != null)
            {
                var bitmapImage = new Bitmap(frame.Width, frame.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
                var g = Graphics.FromImage(bitmapImage);
                g.Clear(System.Drawing.Color.FromArgb(0, 34, 68));

                var _pixelData = new short[frame.PixelDataLength];
                frame.CopyPixelDataTo(_pixelData);
                BitmapData bmapdata = bitmapImage.LockBits(new System.Drawing.Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.WriteOnly, bitmapImage.PixelFormat);

                IntPtr ptr = bmapdata.Scan0;
                Marshal.Copy(_pixelData, 0, ptr, frame.Width * frame.Height);
                bitmapImage.UnlockBits(bmapdata);

                return bitmapImage;
                
            }
            return null;
        }


        private System.Windows.Point GetJointPoint(Skeleton skel, JointType jointType)
        {
            var joint = skel.Joints[jointType];

            return new System.Windows.Point
            {
                X = (StickMen.Width / 2) + (StickMen.Height * joint.Position.X / 3),
                Y = (StickMen.Width / 2) - (StickMen.Height * joint.Position.Y / 3)
            };
            
        }

        private void DrawStickMan(Skeleton skeleton, System.Windows.Media.Brush brush, int thickness)
        {
            foreach (var run in SkeletonSegmentRuns)
            {
                var next = this.GetJointPoint(skeleton, run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = this.GetJointPoint(skeleton, run[i]);

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

        private void DrawStickMen(Skeleton[] skeletons)
        {
            // Remove any previous skeletons.
            StickMen.Children.Clear();

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Draw a background for the next pass.
                    this.DrawStickMan(skeleton, System.Windows.Media.Brushes.WhiteSmoke, 7);
                }
            }

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    var brush = DateTime.UtcNow < this.highlightTime && skeleton.TrackingId == this.highlightId ? System.Windows.Media.Brushes.Red :
                        skeleton.TrackingId == this.nearestId ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.Gray;

                    // Draw the individual skeleton.
                    this.DrawStickMan(skeleton, brush, 3);
                }
            }
        }

        void flash()
        {
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));        
            lblRecord.BeginAnimation(Label.OpacityProperty, ani);
            ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            lblRecord.BeginAnimation(Label.OpacityProperty, ani);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (lblRecord.Opacity == 100)
            {
                lblRecord.Opacity = 0;
            }
            else lblRecord.Opacity = 100;
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Label l = lblRecord;

            
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
           

            if (!record)
            {
                record = true;

                try
                {
                    
                }
                catch
                {
                    MessageBox.Show("Continue recording in the same file?");
                }

                
                lblRecord.Opacity = 100;
                dispatcherTimer.Start();
            }
            else
            {
                dispatcherTimer.Stop();
                lblRecord.Opacity = 0;
                record = false;
                writer.writetofile(SkelRec);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sensor.ElevationAngle -= 10;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            sensor.ElevationAngle += 10;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            StopKinect(sensor);
        }

        

        

    }
}
