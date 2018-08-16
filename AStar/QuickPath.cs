// * ------------------------
// * Quick Path  ver 1.0
// * (两种算法)
// *
// *
// * 1. A* 算法,最优路径
// * 2. 快速不最优算法, 多次迭代
// * 3. 算法优化
// * 4. 拐角处理 
// * ------------------------



using System;
using UnityEngine;
using System.Collections.Generic;


namespace KGFrame
{
     

    /// <summary>
    /// Quick Path 快速寻路
    /// </summary>
    public class QuickPath 
    {
        // sbyte  8位有符号整数   -128 to 127
        //  0  = 可以移动
        //  -1 = 不可以移动
        //  1  = 障碍物

        public struct NPoint
        {
            public int F;               // G + H, 总消耗
            public int G;               // 该点到上一步的值
            public int H;               // 该点到目标的值
            public float X;               //
            public float Y;               //
            public float ParentX;         //
            public float ParentY;         //
            public bool bSucess;        //用来判断该位置是否合法, 如果为False,则不进入List.
        }


        public GMap map;                // 地图数据
        public int iMaxFind = 50;       //最大寻找格子数量, 防止超复杂地图
        public int iMaxStep = 100;      //最大步数, 防止超时 
        public bool bSimple = false;    //简单寻找

        private NPoint nStart;          //开始坐标
        private NPoint nEnd;            //结束坐标
        private ArrayContainer arrOpen = new ArrayContainer();
        private ArrayContainer arrClose = new ArrayContainer();  


        /// <summary>
        /// 得到路径
        /// 结果可能返回NULL
        /// 
        /// </summary>
        /// <param name="vForm"></param>
        /// <param name="vEnd"></param>
        /// <param name="bSimple"></param>
        /// <returns></returns>
        public List<NPoint> GetPath(Vector3 vForm, Vector3 vEnd, bool _bSimple = false)
        {
            if (map == null && GMap.instance !=null)
            {
                map = GMap.instance;
            }

            if (map == null || map.bitMap == null)
            {
                Debug.Log("No Map Data");
                return null;
            }

            bSimple = _bSimple;

            if (!bSimple)
            {
                iMaxFind = 200;
                iMaxStep = 200;
                return CalcAStarPath(vForm, vEnd);
            }
            else
            {
                iMaxFind = 50;
                iMaxStep = 100;
                return CalcSimplePath(vForm, vEnd);
            }
        }



        /// <summary>
        /// 计算简单路径
        /// 结果可能返回NULL
        /// </summary>
        /// <param name="vForm"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public List<NPoint> CalcSimplePath(Vector3 vForm, Vector3 vEnd, float fForward = 0.5f, float fRadius = 0.5f)
        {

            int iStartX = Mathf.RoundToInt(vForm.x + map.OffsetX);
            int iStartY = Mathf.RoundToInt(vForm.z + map.OffsetY);
            int iEndX = Mathf.RoundToInt(vEnd.x + map.OffsetX);
            int iEndY = Mathf.RoundToInt(vEnd.z + map.OffsetY); 
            if (iStartX == iEndX && iStartY == iEndY)
                return null;
              
            List<NPoint> mPath = new List<NPoint>();

            Vector2 vCur = new Vector2(vForm.x, vForm.z);
            Vector2 vTo = new Vector2(vEnd.x, vEnd.z);

            int iDelayOut = 0;
            while (iDelayOut < iMaxFind)
            {
                Vector2 vNext = Vector2.MoveTowards(vCur, vTo, fForward + fRadius);
                if (CalcForwardCanMove(vCur, vNext) == false)
                { 
                    vNext = GetSimpleNextPos(ref vCur, vForm, vEnd, fForward);
                    if (vNext == Vector2.zero)
                    {
                        break;//结束寻找
                    }
                } 

                NPoint mpoint = new NPoint();
                mpoint.X = vNext.x;
                mpoint.Y = vNext.y;
                mpoint.ParentX = vCur.x;
                mpoint.ParentY = vCur.y;
                mpoint.bSucess = true;
                mPath.Add(mpoint);

                vCur = new Vector2(vNext.x, vNext.y); 

                if(Vector2.Distance(vNext, vTo) < fRadius){  
                    break;
                }

                iDelayOut++;
            }


            if (map.GetMap(vEnd) == 0 && mPath.Count > 0)
            {
                NPoint mEndLast = mPath[mPath.Count - 1];
                if (Vector2.Distance(new Vector2(mEndLast.X, mEndLast.Y), new Vector2(vEnd.x, vEnd.z)) < fRadius)
                {  //如果最后一步与目标少于fRadius, 则把最后一步放入
                    NPoint mEndPoint = new NPoint();
                    mEndPoint.X = vEnd.x;
                    mEndPoint.Y = vEnd.z;
                    mEndPoint.bSucess = true;
                    mPath.Add(mEndPoint); 
                }
            }

            mPath.Reverse();
            return mPath;
        }




