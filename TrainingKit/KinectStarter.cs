using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace TrainingKit
{
    public class KinectStarter
    {
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        private int nearestId = -1;
        private Image image;
        private Canvas canvas;
        private KinectSensor sensor;
        private bool record = false;
        private List<Skeleton> skelrec = new List<Skeleton>{};

        public KinectStarter(KinectSensor sensor, Image image, Canvas canvas)
        {
            this.sensor = sensor;
            this.image = image;
            this.canvas = canvas;
        }

        public void motorUp(int value)
        {
            sensor.ElevationAngle += value;
        }
        public void motorDown(int value)
        {
            sensor.ElevationAngle -= value;
        }
        public void StartRecord()
        {
            record = true;
        }
        public void StopRecord()
        {
            record = false;
            SkelIO io = new SkelIO();
            io.writetofile(skelrec);
        }

        public void StopKinect()
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
        public KinectStarter()
        {
        }
        public void StartSensor()
        {
            if (sensor == null)
            {
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
            sensor.AllFramesReady += SensorAllFramesReady;

            try
            {
                sensor.Start();
            }
            catch
            {

            }
        }

        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Draw drawcolour = new Draw(image);
            Draw drawskel = new Draw(canvas);
            try
            {
                foreach (Skeleton skel in OnSkelFrameReady(sender, e))
                {
                    drawskel.DrawFrame(skel);
                }
                drawcolour.DrawFrame(ColorImageReady(sender, e));
            }
            catch
            {
                //do nothing?
            }
        
        }

        private Skeleton[] OnSkelFrameReady(object sender, AllFramesReadyEventArgs e)
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
                }
                catch
                {
                    return null;
                }
                var newNearistId = -1;
                var nearestDistance2 = double.MaxValue;

                foreach (var skeleton in allSkeletons)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (record)
                        {
                            skelrec.Add(skeleton);
                        }
                        var distance2 = (skeleton.Position.X * skeleton.Position.X) + (skeleton.Position.Y * skeleton.Position.Y) + (skeleton.Position.Z * skeleton.Position.Z);

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

                return allSkeletons;

            }
        }
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private byte[] pixelData;
        
        private WriteableBitmap ColorImageReady(object sender, AllFramesReadyEventArgs e)
        {
            WriteableBitmap CameraSource;
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

                        CameraSource = new WriteableBitmap(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                        CameraSource.WritePixels(
                        new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height),
                        this.pixelData,
                        imageFrame.Width * Bgr32BytesPerPixel,
                        0);
                        return CameraSource;
                    }

                }
            }
            return null;
        }
    }
}
