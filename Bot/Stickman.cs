using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;




namespace Bot
{

    public enum dots
    {
        HEART = 0,
        NECK,
        HEAD,
        TORSO,
        LEFT_SHOULDER,
        LEFT_ELBOW,
        LEFT_HAND,
        RIGHT_SHOULDER,
        RIGHT_ELBOW,
        RIGHT_HAND
    };
    public struct Point
    {
        public double x;
        public double y;
        public double z;
    };
    
   
    class Stickman
    {
        public JOINT[] jointers = new JOINT[10];
        public Point[] jointers_rotated = new Point[10];

        public const int d = -1000;

       
        //The constructor 
        public Stickman()
        {



            //Initializing the joint parameters
            for (int i = 0; i < 10; i++)
            {
                createObj(i);
            }

            jointers[(int)dots.HEART].p.x = 0.0; jointers[(int)dots.HEART].p.y = 100.0; jointers[(int)dots.HEART].p.z = 0.0;
            jointers[(int)dots.HEART].neighbors[0] = (int)dots.NECK;
            jointers[(int)dots.HEART].neighbors[1] = (int)dots.TORSO;
            jointers[(int)dots.HEART].neighbors[2] = (int)dots.LEFT_SHOULDER;
            jointers[(int)dots.HEART].neighbors[3] = (int)dots.RIGHT_SHOULDER;
            jointers[(int)dots.HEART].numNeighbors = 4;
            jointers[(int)dots.HEART].pitchInd = 0;
            jointers[(int)dots.HEART].pitch.angle = 0.0;// radians(0.0);
            jointers[(int)dots.HEART].pitch.min = -99999;
            jointers[(int)dots.HEART].pitch.max = 99999;
            jointers[(int)dots.HEART].yawInd = 1; // here we take x =0 and y = 1 and z = 2
            jointers[(int)dots.HEART].yaw.angle = 0.0;// radians(0.0);
            jointers[(int)dots.HEART].yaw.min = -99999;
            jointers[(int)dots.HEART].yaw.max = 99999;

            jointers[(int)dots.NECK].p.x = 0.0; jointers[(int)dots.NECK].p.y = 126.5; jointers[(int)dots.NECK].p.z = 0.0;
            jointers[(int)dots.NECK].neighbors[0] = (int)dots.HEAD;
            jointers[(int)dots.NECK].numNeighbors = 1;
            jointers[(int)dots.NECK].pitchInd = 1;
            jointers[(int)dots.NECK].pitch.angle = 0.0;// radians(0.0);
            jointers[(int)dots.NECK].pitch.min = -(Math.PI / 2.0);
            jointers[(int)dots.NECK].pitch.max = Math.PI / 2.0;
            jointers[(int)dots.NECK].yawInd = 0;
            jointers[(int)dots.NECK].yaw.angle = 0.0;// radians(0.0);
            jointers[(int)dots.NECK].yaw.min = -(Math.PI / 4.0);
            jointers[(int)dots.NECK].yaw.max = Math.PI / 4.0;

            jointers[(int)dots.HEAD].p.x = 0.0; jointers[(int)dots.HEAD].p.y = 212.4; jointers[(int)dots.HEAD].p.z = 0.0;
            jointers[(int)dots.HEAD].numNeighbors = 0;
            jointers[(int)dots.HEAD].pitchInd = -1;
            jointers[(int)dots.HEAD].yawInd = -1;

            jointers[(int)dots.TORSO].p.x = 0.0; jointers[(int)dots.TORSO].p.y = 0.0; jointers[(int)dots.TORSO].p.z = 0.0;
            jointers[(int)dots.TORSO].numNeighbors = 0;
            jointers[(int)dots.TORSO].pitchInd = -1;
            jointers[(int)dots.TORSO].yawInd = -1;


            jointers[(int)dots.LEFT_SHOULDER].p.x = 98.0; jointers[(int)dots.LEFT_SHOULDER].p.y = 100.0; jointers[(int)dots.LEFT_SHOULDER].p.z = 0.0;
            jointers[(int)dots.LEFT_SHOULDER].neighbors[0] = (int)dots.LEFT_ELBOW;
            jointers[(int)dots.LEFT_SHOULDER].numNeighbors = 1;
            jointers[(int)dots.LEFT_SHOULDER].pitchInd = 0;
            jointers[(int)dots.LEFT_SHOULDER].pitch.angle = 0.0;// radians(0.0);
            jointers[(int)dots.LEFT_SHOULDER].pitch.min = -1.5621;// radians(-119.5);//-4.0 * M_PI / 3.0;
            jointers[(int)dots.LEFT_SHOULDER].pitch.max = 1.5621;// radians(119.5);//M_PI / 3.0;
            jointers[(int)dots.LEFT_SHOULDER].yawInd = 1;
            jointers[(int)dots.LEFT_SHOULDER].yaw.angle = 0.0;// radians(0.0);
            jointers[(int)dots.LEFT_SHOULDER].yaw.min = 0.0087;// radians(-18);//0.0;
            jointers[(int)dots.LEFT_SHOULDER].yaw.max = 1.6494;// radians(76);//M_PI / 2.0;

            jointers[(int)dots.LEFT_ELBOW].p.x = 98.0; jointers[(int)dots.LEFT_ELBOW].p.y = 100.0; jointers[(int)dots.LEFT_ELBOW].p.z = 90.0;
            jointers[(int)dots.LEFT_ELBOW].neighbors[0] = (int)dots.LEFT_HAND;
            jointers[(int)dots.LEFT_ELBOW].numNeighbors = 1;
            jointers[(int)dots.LEFT_ELBOW].pitchInd = 2;
            jointers[(int)dots.LEFT_ELBOW].pitch.angle = 0.0;// 0.0;// radians(0.0);
            jointers[(int)dots.LEFT_ELBOW].pitch.min = 0.0;// radians(-119.5);//-5.0 * M_PI / 3.0;
            jointers[(int)dots.LEFT_ELBOW].pitch.max = 2.0857;// radians(119.5);//M_PI / 6.0;
            jointers[(int)dots.LEFT_ELBOW].yawInd = 1;
            jointers[(int)dots.LEFT_ELBOW].yaw.angle = 0.0;// radians(-2.0);
            jointers[(int)dots.LEFT_ELBOW].yaw.min = -1.5621;// radians(-88.5);//-M_PI / 1.8;
            jointers[(int)dots.LEFT_ELBOW].yaw.max = -0.0087;// radians(-2.0);

            jointers[(int)dots.LEFT_HAND].p.x = 98.0; jointers[(int)dots.LEFT_HAND].p.y = 100.0; jointers[(int)dots.LEFT_HAND].p.z = 90.0 + 50.55 + 58.0;
            jointers[(int)dots.LEFT_HAND].numNeighbors = 0;
            jointers[(int)dots.LEFT_HAND].pitchInd = -1;
            jointers[(int)dots.LEFT_HAND].yawInd = -1;

            jointers[(int)dots.RIGHT_SHOULDER].p.x = -98.0; jointers[(int)dots.RIGHT_SHOULDER].p.y = 100.0; jointers[(int)dots.RIGHT_SHOULDER].p.z = 0.0;
            jointers[(int)dots.RIGHT_SHOULDER].neighbors[0] = (int)dots.RIGHT_ELBOW;
            jointers[(int)dots.RIGHT_SHOULDER].numNeighbors = 1;
            jointers[(int)dots.RIGHT_SHOULDER].pitchInd = 0;
            jointers[(int)dots.RIGHT_SHOULDER].pitch.angle = 0.0; //radians(0.0);
            jointers[(int)dots.RIGHT_SHOULDER].pitch.min = -1.5621;// radians(-119.5);//-4.0 * M_PI / 3.0;
            jointers[(int)dots.RIGHT_SHOULDER].pitch.max = 1.5621;// radians(119.5);//M_PI / 3.0;
            jointers[(int)dots.RIGHT_SHOULDER].yawInd = 1;
            jointers[(int)dots.RIGHT_SHOULDER].yaw.angle = 0.0;// radians(0.0);
            jointers[(int)dots.RIGHT_SHOULDER].yaw.min = -1.6494;// radians(-76);//-M_PI / 2.0;
            jointers[(int)dots.RIGHT_SHOULDER].yaw.max = -0.0087;// radians(18);

            jointers[(int)dots.RIGHT_ELBOW].p.x = -98.0; jointers[(int)dots.RIGHT_ELBOW].p.y = 100.0; jointers[(int)dots.RIGHT_ELBOW].p.z = 90.0;
            jointers[(int)dots.RIGHT_ELBOW].neighbors[0] = (int)dots.RIGHT_HAND;
            jointers[(int)dots.RIGHT_ELBOW].numNeighbors = 1;
            jointers[(int)dots.RIGHT_ELBOW].pitchInd = 2;
            jointers[(int)dots.RIGHT_ELBOW].pitch.angle = 0.0;// radians(0.0);
            jointers[(int)dots.RIGHT_ELBOW].pitch.min = 0.0;// radians(-119.5);//-M_PI / 6.0;
            jointers[(int)dots.RIGHT_ELBOW].pitch.max = 2.0857;// radians(119.5);//5.0 * M_PI / 3.0;
            jointers[(int)dots.RIGHT_ELBOW].yawInd = 1;
            jointers[(int)dots.RIGHT_ELBOW].yaw.angle = 0.0;// radians(0.0);
            jointers[(int)dots.RIGHT_ELBOW].yaw.min = 0.0087;// radians(2.0);//-M_PI / 1.8;
            jointers[(int)dots.RIGHT_ELBOW].yaw.max = 1.5621;// radians(88.5);//0.0;


            jointers[(int)dots.RIGHT_HAND].p.x = -98.0; jointers[(int)dots.RIGHT_HAND].p.y = 100.0; jointers[(int)dots.RIGHT_HAND].p.z = 90.0 + 50.55 + 58.0;
            jointers[(int)dots.RIGHT_HAND].numNeighbors = 0;
            jointers[(int)dots.RIGHT_HAND].pitchInd = -1;
            jointers[(int)dots.RIGHT_HAND].yawInd = -1;
        }

