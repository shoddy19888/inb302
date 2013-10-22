using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace TrainingKit
{
    public class SkeletonList
    {
        //Variables
        private Skeleton[] SkelList;
        private int CurrentSkel;
        private int Start = 0, End = 0; 

     
        //Constructors
        public SkeletonList()
        {
        }
        public SkeletonList(Skeleton[] SkelList)
        {
            this.SkelList = SkelList;
        }

        //Functions
        public Skeleton GetCurrentSkel()
        {
            return SkelList[CurrentSkel];
        }
        public Skeleton NextSkel()
        {

            CurrentSkel++;
            return SkelList[CurrentSkel];
        }
        public Skeleton PreviousSkel()
        {
            CurrentSkel--;
            return SkelList[CurrentSkel];
        }
        public Skeleton GetSkelAtIndex(int Index)
        {
            return SkelList[Index];
        }
        public int GetSkelCount()
        {
            return SkelList.Count();
        }
        public Skeleton[] GetSkelList()
        {
            return SkelList;
        }
        public int GetCurrentSkelIndex()
        {
            return CurrentSkel;
        }
        public void Reset()
        {
            CurrentSkel = 0;
        }
        public void setCurrentSkel(int index)
        {
            CurrentSkel = index;
        }

        public bool isEnd()
        {
            if (CurrentSkel == (SkelList.Count()-1))
            {
                return true;
            }
            return false;
        }

        public void setStart(int start)
        {
            Start = start;
        }
        public void setEnd(int end)
        {
            End = end;
        }

        public int getStart()
        {
            return Start;
        }
        public int getEnd()
        {
            return End;
        }
       
    }
}
