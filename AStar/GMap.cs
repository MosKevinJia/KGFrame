
using System;
using System.Collections.Generic;
using UnityEngine;


namespace KGFrame
{

     /// <summary>
     /// 当前场景唯一的地图
     /// </summary>
    public class GMap
    {
        // sbyte  8位有符号整数   -128 to 127
        //  0  = 可以移动
        //  -1 = 不可以移动
        //  1  = 障碍物

        public static GMap instance = null;

        public string obstacleTag = "obstacle";
        public string groundTag = "Ground";
        public sbyte[,] bitMap;      //地图数据
        public sbyte[,] bitDynamic;  //动态物体

        public int mask_wall, mask_def, mask_ground;


        public int Width
        {
            get { return iWidth; }
        } 

        public int Height
        {
            get { return iHeight; }
        }

        public int OffsetX
        {
            get { return iOffsetX; }
        }

        public int OffsetY
        {
            get { return iOffsetY; }
        }
         

        private int iOffsetX;
        private int iOffsetY;
        private int iWidth;
        private int iHeight;
        private Ray ray;
        private RaycastHit hitInfo;
         
        public GMap()
        {
            // mask_wall = LayerMask.NameToLayer("Wall");
            // mask_def = LayerMask.NameToLayer("Default");
            mask_ground = 11;// LayerMask.NameToLayer("Ground");
            instance = this;
        }
          
        //public GMap(int _Width, int _Height)
        //{
        //    InitializeArray(_Width, _Height, 0);
        //} 


        /// <summary>
        /// 初始化数组
        /// </summary>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <param name="val"></param>
        private void InitializeArray(int _w, int _h, sbyte val)
        {
            bitMap = new sbyte[_w, _h];
            bitDynamic = new sbyte[_w, _h];

            for (int i = 0; i < _h; i++)
                for (int j = 0; j < _w; j++)
                {
                    bitMap[i, j] = val;
                    bitDynamic[i, j] = val;
                } 
        }
        


        /// <summary>
        /// 导入外部地图数据
        /// </summary>
        /// <param name="data"></param>
        public void ImportMapData(ref sbyte[,] data, int _offsetX = 0, int _offsetY = 0)
        {
            iOffsetX = _offsetX;
            iOffsetY = _offsetY;

            iWidth = data.GetLength(0);
            iHeight = data.GetLength(1);

            bitMap = new sbyte[iWidth, iHeight];
            bitDynamic = new sbyte[iWidth, iHeight];   //值默认为0

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    bitMap[x, y] = data[x, y];
                }
            }

