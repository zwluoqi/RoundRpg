using System;
using System.Collections.Generic;
using UnityEngine;




    public class MathUtil
    {

      
        public static Vector3 GetDistance(Vector3 position1, double radius1, Vector3 position2, double radius2, out double dis)
        {
            position1.y = 0;
            position2.y = 0;
            Vector3 sourceDir = position2 - position1;
            sourceDir.y = 0;
            dis = sourceDir.magnitude - (radius1 + radius2);
            if (dis > 0)
            {
                Vector3 rd = (float)dis * sourceDir.normalized;
                return rd;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public static Vector3 GetDistance(Vector3 position1, double radius1, Vector3 position2, double radius2)
        {
            double dis;
            position1.y = 0;
            position2.y = 0;
            Vector3 d = position2 - position1;
            d.y = 0;
            dis = d.magnitude - (radius1 + radius2);
            if (dis > 0)
            {
                Vector3 rd = (float)dis * d.normalized;
                return rd;
            }
            else
            {
                return Vector3.zero;
            }
        }


        public static Vector3 TransPosition(Vector3 relativePosition, Vector3 sourcePos, Vector3 forawrd, out Vector3 dir)
        {
            double x = relativePosition.z * forawrd.x - relativePosition.x * forawrd.z;
            double z = relativePosition.z * forawrd.z + relativePosition.x * forawrd.x;
            Vector3 newPos = new Vector3(sourcePos.x + (float)x, sourcePos.y + relativePosition.y, sourcePos.z + (float)z);
            dir = newPos - sourcePos;
            return newPos;

        }



		public static float zeroDis = 0.001f;
		public static float zeroDisSqrt = 0.000001f;


        public static float SqrtDistanceNoY(Vector3 a, Vector3 b)
        {
            float x = a.x - b.x;
            float z = a.z - b.z;
            return x * x + z * z;
        }

        public static float DistanceNoY(Vector3 a, Vector3 b)
        {
            float sqrtDis = SqrtDistanceNoY(a, b);
            return Mathf.Sqrt(sqrtDis);
        }


        public static bool PointInEllipse(Vector3 position0,float a,float b,float w,float l)
        {
            return (w * w) / (a * a) + (l * l) / (b * b) <= 1;
        }



        //cneter在矩形中点
        public static bool PointInRect(Vector3 position0, float radiu, Vector3 dir, Vector3 center, float length, float width)
        {
            Vector3 dir0 = dir.normalized;
            Vector3 dir1 = dir0;
            Vector3 dir2 = Rotation(dir0, 90);//Quaternion.Euler(0, 90, 0) * dir0;

            Vector3 position1 = center - dir2 * length / 2 - dir1 * (width / 2 + radiu);
            Vector3 position2 = center + dir2 * length / 2 - dir1 * (width / 2 + radiu);
//            Vector3 position3 = center + dir2 * length / 2 + dir1 * (width / 2 + radiu);
            Vector3 position4 = center - dir2 * length / 2 + dir1 * (width / 2 + radiu);

            Vector3 offsetX = position2 - position1;
            Vector3 offsetZ = position4 - position1;

            float projectX = Vector3.Dot(position0 - position1, offsetX) ;
            float projectZ = Vector3.Dot(position0 - position1, offsetZ) ;
            if (projectX <= offsetX.sqrMagnitude && projectX >= 0 && projectZ <= offsetZ.sqrMagnitude && projectZ >= 0)
            {
                return true;
            }
            return false;
        }

        //cneter在矩形下方
        public static bool PointInRectBySelect(Vector3 position0, double radiu, Vector3 dir, Vector3 center, double length, double width)
        {
            Vector3 dir0 = dir.normalized;
            Vector3 dir1 = dir0;
            Vector3 dir2 = Rotation(dir0, 90); //Quaternion.Euler(0, 90, 0) * dir0;

            Vector3 position1 = center - dir2 * (float)width / 2;
            Vector3 position2 = center + dir2 * (float)width / 2;
//            Vector3 position3 = center + dir2 * (float)width / 2 + dir1 * (float)(length + radiu);
            Vector3 position4 = center - dir2 * (float)width / 2 + dir1 * (float)(length + radiu);

            Vector3 offsetX = position2 - position1;
            Vector3 offsetZ = position4 - position1;

            double projectX = Vector3.Dot(position0 - position1, offsetX) ;
            double projectZ = Vector3.Dot(position0 - position1, offsetZ) ;
            if (projectX <= offsetX.sqrMagnitude && projectX >= 0 && projectZ <= offsetZ.sqrMagnitude && projectZ >= 0)
            {
                return true;
            }
            return false;
        }


        public static bool PointInRectCenterAtDown(Vector3 p,Vector3 p1,Vector3 p2,Vector3 p3,Vector3 p4)
        {
            Vector3 offsetX = p2 - p1;
            Vector3 offsetZ = p4 - p1;

            double projectX = Vector3.Dot(p - p1, offsetX);
            double projectZ = Vector3.Dot(p - p1, offsetZ);
            if (projectX <= offsetX.sqrMagnitude && projectX >= 0 && projectZ <= offsetZ.sqrMagnitude && projectZ >= 0)
            {
                return true;
            }
            return false;
        }


        public static bool PointInCircle(Vector3 position0, double ignoreRadiu, Vector3 dir, Vector3 center, double radius, double angle)
        {
            if (angle == 360)
                return PointIn360Circle(position0, ignoreRadiu, center, radius);

            Vector3 offsetAngle = position0 - center;
            offsetAngle.y = 0;
            double angleD2 = Vector3.Angle(dir, offsetAngle);
            if (MathUtil.GetDistance(position0, ignoreRadiu, center, radius) == Vector3.zero
                && angleD2 <= angle * 0.5f)
            {
                return true;
            }
            return false;
        }

        public static bool PointIn360Circle(Vector3 position0, double ignoreRadiu,Vector3 center, double radius )
        {
            Vector3 offsetAngle = position0 - center;
            offsetAngle.y = 0;
            if (MathUtil.GetDistance(position0, ignoreRadiu, center, radius) == Vector3.zero)
            {
                return true;
            }
            return false;
        }

        public static Vector3 Rotation(Vector3 source, double angle)
        {
            double x = source.x;
            double z = source.z;

            double rad = Mathf.Deg2Rad * angle;

            double target_x = x * Mathf.Cos((float)rad) + z * Mathf.Sin((float)rad);
            double target_z = x * Mathf.Sin(-(float)rad) + z * Mathf.Cos((float)rad);

            return new Vector3((float)target_x, 0, (float)target_z);
        }


		public static float VectorTEuler(Vector3 source)
		{
			return Mathf.Atan2(source.x, source.z) * Mathf.Rad2Deg;
		}

		public static Vector3 EulerTVector(float angle)
		{
			float rad = angle * Mathf.Deg2Rad;
			return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
		}



    }

