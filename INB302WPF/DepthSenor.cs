﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Kinect;

namespace INB302WPF
{
    public partial class DepthSenor : Form
    {
        private KinectSensorChooser _chooser;
        private Bitmap _bitmap;

        public DepthSenor()
        {
            InitializeComponent();
        }

        private void DepthSenor_Load(object sender, EventArgs e)
        {
            _chooser = new KinectSensorChooser();
            _chooser.KinectChanged += ChooserSensorChanged;
            _chooser.Start();
        }
        void ChooserSensorChanged(object sender, KinectChangedEventArgs e)
        {
            var old = e.OldSensor;
            StopKinect(old);

            var newsensor = e.NewSensor;
            if (newsensor == null)
            {
                return;
            }

            newsensor.SkeletonStream.Enable();
            newsensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            newsensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            newsensor.AllFramesReady += SensorAllFramesReady;

            try
            {
                newsensor.Start();
                rtbMessages.Text = "Kinect Started" + "\r";
            }
            catch (System.IO.IOException)
            {
                rtbMessages.Text = "Kinect Not Started" + "\r";
                //maybe another app is using Kinect
                _chooser.TryResolveConflict();
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
            video.Image = _bitmap;
        }


        void SensorDepthFrameReady(AllFramesReadyEventArgs e)
        {
            // if the window is displayed, show the depth buffer image
            if (WindowState != FormWindowState.Minimized)
            {
                using (var frame = e.OpenDepthImageFrame())
                {
                    _bitmap = CreateBitMapFromDepthFrame(frame);
                }
            }
        }


        private Bitmap CreateBitMapFromDepthFrame(DepthImageFrame frame)
        {
            if (frame != null)
            {
                var bitmapImage = new Bitmap(frame.Width, frame.Height, PixelFormat.Format16bppRgb565);
                var g = Graphics.FromImage(bitmapImage);
                g.Clear(Color.FromArgb(0, 34, 68));

                //Copy the depth frame data onto the bitmap 
                var _pixelData = new short[frame.PixelDataLength];
                frame.CopyPixelDataTo(_pixelData);
                BitmapData bmapdata = bitmapImage.LockBits(new Rectangle(0, 0, frame.Width,
                 frame.Height), ImageLockMode.WriteOnly, bitmapImage.PixelFormat);
                IntPtr ptr = bmapdata.Scan0;
                Marshal.Copy(_pixelData, 0, ptr, frame.Width * frame.Height);
                bitmapImage.UnlockBits(bmapdata);

                return bitmapImage;
            }
            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _chooser.Kinect.ElevationAngle = _chooser.Kinect.ElevationAngle + int.Parse(txtAngle.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("You are tilting it too quickly");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                _chooser.Kinect.ElevationAngle -= int.Parse(txtAngle.Text);
            }
            catch (FormatException f)
            {
                MessageBox.Show(f.Message);
            }

            catch (Exception)
            {
                MessageBox.Show("You are tilting it to quickly");
            }
        }
    }
}