            GMap.instance = this;

        }







        /// <summary>
        /// 初始化场景地图
        /// </summary>
        public void InitSceneMap(int _iX = -200, int _iY = -200, int _iWidth = 400, int _iHeight = 400)
        {

            iOffsetX = 0;
            iOffsetY = 0;
            iWidth = _iWidth;
            iHeight = _iHeight;

            if (_iX < 0)  iOffsetX = Math.Abs(_iX); 
            if (_iY < 0)  iOffsetY = Math.Abs(_iY);
            if (_iX > 0)  iOffsetX = -_iX;
            if (_iY > 0)  iOffsetY = -_iY;

            InitializeArray(iWidth, iHeight, 0); 
 
            for (int y = _iY; y < iHeight + _iY; y++)
            {
                for (int x = _iX; x < iWidth + _iX; x++)
                {
                    ray = new Ray(new Vector3(x, 5, y), Vector3.down * 10);
                    Debug.DrawLine(new Vector3(x, 3, y), new Vector3(x, -1, y));
                    //Debug.Break();
                 
                    if (!Physics.Raycast(ray, out hitInfo, 10, ( 1 << mask_def)))
                    {
                        //GameObject gameObj = hitInfo.collider.gameObject;
                        //if (gameObj.tag == obstacleTag)
                        //{
                            bitMap[x + iOffsetX, y + iOffsetY] = 1 ; 
                        //}
                    }
                    
                }
            }

            GMap.instance = this;

        }



        /// <summary>
        /// 得到map值 Vector3
        /// </summary>
        /// <param name="vPos"></param>
        /// <returns></returns>
        public int GetMap(Vector3 vPos)
        {
            int x = Mathf.RoundToInt(vPos.x);
            int y = Mathf.RoundToInt(vPos.z);
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return -1;
            else
                return bitMap[x, y]; 
        }


        /// <summary>
        /// 得到map值 Vector2
        /// </summary>
        /// <param name="vPos"></param>
        /// <returns></returns>
        public int GetMap(Vector2 vPos)
        {
            int x = Mathf.RoundToInt(vPos.x);
            int y = Mathf.RoundToInt(vPos.y); 
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return -1;
            else
                return bitMap[x, y]; 
        }


        /// <summary>
        /// 得到map值 x, y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetMap(int x, int y)
        {
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return -1;
            else
                return bitMap[x, y]; 
        }



        /// <summary>
        /// 设置动态地图
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="v"></param>
        public void SetDynamicMap(int x, int y, int v)
        {
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return;

            bitDynamic[x, y] = (sbyte)v;
        }

        /// <summary>
        /// 添加动态障碍
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="v"></param>
        public void AddDynamicMap(int x, int y, int v)
        {
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return;

            bitDynamic[x, y] += (sbyte)v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetDynamicMap(int x, int y)
        {
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return -1;
            else
                return bitDynamic[x, y];
        }






        /// <summary>
        /// 设置动态地图
        /// 0 = 空
        /// 1 = 有
        /// </summary>
        /// <param name="vPos"></param>
        /// <param name="v"></param>
        public void SetDynamicMap(Vector3 vPos, int v)
        {
            int x = Mathf.RoundToInt(vPos.x);
            int y = Mathf.RoundToInt(vPos.z);
            x += iOffsetX;
            y += iOffsetY;

            if (x < 0 || y < 0 || x >= iWidth || y >= iHeight)
                return;
            else
                bitDynamic[x, y] = (sbyte)v; 

            //bitDynamic[Mathf.RoundToInt(vPos.x) + iOffsetX, Mathf.RoundToInt(vPos.z) + iOffsetY] = (sbyte)v;
        }





        /// <summary>
        /// 得到随机空余位置
        /// -复杂地图 不能完全保证能得到位置
        /// </summary>
        public Vector3 GetRandomPosition()
        {
            Vector3 vPos = Vector3.zero;

            for (int i = 0; i < 100; i++)
            { //最多尝试100次, 一般1,2次就能得到
                int x = UnityEngine.Random.Range(0, iWidth);
                int y = UnityEngine.Random.Range(0, iHeight);
                int p = bitMap[x, y];
                if (p == 0)
                {
                    vPos = new Vector3(x - iOffsetX, 0, y - iOffsetY);
                    break;
                }
            }
            return vPos;
        }


        /// <summary>
        /// 得到周围的位置, 排除1个位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ext1x"></param>
        /// <param name="ext1y"></param>
        /// <returns></returns>
        public Vector3 GetAroundPositionExclude(int x, int y, int ext1x, int ext1y)
        {
            bool t = GetCanMovePoint(x, y - 1) && x != ext1x && y - 1 != ext1y;
            bool r = GetCanMovePoint(x + 1, y) && x+1 != ext1x && y  != ext1y;
            bool b = GetCanMovePoint(x, y + 1) && x != ext1x && y + 1 != ext1y;
            bool l = GetCanMovePoint(x - 1, y) && x-1 != ext1x && y  != ext1y;

            //bool tl = GetCanMovePoint(x - 1, y - 1);
            //bool rt = GetCanMovePoint(x + 1, y - 1);
            //bool br = GetCanMovePoint(x + 1, y + 1);
            //bool lb = GetCanMovePoint(x - 1, y + 1); 

            List<Vector3> v = new List<Vector3>();

            if (t)
                v.Add(new Vector3(x, 0, y - 1 - 0.5f));
            else if (r)
                v.Add(new Vector3(x + 1 + 0.5f, 0, y));
            else if (b)
                v.Add(new Vector3(x, 0, y + 1 + 0.5f));
            else if (l)
                v.Add(new Vector3(x - 1 - 0.5f, 0, y));
            //else if (tl)
            //    v.Add(new Vector3(x - 1, 0, y - 1));
            //else if (rt)
            //    v.Add(new Vector3(x + 1, 0, y - 1));
            //else if (br)
            //    v.Add(new Vector3(x + 1, 0, y + 1));
            //else if (lb)
            //    v.Add(new Vector3(x - 1, 0, y + 1));

            if (v.Count > 0)
            {
                return v[UnityEngine.Random.Range(0, v.Count)];
            }
            return new Vector3(x, 0, y);

        }


        /// <summary>
        /// 得到周围的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 GetAroundPosition(int x, int y)
        {
            bool t = GetCanMovePoint(x, y - 1);
            bool r = GetCanMovePoint(x + 1, y);
            bool b = GetCanMovePoint(x, y + 1);
            bool l = GetCanMovePoint(x - 1, y);

            //bool tl = GetCanMovePoint(x - 1, y - 1);
            //bool rt = GetCanMovePoint(x + 1, y - 1);
            //bool br = GetCanMovePoint(x + 1, y + 1);
            //bool lb = GetCanMovePoint(x - 1, y + 1); 

            List<Vector3> v = new List<Vector3>(); 
           
            if(t)
                v.Add( new Vector3(x, 0, y - 1 - 0.5f));
            else if(r)
                v.Add(new Vector3(x + 1 + 0.5f, 0, y));
            else if (b)
                v.Add( new Vector3(x, 0, y + 1 + 0.5f));
            else if (l)
                v.Add(new Vector3(x - 1 - 0.5f, 0, y));
            //else if (tl)
            //    v.Add(new Vector3(x - 1, 0, y - 1));
            //else if (rt)
            //    v.Add(new Vector3(x + 1, 0, y - 1));
            //else if (br)
            //    v.Add(new Vector3(x + 1, 0, y + 1));
            //else if (lb)
            //    v.Add(new Vector3(x - 1, 0, y + 1));

            if (v.Count > 0)
            {
                return v[UnityEngine.Random.Range(0, v.Count)];
            }
            return new Vector3(x ,0, y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool GetCanMovePoint(int x, int y)
        {
            x += iOffsetX;
            y += iOffsetY; 

            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            if (bitMap[x, y] == 0 && bitDynamic[x, y] == 0)
                return true;

            return false;
        }


    }


}