        void createObj(int jointNumber)
        {
            jointers[jointNumber] = new JOINT();
            jointers[jointNumber].p = new Point();
            jointers[jointNumber].pitch = new MOTOR();
            jointers[jointNumber].yaw = new MOTOR();
            jointers[jointNumber].neighbors = new int[4];

        }

        ///<summary>
        ///This are functions for rotation of the stickBot
        ///</summary>


        public void robotPose(Stickman robot)
        {
            int i, j;
            //what we see in robotpose()

            //in this for loop we will just copy the current posititions of the robot

            for (i = 0; i < 10; i++)
            {
                robot.jointers_rotated[i].x = robot.jointers[i].p.x;
                robot.jointers_rotated[i].y = robot.jointers[i].p.y;
                robot.jointers_rotated[i].z = robot.jointers[i].p.z;
            }

            double[] angles = new double[3];
            //here  we start with the hand, nd start the rotation

            //that is for example first that comes is Righthand=9, Lefthand=6, Torso=2 and head=3 .. so for that joint the yawInd & pitchInd is -1 so nothing is done 
            //for r
            for (i = 9; i >= 0; i--)
            {
                //so i will take the  right_elbow as example - it's p = (-98.0, 100.0, 90.0) , i = 8, angles(0.0,1.56210,0.0) - the yaw rotation of the right elbow is along Y and the pitch will be along z
                // then 
                // yaw first
                if (robot.jointers[i].yawInd >= 0)
                {
                    for (j = 0; j < 3; j++)
                        angles[j] = 0.0;
                    angles[robot.jointers[i].yawInd] = robot.jointers[i].yaw.angle;

                    if (robot.jointers[i].yaw.angle > robot.jointers[i].yaw.max)
                        angles[robot.jointers[i].yawInd] = robot.jointers[i].yaw.max;
                    if (robot.jointers[i].yaw.angle < robot.jointers[i].yaw.min)
                        angles[robot.jointers[i].yawInd] = robot.jointers[i].yaw.min;

                    Rotate(robot, robot.jointers[i].p, i, angles); //point p 


                }

                // then pitch
                if (robot.jointers[i].pitchInd >= 0)
                {
                    for (j = 0; j < 3; j++)
                        angles[j] = 0.0;
                    angles[robot.jointers[i].pitchInd] = robot.jointers[i].pitch.angle;

                    if (robot.jointers[i].pitch.angle > robot.jointers[i].pitch.max)
                        angles[robot.jointers[i].pitchInd] = robot.jointers[i].pitch.max;
                    if (robot.jointers[i].pitch.angle < robot.jointers[i].pitch.min)
                        angles[robot.jointers[i].pitchInd] = robot.jointers[i].pitch.min;

                    Rotate(robot, robot.jointers[i].p, i, angles);
                }
            }
        }

