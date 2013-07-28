//Every OpenTK game will contain 4 basic methods:

//OnLoad: this is the place to load resources from disk, like images or music.
//OnUpdateFrame: this is a suitable place to handle input, update object positions,run physics or AI calculations.

//OnRenderFrame: this contains the code that renders your graphics. It typically begins with a call to GL.Clear() and ends with a call to SwapBuffers.

//OnResize: this method is called automatically whenever your game window changes size. Fullscreen applications will typically call it only once.
//Windowed applications may call it more often. In most circumstances, you can simply copy & paste the code from Game.cs.

/*
 * 
 * 
 * This version is the one which copies the robot exactly - that is when the robot angles are given it gives the same result
 * 
 * 
 * 
 */
using System;


using System.Threading;
using System.Diagnostics;


using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;


using Aldebaran.Proxies;
using Microsoft.Kinect;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
namespace Bot
{
    class Program:GameWindow
    {


        Stopwatch stR, stL, total_time;
        public const int d = -1000;
        float x, y;
        //Point target, target1;
        Stickman drawingRobot, compRobot;


        Point targetHandRight, targetHandLeft, targetElbowRight, targetElbowLeft;

        //This is for the Nao robot
        float speed = 0.2f;
        MotionProxy mt;


        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        KinectSensor sensor;

        private Skeleton[] skeletonData;

        Thread timerKinect;

        double ArmLength, botArmLength;
        int i = new int();

        ///<summary>
        ///The main entry point from the application
        ///</summary>
        ///
        [STAThread]
        static void Main()
        {
            using (Program a = new Program())
            {
                a.Run(30.0);
            }

        }



        /// <summary>
        /// Creates a window with the spedified size and title
        /// </summary>
        /// <param name="args"></param>
        public Program()
        :base(800,600, GraphicsMode.Default, "CSharp Bot")
        {
            VSync = VSyncMode.On;
            x = 10;
            y = 10;

           ConnectNao();  
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);//black background

            GL.Enable(EnableCap.DepthTest);


            stR = new Stopwatch();

            stL = new Stopwatch();
            total_time = new Stopwatch();


            drawingRobot = new Stickman();

            compRobot = new Stickman();

            //Kinect's Thread starts

            timerKinect = new Thread(ConnectKinect);
            timerKinect.Start();

        }
        /* *********************************************
        //
        //  Methods Related to Nao Connection
        // 
        ************************************************ */

        /// <summary>
        /// Methods for updating NAO positions
        /// </summary>
        void ConnectNao()
        {
            string IP = "10.104.12.47";
            int port = 9559;

            mt = new MotionProxy(IP, port);

            mt.setStiffnesses("Body", 0.8f);
            //The motion of these hands will indicate the robot's connection 
            mt.setAngles("RWristYaw", (float)1.5621, speed);
            mt.setAngles("LWristYaw", (float)-1.5621, speed);
            Console.WriteLine("NAO Connected");
        
        }
        void updateJointPosition()
        {
            Console.WriteLine("Moving Joints");
            
            mt.setAngles("RElbowRoll", (float)drawingRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle, speed);
            mt.setAngles("LElbowRoll", (float)drawingRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle, speed);

          
            //pitch after that
            if ((float)drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle > (float)drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.max)
            {
                drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.max;
            }

            if ((float)drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle > (float)drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.max)
            {
                drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.max;
            }

            if ((float)drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle < (float)drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.min)
            {
                drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.min;
            }

            if ((float)drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle > (float)drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.min)
            {
                drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.min;
            }

            
            mt.setAngles("RElbowYaw", (float)drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle, speed);
            mt.setAngles("LElbowYaw", (float)drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle, speed);


            mt.setAngles("RShoulderRoll", (float)drawingRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle, speed);
            mt.setAngles("LShoulderRoll", (float)drawingRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle, speed);
            
            mt.setAngles("RShoulderPitch", (float)drawingRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle, speed);
            mt.setAngles("LShoulderPitch", (float)drawingRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle, speed);

        }
        /// <summary>
        ///  this is the place to load resources from disk, like images or music.
        /// Load resouces here 
        /// </summary>
        /// 
     