        /// <summary>
        /// 得到下一个目标
        /// </summary>
        /// <returns></returns>
        private Vector2 GetSimpleNextPos(ref Vector2 vCur, Vector3 vForm, Vector3 vEnd, float fForward)
        {
 
            Vector2 vNext = Vector2.zero;

            int curx = Mathf.RoundToInt(vCur.x);
            int cury = Mathf.RoundToInt(vCur.y);
           

            if (vForm.x > vEnd.x)
            {
                // 向左
                if (map.GetMap(curx - 1, cury) == 0 && vCur.x - fForward >= vEnd.x)
                    return new Vector2(curx - 1, vCur.y);
            }
            else
            {
                //向右
                if (map.GetMap(curx + 1, cury) == 0 && vCur.x + fForward <= vEnd.x)
                {
                    return new Vector2(curx + 1, vCur.y);
                }
            }


            if (vForm.z > vEnd.z)
            {

                if (map.GetMap(curx, cury - 1) == 0 && vCur.y - fForward >= vEnd.z)
                    return new Vector2(vCur.x, cury - 1);
            }
            else
            {
                if (map.GetMap(curx, cury + 1) == 0 && vCur.y + fForward <= vEnd.z)
                    return new Vector2(vCur.x, cury + 1);
            }  

            return vNext;
        }



        /// <summary>
        /// 计算是否能够向前移动
        /// </summary>
        /// <returns></returns>
        private bool CalcForwardCanMove(Vector2 vthis, Vector2 vto)
        { 
            if (map.GetMap(vto) == 0) 
                return true; 
            else
                return false;
        }


         


        /// <summary>
        /// 计算A* 路径
        /// </summary>
        /// <param name="vForm"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public List<NPoint> CalcAStarPath(Vector3 vForm, Vector3 vEnd)
        {
             
            int iStartX = Mathf.RoundToInt(vForm.x + map.OffsetX);
            int iStartY = Mathf.RoundToInt(vForm.z + map.OffsetY);
            int iEndX = Mathf.RoundToInt(vEnd.x + map.OffsetX);
            int iEndY = Mathf.RoundToInt(vEnd.z + map.OffsetY); 

            if (iStartX == iEndX && iStartY == iEndY) 
                return null;  

            arrOpen.Clear();
            arrClose.Clear();

            List<NPoint> mPath = new List<NPoint>();

            nStart.X = iStartX;
            nStart.Y = iStartY;
            nStart.bSucess = true;

            nEnd.X = iEndX;
            nEnd.Y = iEndY;

            nStart = GetPoint(iStartX, iStartY, nStart);
            arrClose.Add(nStart);


            //开始计算
            CalcAroundPoint(nStart);

            int iDelayOut = 0;
            int iLast = -1;         //最后一步
            while (arrOpen.Count > 0 && iDelayOut < iMaxFind)
            {
                NPoint np = arrOpen.GetMin();
                arrClose.Add(np);
                arrOpen.Remove(np);

                iLast = arrClose.Find(nEnd); // 如果最后一步已经找到
                if (iLast >= 0)
                {
                    break;
                }

                CalcAroundPoint(np);
                iDelayOut++;
            }

            if (iDelayOut >= iMaxFind)
            {
                //没有找到路径, 用最近的路径
                NPoint nE = arrClose.GetMin();
                iLast = arrClose.Find(nE);
                nE.X -= map.OffsetX;
                nE.Y -= map.OffsetY;
                mPath.Add(nE);
            }
            else
            {   //把最后一步放到列表里
                NPoint nLast = arrClose.Get(iLast);
                if (nLast.bSucess)
                {
                    nLast.X -= map.OffsetX;
                    nLast.Y -= map.OffsetY;
                    mPath.Add(nLast);
                }
            }


            //从关闭表中得到路径
            iDelayOut = 0;

            while (iLast > 0 && iDelayOut < arrClose.Count && iDelayOut < iMaxStep)
            {
                NPoint nE = arrClose.Get(iLast);

                if (!nE.bSucess)
                    continue;

                iLast = arrClose.FindParent(nE);
                iDelayOut++;
                if (iLast > 0)
                {
                    NPoint nE2 = arrClose.Get(iLast);
                    nE2.X -= map.OffsetX;
                    nE2.Y -= map.OffsetY;
                    mPath.Add(nE2);
                    //Debug.DrawLine(new Vector3(nE.X - map.OffsetX, 1, nE.Y - map.OffsetY), new Vector3(nE2.X, 1, nE2.Y)); 
                    //Debug.Break();
                }
            }

            return mPath;

        }