        public void Rotate(Stickman robot, Point p, int index, double[] angles)
        {
            int i;
            Point np = new Point();
            for (i = 0; i < robot.jointers[index].numNeighbors; i++)
            {
                rotatePoint(out np, robot.jointers_rotated[robot.jointers[index].neighbors[i]], angles, p);
                robot.jointers_rotated[robot.jointers[index].neighbors[i]].x = np.x;
                robot.jointers_rotated[robot.jointers[index].neighbors[i]].y = np.y;
                robot.jointers_rotated[robot.jointers[index].neighbors[i]].z = np.z;

                Rotate(robot, p, robot.jointers[index].neighbors[i], angles);
            }
        }



        public void rotatePoint(out Point np, Point p, double[] angles, Point center)
        {
            Point p1 = new Point();
            Point p2 = new Point();

            p2.x = p.x - center.x;
            p2.y = p.y - center.y;
            p2.z = p.z - center.z;
            //rotate by angle[1]
            p1.x = p2.x * Math.Cos(angles[1]) + p2.z * Math.Sin(angles[1]);
            p1.y = p2.y;
            p1.z = p2.x * (-Math.Sin(angles[1])) + p2.z * Math.Cos(angles[1]);

            // rotate by angles[0]
            p2.x = p1.x;
            p2.y = p1.y * Math.Cos(angles[0]) + p1.z * (-Math.Sin(angles[0]));
            p2.z = p1.y * Math.Sin(angles[0]) + p1.z * Math.Cos(angles[0]);

            //rotate by angle[2]
            np.x = p2.x * Math.Cos(angles[2]) + p2.y * (-Math.Sin(angles[2]));
            np.y = p2.x * Math.Sin(angles[2]) + p2.y * (Math.Cos(angles[2]));
            np.z = p2.z;


            np.x += center.x;
            np.y += center.y;
            np.z += center.z;
        }

