using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace INB302WPF
{
    class KinectChangedEventArgs : EventArgs
    {
        public KinectChangedEventArgs(KinectSensor oldSensor, KinectSensor newSensor)
        {
            this.OldSensor = oldSensor;
            this.NewSensor = newSensor;
        }

        public KinectSensor OldSensor { get; private set; }

        public KinectSensor NewSensor { get; private set; }
    }
}