        /// <summary>
        /// 得到周围的格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></return
        public void CalcAroundPoint(NPoint nParent)
        {
            List<NPoint> list = new List<NPoint>();

            int _x = (int)nParent.X;
            int _y = (int)nParent.Y;

            NPoint t = GetPoint(_x, _y - 1, nParent);
            NPoint r = GetPoint(_x + 1, _y, nParent);
            NPoint b = GetPoint(_x, _y + 1, nParent);
            NPoint l = GetPoint(_x - 1, _y, nParent);

            NPoint tl = GetPoint(_x - 1, _y - 1, nParent);
            NPoint rt = GetPoint(_x + 1, _y - 1, nParent);
            NPoint br = GetPoint(_x + 1, _y + 1, nParent);
            NPoint lb = GetPoint(_x - 1, _y + 1, nParent);


            if (t.bSucess)
                list.Add(t);
            if (r.bSucess)
                list.Add(r);
            if (b.bSucess)
                list.Add(b);
            if (l.bSucess)
                list.Add(l);

            //检查拐角 
            //if (t.G == 0 || l.G == 0) 
            //    tl.F = 200;

            //if (t.G == 0 || r.G == 0)
            //    rt.F = 200;

            //if (b.G == 0 || r.G == 0)
            //    br.F = 200;

            //if (l.G == 0 || b.G == 0)
            //    lb.F = 200;  


            if (t.G == 0 || l.G == 0)
                tl.bSucess = false;

            if (t.G == 0 || r.G == 0)
                rt.bSucess = false;

            if (b.G == 0 || r.G == 0)
                br.bSucess = false;

            if (l.G == 0 || b.G == 0)
                lb.bSucess = false;

             
            if (tl.bSucess)
                list.Add(tl);  
             
            if (rt.bSucess)
                list.Add(rt); 
             
            if (br.bSucess)
                list.Add(br); 
             
            if (lb.bSucess)
                list.Add(lb); 

            for (int i = 0; i < list.Count; i++)
            {
                NPoint n1 = list[i];
                if (n1.G > 0 && !CheckClosePoint(n1))
                {
                    int iIndex = CheckOpenPoint(ref n1, nParent);
                    if (iIndex >= 0)
                    {
                        if (n1.G > nParent.G)
                        {
                            arrOpen.SetParent(iIndex, (int)nParent.X, (int)nParent.Y);
                        }
                    }
                    else
                    {
                        arrOpen.Add(n1);
                    }
                }
            }

        }