        public double project3D(double v, double z, double tv)
        {
            return (((v - tv) * d) / (z + d)) + tv;
        }





        ///<summary>
        ///This are functions for drawing the stickbot
        ///</summary>
        ///


        //***************************************Functions for drawing******************************/

        public unsafe void drawTargetPoint(Stickman robot, int cx, int cy, Point p)
        {
            Point targetRotate = new Point();
            double[] angles = new double[3];

            angles[0] = robot.jointers[(int)dots.HEART].pitch.angle;
            //  Console.WriteLine(" 1- {0}, {1}, {2}", angles[0], angles[1], angles[2]);
            angles[1] = robot.jointers[(int)dots.HEART].yaw.angle;
            // Console.WriteLine("2 - {0}, {1}, {2}", angles[0], angles[1], angles[2]);
            angles[2] = 0.0;
            //  Console.WriteLine("3 - {0}, {1}, {2}", angles[0], angles[1], angles[2]);

            //this reference is to represent the pointer
            rotatePoint(out targetRotate, p, angles, robot.jointers[(int)dots.HEART].p);

            GL.Color3(1.0f, 1.0f, 1.0f);
            drawCircle(project3D(targetRotate.x, targetRotate.z, 0) + cx,
                project3D(targetRotate.y, targetRotate.z, 0) + cy, 4);

        }


        public void drawStickBot2(Stickman robot, int cx, int cy)
        {
            int i;

            GL.Color3(1.0f, 0.0f, 0.0f);
            for (i = 0; i < 10; i++)
            {
                //   i = 8;
                drawCircle(project3D(robot.jointers_rotated[i].x, robot.jointers_rotated[i].z, 0) + cx,
                               project3D(robot.jointers_rotated[i].y, robot.jointers_rotated[i].z, 0) + cy,
                               2);

            }

            for (i = 0; i < 10; i++)
            //   i = 8;
            {
                drawSegments(robot, i, cx, cy);
            }
        }

        public void drawSegments(Stickman robot, int jindex, int cx, int cy)
        {
            int i, ind;

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Begin(BeginMode.Lines);
            for (i = 0; i < robot.jointers[jindex].numNeighbors; i++)
            {
                ind = robot.jointers[jindex].neighbors[i];
                GL.Vertex2(project3D(robot.jointers_rotated[jindex].x, robot.jointers_rotated[jindex].z, 0) + cx,
                           project3D(robot.jointers_rotated[jindex].y, robot.jointers_rotated[jindex].z, 0) + cy);
                GL.Vertex2(project3D(robot.jointers_rotated[ind].x, robot.jointers_rotated[ind].z, 0) + cx,
                           project3D(robot.jointers_rotated[ind].y, robot.jointers_rotated[ind].z, 0) + cy);
            }
            GL.End();


        }

        public void drawCircle(double x, double y, int r)
        {
            double ang, inc = Math.PI / 50;

            GL.Begin(BeginMode.Polygon);
            for (ang = 0; ang < ((2.0 * Math.PI) + inc); ang += inc)
            {
                GL.Vertex2(x + Math.Cos(ang) * r, y + Math.Sin(ang) * r);
            }
            GL.End();
        }


    }
}
