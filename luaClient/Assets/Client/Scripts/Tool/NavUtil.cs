using System;
using System.Collections.Generic;
using UnityEngine;




    public class NavUtil
    {


        /// <summary>
        /// 碰撞场景点
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="nextPos"></param>
        public static bool CollidePosition(Vector3 origin, ref Vector3 nextPos,Vector3 target)
        {
            NavMeshHit hit;
            if (NavMesh.Raycast(origin, target, out hit, -1))
            {
                Vector3 dir = nextPos - origin;

                dir = Vector3.Project(dir, MathUtil.Rotation(hit.normal,90));
                nextPos = origin + dir;
                if(dir != Vector3.zero)
                {
                    PositionValidate(nextPos, out nextPos);
                }
                return true;
            }
            return false;
        }

        public static bool NavMeshRaycast(Vector3 origin, Vector3 target)
        {
            NavMeshHit hit;
            bool flag = NavMesh.Raycast(origin, target, out hit, -1);
            if(flag)
            {
                if(hit.normal!=Vector3.zero)
                {
                    return true;
                }
            }
            return false;
        }

		public static float zeroDis = 0.001f;
		public static float zeroDisSqrt = 0.000001f;

        public static bool DetectLineInNavRange(Vector3 source, Vector3 target,out Vector3 collidePos)
        {
            NavMeshHit hit;
            bool collide = false;
            if (NavMesh.Raycast(source, target, out hit, -1))
            {
                if (MathUtil.SqrtDistanceNoY(target, hit.position) > zeroDisSqrt)
                {
                    collide = true;
                    collidePos = hit.position;
                }
                else
                {
                    collidePos = target;
                }
            }
            else
            {
                collidePos = target;
            }
            return !collide;
        }


        public static bool UsePhysics = false;
        /// <summary>
        /// 在点网格中寻找矫正点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="posCorrect"></param>
        /// <returns></returns>
        public static bool PositionValidate(Vector3 pos, out Vector3 posCorrect)
        {
            NavMeshHit hit;
            bool validate = NavMesh.SamplePosition(pos, out hit, 100f, -1);
            if (validate)
            {
                posCorrect = hit.position;
            }
            else
            {
                posCorrect = pos;
            }
            return validate;
        }

		public static string moveLayerName = "moveLayerName";

        public static Vector3 topPos = new Vector3(0, 100, 0);
        /// <summary>
        /// 向下在box上寻找校正点，相对效率更高
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="posCorrect"></param>
        /// <returns></returns>
        public static bool PositionOnMoveCollideValidate(Vector3 pos, out Vector3 posCorrect)
        {
            RaycastHit hit;
		if (Physics.Raycast(pos + topPos, Vector3.down, out hit, 150,LayerMask.NameToLayer( moveLayerName)))
            {
                //如果有物理碰撞点，就用碰撞点从navmesh上采样出真正的位置
                posCorrect = hit.point;
                Vector3 navCorrect;
                if (PositionValidate(posCorrect, out navCorrect))
                {
                    posCorrect = navCorrect;
                }
                return true;
            }
            else
            {
                posCorrect = pos;
                return false;
            }
        }


    }