         /* *********************************************
        //
        //  Methods Related to kinect connecting and update
        // 
        ************************************************ */
        void ConnectKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            if (null != this.sensor)
            {
                //Turn on the skeleton stream to receive skeleton 
                this.sensor.SkeletonStream.Enable(new TransformSmoothParameters() { Correction = 0.5f, JitterRadius = 0.05f, MaxDeviationRadius = 0.05f, Prediction = 0.5f, Smoothing = 0.5f });
                //this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                skeletonData = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                //start the sensor
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                    Console.WriteLine("Kinect not Connected");
                    
                }
            }
        }

        

        //This method is called whenever there is a frame update by the kinect
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            //This which allows the user to do his calculation before motiving around 
            //We will play with this timer if we needed more time for moving the joints


            Thread.Sleep(200);
            total_time.Start();




            if (true)
            {
                Skeleton[] skeletons = new Skeleton[0];
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (skeletonFrame != null)
                    {
                        skeletonFrame.CopySkeletonDataTo(skeletonData);
                        skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];

                        if (skeletons == null)
                            return;

                        skeletonFrame.CopySkeletonDataTo(skeletons);

                    }
                }

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (skel == null)
                                return;
                          
                            Point TorsoPoint = new Point();
                            TorsoPoint = getPoint(skel.Joints[JointType.Spine]);
                            //Console.WriteLine("My Spine is at: X = {0}, Y = {1}, and Z = {2}", TorsoPoint.x, TorsoPoint.y, TorsoPoint.z);
                            Point SRightPoint = new Point();
                            SRightPoint = getPoint(skel.Joints[JointType.ShoulderRight]);
                            Point SLeftPoint = new Point();
                            SLeftPoint = getPoint(skel.Joints[JointType.ShoulderLeft]);
                            Point ERightPoint = new Point();
                            ERightPoint = getPoint(skel.Joints[JointType.ElbowRight]);
                            Point ELeftPoint = new Point();
                            ELeftPoint = getPoint(skel.Joints[JointType.ElbowLeft]);
                            Point HRightPoint = new Point();
                            HRightPoint = getPoint(skel.Joints[JointType.HandRight]);
                            Point HLeftPoint = new Point();
                            HLeftPoint = getPoint(skel.Joints[JointType.HandLeft]);

                            if (i == 0)
                            {
                                ArmLength = (getDistance(SRightPoint, HRightPoint) +
                                 getDistance(SLeftPoint, HLeftPoint)) /2;
                                
                                i++;
                            }

                            targetElbowRight = getNaoSpace(changeBasis(ERightPoint, TorsoPoint), ArmLength);
                            targetElbowLeft = getNaoSpace(changeBasis(ELeftPoint, TorsoPoint), ArmLength);
                            targetHandRight = getNaoSpace(changeBasis(HRightPoint, TorsoPoint), ArmLength);
                            targetHandLeft = getNaoSpace(changeBasis(HLeftPoint, TorsoPoint), ArmLength);
                            //swapJoints();
                           // Point temphand = targetHandRight;
                           // targetHandRight = targetHandLeft;
                            targetHandRight.x = -targetHandRight.x;
                            targetElbowRight.x = -targetElbowRight.x;
                           // targetHandLeft = temphand;

                           // Point tempElbow = targetElbowRight;
                           // targetHandRight = targetHandLeft;
                            targetElbowLeft.x = -targetElbowLeft.x;
                            targetHandLeft.x = -targetHandLeft.x;

                            findTargetLeft(targetHandLeft, targetElbowLeft, (int)dots.LEFT_HAND);
                            findTargetRight(targetHandRight, targetElbowRight, (int)dots.RIGHT_HAND);

                            drawingRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle;
                            drawingRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle = compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle;
                            drawingRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle = compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle;
                            drawingRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle = compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle;

                            drawingRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle;
                            drawingRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle = compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle;
                            drawingRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle = compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle;
                            drawingRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle = compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle;

                           updateJointPosition();

                            //}
                            total_time.Stop();
                            //   Console.WriteLine("The total_time Elapsed  = {0}", total_time.ElapsedMilliseconds);
                        }

                    }
                }
            }
        }
                     /* *********************************************
                     //
                    //  Method to convert the Kinect Space to Nao Space
                   // 
                     ************************************************ */
                     
                  Point getNaoSpace(Point KinectPoint, double UserArmLenth)
                  {
                    Point NaoPoint = new Point();

                    //We need the arm lenth ratio between the user's arm lenth and the Nao's arm length
                                        
                    double NAOarmLenth = getDistance(compRobot.jointers[(int)dots.RIGHT_SHOULDER].p, compRobot.jointers[(int)dots.RIGHT_ELBOW].p) 
                        + getDistance(compRobot.jointers[(int)dots.RIGHT_ELBOW].p, compRobot.jointers[(int)dots.RIGHT_HAND].p);
                    double ratio = UserArmLenth / NAOarmLenth;
    
                  //  Console.WriteLine("The length of Nao's arm is {0}", NAOarmLenth);
                    NaoPoint.x = KinectPoint.x / ratio;
                    NaoPoint.y = KinectPoint.y / ratio;
                    NaoPoint.z = KinectPoint.z / ratio;
                    
                    return NaoPoint;

                }

                Point getPoint(Joint Jnt)
                {
                    Point pnt = new Point();

                    pnt.x = Jnt.Position.X;
                    pnt.y = Jnt.Position.Y;
                    pnt.z = Jnt.Position.Z;

                    return pnt;
                }

                Point changeBasis(Point pntA, Point Torso)
                {
                    Point pnt = new Point();

                    pnt.x = pntA.x - Torso.x;
                    pnt.y = pntA.y - Torso.y;
                    pnt.z = -(pntA.z - Torso.z);

                    return pnt;

                }

                /// <summary>
                /// Helper method to mirror the skeleton before calculating joint angles for the avatar.
                /// </summary>
                /// <param name="skeleton">The skeleton to mirror.</param>
                /// 
                int beforeMirror = 0;
                int afterMirror = 0;
              
        /// <summary>
        /// this method is called automatically whenever your game window changes size.
        /// Called when your window is resized. Set your viewport here. It is also a good place to set your projection matrix(
        /// which probably changes along when the aspect ratio of your windwo).
        /// </summary>
        /// 
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height); // Use all of the glControl painting area

            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)ClientRectangle.Width / ClientRectangle.Height, 2.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadIdentity();
            GL.Ortho(0, ClientRectangle.Width, 0, ClientRectangle.Height, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)

            GL.MatrixMode(MatrixMode.Modelview);

            
        }


        /// <summary>
        /// a suitable place to handle input, update object positions, run physics or AI calculations.
        /// Called when it is time to setup the next frame. Add you game logic here
        /// </summary>
        /// <param name="e">Contains timing information of framerate independet logic</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.Escape])
                Exit();
            if (Keyboard[Key.Up])
            {
                y += 10;
                drawingRobot.jointers[(int)dots.HEART].pitch.angle += 0.1;
                Console.WriteLine("{0}", drawingRobot.jointers[(int)dots.HEART].pitch.angle);
            }
            if (Keyboard[Key.Down])
            {
                y -= 10;
                drawingRobot.jointers[(int)dots.HEART].pitch.angle -= 0.1;
                Console.WriteLine("{0}", drawingRobot.jointers[(int)dots.HEART].pitch.angle);
            }
            if (Keyboard[Key.Left])
            {
                x -= 10;

                drawingRobot.jointers[(int)dots.HEART].yaw.angle += 0.1;
                Console.WriteLine("{0}", drawingRobot.jointers[(int)dots.HEART].yaw.angle);
            }
            if (Keyboard[Key.Right])
            {
                x += 10;
                drawingRobot.jointers[(int)dots.HEART].yaw.angle -= 0.1;
                Console.WriteLine("{0}", drawingRobot.jointers[(int)dots.HEART].yaw.angle);
            }

                
        }

        /// <summary>
        /// this contains the code that renders your graphics. It typically begins with a call to GL.Clear() and ends with a call to SwapBuffers.
        /// Called when it is time to render the next frame. Add your rendering code there 
        /// </summary>
        /// <param name=e"> Contains timing information</param>
        /// 
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.PushMatrix();
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Begin(BeginMode.Points);
            //GL.Vertex2(x, y);
            //GL.End();
           

            drawingRobot.robotPose(drawingRobot);
            drawingRobot.drawStickBot2(drawingRobot,ClientRectangle.Width / 2 , ClientRectangle.Height / 2 - 100);
          
            drawingRobot.drawTargetPoint(drawingRobot, ClientRectangle.Width / 2, ClientRectangle.Height / 2 - 100, targetHandLeft);
          drawingRobot.drawTargetPoint(drawingRobot, ClientRectangle.Width / 2, ClientRectangle.Height / 2 - 100, targetHandRight);
            
            
            //compRobot.robotPose(compRobot);
            //compRobot.drawStickBot2(compRobot, ClientRectangle.Width / 2 - 250, ClientRectangle.Height / 2 + 200);
            //compRobot.drawTargetPoint(compRobot, ClientRectangle.Width / 2 - 250, ClientRectangle.Height / 2 + 200, targetHandLeft);
            //compRobot.drawTargetPoint(compRobot, ClientRectangle.Width / 2 - 250, ClientRectangle.Height / 2 + 2000, targetHandRight);

            GL.PopMatrix();
            GL.Flush();
            //Console.WriteLine("{0},{1},{2}",robot.jointers[0].p.x, robot.jointers[0].p.y, robot.jointers[0].p.z);
            SwapBuffers();
        }

        System.IO.StreamWriter file = new System.IO.StreamWriter(@"test.txt");




        /// <summary>
        /// To find the angles needed to get the hands to a given point  
        /// </summary>
        /// <param name=e"> Contains timing information</param>
        /// 
        void findTargetRight(Point hnd, Point elbw, int jind)
        {
            stR.Start();
            double[,] best_joints = new double[99999, 4];
            double pitch1 = new double(), pitch2 = new double();
            double yaw1 = new double(), yaw2 = new double();
            double best_pitch1 = new double(), best_pitch2 = new double(), best_yaw1 = new double(), best_yaw2 = new double();
            double cpitch1 = new double(), cpitch2 = new double(), cyaw1 = new double(), cyaw2 = new double();
            double dist = new double(), minDist = 99999;
            double inc = 0.3, inc2 = 0.025;
            int k = 0;
            file.WriteLine("First Iteration With course Refinement \n");
            for (pitch1 = compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.min; pitch1 <= compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.max; pitch1 += inc)
            {
                compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle = pitch1;
                for (yaw1 = compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.min; yaw1 <= compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.max; yaw1 += inc)
                {
                    compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle = yaw1;
                    for (pitch2 = compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.min; pitch2 <= compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.max; pitch2 += inc)
                    {
                        compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = pitch2;
                        for (yaw2 = compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.min; yaw2 <= compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.max; yaw2 += inc)
                        {
                            compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle = yaw2;

                            compRobot.robotPose(compRobot);

                            dist = getDistance(hnd, compRobot.jointers_rotated[jind]);

                            if (dist < minDist)
                            {
                                // Write the string to a file.

                                file.WriteLine("{0} = ( {1} , {2} ) == ( {3} , {4} )", dist, pitch1, yaw1, pitch2, yaw2);
                                minDist = dist;
                                best_pitch1 = pitch1;
                                best_pitch2 = pitch2;
                                best_yaw1 = yaw1;
                                best_yaw2 = yaw2;
                            }
                        }
                    }
                }
            }
            cpitch1 = best_pitch1;
            cpitch2 = best_pitch2;
            cyaw1 = best_yaw1;
            cyaw2 = best_yaw2;
            minDist = 999999;
            file.WriteLine("\n \nSecond Iteration With Fine Refinement \n");
            for (pitch1 = cpitch1 - inc; pitch1 <= cpitch1 + inc; pitch1 += inc2)
            {
                if ((pitch1 < compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.min) || (pitch1 > compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.max))
                    continue;
                compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle = pitch1;
                for (yaw1 = cyaw1 - inc; yaw1 <= cyaw1 + inc; yaw1 += inc2)
                {
                    if ((yaw1 < compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.min) || (yaw1 > compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.max))
                        continue;
                    compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle = yaw1;
                    for (pitch2 = cpitch2 - inc; pitch2 <= cpitch2 + inc; pitch2 += inc)
                    {
                        if ((pitch1 < compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.min) || (pitch1 > compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.max))
                            continue;
                        compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = pitch2;
                        for (yaw2 = cyaw2 - inc; yaw2 <= cyaw2 + inc; yaw2 += inc)
                        {
                            if ((yaw2 < compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.min) || (yaw2 > compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.max))
                                continue;
                            compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle = yaw2;

                            compRobot.robotPose(compRobot);
                            dist = getDistance(hnd, compRobot.jointers_rotated[jind]);
                            if (dist < minDist)
                            {
                                file.WriteLine("{0} = ( {1} , {2} ) == ( {3} , {4} )", dist, pitch1, yaw1, pitch2, yaw2);
                                minDist = dist;
                                best_pitch1 = pitch1;
                                best_pitch2 = pitch2;
                                best_yaw1 = yaw1;
                                best_yaw2 = yaw2;
                            }

                            if (dist < minDist)
                            {
                                //if the distance is less than predefined min we save it in the box
                                best_joints[k, 0] = pitch1;
                                best_joints[k, 1] = pitch2;
                                best_joints[k, 2] = yaw1;
                                best_joints[k, 3] = yaw2;
                                //printf("%d = %0.2f\n",k,best_joints[k][0]);
                                k++;
                            }
                        }
                    }
                }
            }
            minDist = 999999;
            int i;
            for (i = 0; i < k; i++)
            {
                compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle = best_joints[i, 0];
                compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle = best_joints[i, 1];
                compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = best_joints[i, 2];
                compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle = best_joints[i, 3];
                compRobot.robotPose(compRobot);

                dist = getDistance(elbw, compRobot.jointers_rotated[jind - 1]);
                if (dist < minDist)
                {
                    minDist = dist;
                    best_pitch1 = pitch1;
                    best_pitch2 = pitch2;
                    best_yaw1 = yaw1;
                    best_yaw2 = yaw2;
                }
            }
            compRobot.jointers[(int)dots.RIGHT_SHOULDER].pitch.angle = best_pitch1;
            compRobot.jointers[(int)dots.RIGHT_SHOULDER].yaw.angle = best_yaw1;
            compRobot.jointers[(int)dots.RIGHT_ELBOW].pitch.angle = best_pitch2;
            compRobot.jointers[(int)dots.RIGHT_ELBOW].yaw.angle = best_yaw2;

            stR.Stop();
            Console.WriteLine("The time Elapsed to find the right hand angles = {0}", stR.ElapsedMilliseconds);
            //     file.Close();
        }



        //Left hand angles 

        void findTargetLeft(Point hnd, Point elbw, int jind)
        {
            stL.Start();
            double[,] best_joints = new double[99999, 4];
            double pitch1 = new double(), pitch2 = new double();
            double yaw1 = new double(), yaw2 = new double();
            double best_pitch1 = new double(), best_pitch2 = new double(), best_yaw1 = new double(), best_yaw2 = new double();
            double cpitch1 = new double(), cpitch2 = new double(), cyaw1 = new double(), cyaw2 = new double();
            double dist = new double(), minDist = 99999;
            double inc = 0.3, inc2 = 0.025;
            int k = 0;
            for (pitch1 = compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.min; pitch1 <= compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.max; pitch1 += inc)
            {
                compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle = pitch1;
                for (yaw1 = compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.min; yaw1 <= compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.max; yaw1 += inc)
                {
                    compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle = yaw1;
                    for (pitch2 = compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.min; pitch2 <= compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.max; pitch2 += inc)
                    {
                        compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = pitch2;
                        for (yaw2 = compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.min; yaw2 <= compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.max; yaw2 += inc)
                        {
                            compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle = yaw2;

                            compRobot.robotPose(compRobot);

                            dist = getDistance(hnd, compRobot.jointers_rotated[jind]);

                            if (dist < minDist)
                            {
                                minDist = dist;
                                best_pitch1 = pitch1;
                                best_pitch2 = pitch2;
                                best_yaw1 = yaw1;
                                best_yaw2 = yaw2;
                            }
                        }
                    }
                }
            }
            cpitch1 = best_pitch1;
            cpitch2 = best_pitch2;
            cyaw1 = best_yaw1;
            cyaw2 = best_yaw2;
            for (pitch1 = cpitch1 - inc; pitch1 <= cpitch1 + inc; pitch1 += inc2)
            {
                if ((pitch1 < compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.min) || (pitch1 > compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.max))
                    continue;
                compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle = pitch1;
                for (yaw1 = cyaw1 - inc; yaw1 <= cyaw1 + inc; yaw1 += inc2)
                {
                    if ((yaw1 < compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.min) || (yaw1 > compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.max))
                        continue;
                    compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle = yaw1;
                    for (pitch2 = cpitch2 - inc; pitch2 <= cpitch2 + inc; pitch2 += inc)
                    {
                        if ((pitch1 < compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.min) || (pitch1 > compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.max))
                            continue;
                        compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = pitch2;
                        for (yaw2 = cyaw2 - inc; yaw2 <= cyaw2 + inc; yaw2 += inc)
                        {
                            if ((yaw2 < compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.min) || (yaw2 > compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.max))
                                continue;
                            compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle = yaw2;

                            compRobot.robotPose(compRobot);
                            dist = getDistance(hnd, compRobot.jointers_rotated[jind]);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                best_pitch1 = pitch1;
                                best_pitch2 = pitch2;
                                best_yaw1 = yaw1;
                                best_yaw2 = yaw2;
                            }

                            if (dist < minDist)
                            {
                                //if the distance is less than predefined min we save it in the box
                                best_joints[k, 0] = pitch1;
                                best_joints[k, 1] = pitch2;
                                best_joints[k, 2] = yaw1;
                                best_joints[k, 3] = yaw2;
                                //printf("%d = %0.2f\n",k,best_joints[k][0]);
                                k++;
                            }
                        }
                    }
                }
            }
            minDist = 999999;
            int i;
            for (i = 0; i < k; i++)
            {
                compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle = best_joints[i, 0];
                compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle = best_joints[i, 1];
                compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = best_joints[i, 2];
                compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle = best_joints[i, 3];
                compRobot.robotPose(compRobot);

                dist = getDistance(elbw, compRobot.jointers_rotated[jind - 1]);
                if (dist < minDist)
                {
                    minDist = dist;
                    best_pitch1 = pitch1;
                    best_pitch2 = pitch2;
                    best_yaw1 = yaw1;
                    best_yaw2 = yaw2;
                }
            }
            compRobot.jointers[(int)dots.LEFT_SHOULDER].pitch.angle = best_pitch1;
            compRobot.jointers[(int)dots.LEFT_SHOULDER].yaw.angle = best_yaw1;
            compRobot.jointers[(int)dots.LEFT_ELBOW].pitch.angle = best_pitch2;
            compRobot.jointers[(int)dots.LEFT_ELBOW].yaw.angle = best_yaw2;
            stL.Stop();
            Console.WriteLine("The time Elapsed to find the left hand angles = {0}", stL.ElapsedMilliseconds);
        }


        double getDistance(Point p1, Point p2)
        {
            double x, y, z;

            x = p1.x - p2.x;
            y = p1.y - p2.y;
            z = p1.z - p2.z;

            double dist = x * x + y * y + z * z;

            return Math.Sqrt(dist);
        }


 
      
        
    }
}




