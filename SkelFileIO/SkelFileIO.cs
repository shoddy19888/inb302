using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Forms;

namespace SkelFileIO
{
    public class SkelFileIO
    {
        public void writetofile(List<Skeleton> skellist)
        {

            List<string> data = new List<string>();

            foreach (Skeleton skeleton in skellist)
            {
                data.Add("$;");

                foreach (Joint joint in skeleton.Joints)
                {
                    data.Add(joint.JointType.ToString() + "," + joint.Position.X.ToString() + "," + joint.Position.Y.ToString() + "," + joint.Position.Z.ToString() + ";");
                }
            }

            System.IO.File.WriteAllLines(DialogBoxDisplay(true), data);

        }

        public string DialogBoxDisplay(bool save)
        {
            if (save)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".kinlog"; // Default file extension
                dlg.Filter = "KinLog documents |*.kinlog"; // Filter files by extension

                // Show save file dialog box
                DialogResult result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == DialogResult.OK)
                {
                    // Save document
                    return dlg.FileName;
                }
                else return null;
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".kinlog"; // Default file extension
                dlg.Filter = "KinLog documents |*.kinlog"; // Filter files by extension

                // Show save file dialog box
                //Nullable<bool> result = dlg.ShowDialog();
                DialogResult result = dlg.ShowDialog();
                // Process save file dialog box results
                if (result == DialogResult.OK)
                {
                    // Save document
                    return dlg.FileName;
                }
                else return null;
            }
        }

        public List<List<List<string>>> ReadKinLog()
        {
            System.IO.StreamReader file;
            try
            {
                file = new System.IO.StreamReader(DialogBoxDisplay(false));
            }
            catch
            {
                return null;
            }
            List<string> readfile = new List<string>();
            while (!file.EndOfStream)
            {
                readfile.Add(file.ReadLine());
            }

            bool isFrameNum = false;

            StringBuilder sb = new StringBuilder();
            List<string> data;
            List<List<string>> ListData = new List<List<string>>();
            List<List<List<String>>> Framelist = new List<List<List<string>>>();

            foreach (string s in readfile)
            {


                data = new List<string>();
                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '$':
                            {
                                isFrameNum = true;
                                break;
                            }
                        case ',':
                            {
                                data.Add(sb.ToString());
                                sb.Clear();
                            } break;
                        case ';':
                            {
                                if (isFrameNum)
                                {

                                    sb.Clear();
                                    isFrameNum = false;
                                    if (ListData.Count != 0)
                                    {
                                        Framelist.Add(ListData);
                                        ListData = new List<List<string>>();
                                    }
                                    break;
                                }
                                data.Add(sb.ToString());
                                sb.Clear();
                                ListData.Add(data);

                                data = new List<string>();
                            } break;
                        default: sb.Append(c); break;
                    }

                }
            }

            return Framelist;
        }
        public Skeleton[] converttoskeleton(List<List<List<string>>> data)
        {
            Skeleton[] skellist;
            int skelnum = 0;
            try
            {
                skellist = new Skeleton[data.Count];
            }
            catch
            {
                MessageBox.Show("You need to select a file to play a file.");
                return null;
            }
            string current = "";
            foreach (List<List<string>> frame in data)
            {
                Skeleton skel = new Skeleton();
                foreach (List<string> joint in frame)
                {

                    var pos = new SkeletonPoint();
                    pos.X = float.Parse(joint[1]);
                    pos.Y = float.Parse(joint[2]);
                    pos.Z = float.Parse(joint[3]);


                    current = joint[0];
                    Joint sjoint = new Joint();
                    sjoint.Position = pos;



                    switch (current)
                    {
                        case "HipCenter":
                            {
                                var testJoint = skel.Joints[JointType.HipCenter];
                                testJoint.Position = pos;
                                skel.Joints[JointType.HipCenter] = testJoint;
                            } break;
                        case "Spine":
                            {
                                var testJoint = skel.Joints[JointType.Spine];
                                testJoint.Position = pos;
                                skel.Joints[JointType.Spine] = testJoint;
                            } break;
                        case "ShoulderCenter":
                            {
                                var testJoint = skel.Joints[JointType.ShoulderCenter];
                                testJoint.Position = pos;
                                skel.Joints[JointType.ShoulderCenter] = testJoint;
                            }; break;
                        case "Head":
                            {
                                var testJoint = skel.Joints[JointType.Head];
                                testJoint.Position = pos;
                                skel.Joints[JointType.Head] = testJoint;
                            }; break;
                        case "ShoulderLeft":
                            {
                                var testJoint = skel.Joints[JointType.ShoulderLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.ShoulderLeft] = testJoint;
                            }; break;
                        case "ElbowLeft":
                            {
                                var testJoint = skel.Joints[JointType.ElbowLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.ElbowLeft] = testJoint;
                            }; break;
                        case "WristLeft":
                            {
                                var testJoint = skel.Joints[JointType.WristLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.WristLeft] = testJoint;
                            }; break;
                        case "HandLeft":
                            {
                                var testJoint = skel.Joints[JointType.HandLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.HandLeft] = testJoint;
                            }; break;
                        case "ShoulderRight":
                            {
                                var testJoint = skel.Joints[JointType.ShoulderRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.ShoulderRight] = testJoint;
                            }; break;
                        case "ElbowRight":
                            {
                                var testJoint = skel.Joints[JointType.ElbowRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.ElbowRight] = testJoint;
                            }; break;
                        case "WristRight":
                            {
                                var testJoint = skel.Joints[JointType.WristRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.WristRight] = testJoint;
                            }; break;
                        case "HandRight":
                            {
                                var testJoint = skel.Joints[JointType.HandRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.HandRight] = testJoint;
                            }; break;
                        case "HipLeft":
                            {
                                var testJoint = skel.Joints[JointType.HipLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.HipLeft] = testJoint;
                            }; break;
                        case "KneeLeft":
                            {
                                var testJoint = skel.Joints[JointType.KneeLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.KneeLeft] = testJoint;
                            }; break;
                        case "AnkleLeft":
                            {
                                var testJoint = skel.Joints[JointType.AnkleLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.AnkleLeft] = testJoint;
                            }; break;
                        case "FootLeft":
                            {
                                var testJoint = skel.Joints[JointType.FootLeft];
                                testJoint.Position = pos;
                                skel.Joints[JointType.FootLeft] = testJoint;
                            }; break;
                        case "HipRight":
                            {
                                var testJoint = skel.Joints[JointType.HipRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.HipRight] = testJoint;
                            }; break;
                        case "KneeRight":
                            {
                                var testJoint = skel.Joints[JointType.KneeRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.KneeRight] = testJoint;
                            }; break;
                        case "AnkleRight":
                            {
                                var testJoint = skel.Joints[JointType.AnkleRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.AnkleRight] = testJoint;
                            }; break;
                        case "FootRight":
                            {
                                var testJoint = skel.Joints[JointType.FootRight];
                                testJoint.Position = pos;
                                skel.Joints[JointType.FootRight] = testJoint;
                            }; break;
                    }

                }

                skellist[skelnum] = skel;

                skelnum++;

            }


            return skellist;

        }

    }


}