        /// <summary>
        /// 检查开放表
        /// </summary>
        /// <param name="npoint"></param>
        /// <param name="nParent"></param>
        /// <returns></returns>
        public int CheckOpenPoint(ref NPoint npoint, NPoint nParent)
        {
            return arrOpen.Find(npoint);
        }





        /// <summary>
        /// 检查是否存在于关闭表
        /// </summary>
        /// <param name="npoint"></param>
        /// <returns></returns>
        public bool CheckClosePoint(NPoint npoint)
        {
            if (arrClose.Find(npoint) > -1)
            {
                return true;
            }
            return false;
        }



        /// <summary>
        /// 得到地图点
        /// 判断是否超过边界, 并计算H G F 值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nParent"></param>
        /// <returns></returns>
        private NPoint GetPoint(int x, int y, NPoint nParent)
        {
            NPoint np;
            np.X = x;
            np.Y = y;
            np.ParentX = nParent.X;
            np.ParentY = nParent.Y;
            np.bSucess = true;
            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height || map.bitMap[x, y] != 0  || map.bitDynamic[x, y] != 0)
            {
                NPoint nd = nParent;
                nd.G = 0;
                nd.bSucess = false;
                return nd;
            }
            else
            {
                np.H = (int)Mathf.Abs(nEnd.X - np.X) + (int)Mathf.Abs(nEnd.Y - np.Y) * 10;
                np.G = CalcG(x, y, nParent);
                np.F = np.H + np.G;
                np.bSucess = true;
                return np;
            }
        }






        public const int OBLIQUE = 14;
        public const int STEP = 10;

        private int CalcG(int x, int y, NPoint nParent)
        {
            int G = 5;// (Mathf.Abs(nParent.X - x) + Mathf.Abs(nParent.Y - y)) == 2 ? STEP : OBLIQUE; 
            return G;
        }









        /// <summary>
        /// 容器
        /// </summary>
        public class ArrayContainer
        {
            public List<NPoint> listPoint = new List<NPoint>();

            public int Count
            {
                get { return listPoint.Count; }
            }
            /// <summary>
            /// 添加节点
            /// </summary>
            /// <param name="np"></param>
            public void Add(NPoint np)
            {
                if(np.bSucess)
                    listPoint.Add(np);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="np"></param>
            /// <param name="npParent"></param>
            /// <returns></returns>
            public int Find(NPoint np)
            {
                for (int i = 0; i < listPoint.Count; i++)
                {
                    if (np.X == listPoint[i].X && np.Y == listPoint[i].Y)
                    {
                        return i;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="np"></param>
            /// <returns></returns>
            public int FindParent(NPoint np)
            {
                for (int i = 0; i < listPoint.Count; i++)
                {
                    if (np.ParentX == listPoint[i].X && np.ParentY == listPoint[i].Y)
                    {
                        return i;
                    }
                }
                return -1;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="i"></param>
            /// <param name="iParentX"></param>
            /// <param name="iParentY"></param>
            public void SetParent(int i, int iParentX, int iParentY)
            {
                NPoint n = listPoint[i];
                n.ParentX = iParentX;
                n.ParentY = iParentY;
                listPoint[i] = n;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public NPoint Get(int index)
            {
                if (index < 0 || index >= listPoint.Count)
                {
                    NPoint nd;
                    nd.X = nd.Y = nd.ParentX = nd.ParentY = 0;
                    nd.H = nd.G = nd.F = 0;
                    nd.bSucess = false;
                    return nd;
                }
                else
                {
                    NPoint n = listPoint[index];
                    return n;
                }

            }


            /// <summary>
            /// 删除
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public void Remove(NPoint npoint)
            {
                int i = Find(npoint);
                if (i > -1)
                {
                    listPoint.RemoveAt(i);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Clear()
            {
                listPoint.Clear();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public NPoint GetMin()
            {
                int index = 0;
                int iF = listPoint[0].F;
                for (int i = 1; i < listPoint.Count; i++)
                {
                    if (iF > listPoint[i].F)
                    {
                        iF = listPoint[i].F;
                        index = i;
                    }
                }
                return listPoint[index];
            }


        }



    }



}