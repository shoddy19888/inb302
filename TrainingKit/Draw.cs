using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TrainingKit

{
    class Draw
    {
        Canvas canvas;
        Image image;
        public Draw(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public Draw(Image image)
        {
            this.image = image;
        }

        public void DrawFrame(Skeleton skeleton)
        {
            DrawStickMan(skeleton, Brushes.Black, 7);
        }
        public void DrawFrame(Skeleton skeleton, SolidColorBrush brush)
        {
            DrawStickMan(skeleton, brush, 7);
        }

        public void DrawFrame(WriteableBitmap CameraSource)
        {
            image.Source = CameraSource;
        }

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

        private Point GetJointPoint(Skeleton skeleton, JointType jointType)
        {
            var joint = skeleton.Joints[jointType];

            var point = new Point
            {
                X = (canvas.Width / 2) + (canvas.Height * joint.Position.X / 3),
                Y = (canvas.Width / 2) - (canvas.Height * joint.Position.Y / 3)
            };

            return point;
        }

        private void DrawStickMan(Skeleton skeleton, Brush brush, int thickness)
        {
            canvas.Children.Clear();
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

                    canvas.Children.Add(line);
                }
            }
        }

    }
}
