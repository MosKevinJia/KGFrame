using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KGFrame; 


/*
 *  KGFrame 
 *  
 *  Editor 2017-11-27
 *  
 *  组件:
 *      CResStore ------------ 资源池
 *      CResLoader ----------- 异步加载类
 *      CKUI ----------------- UI
 *          CKUIWindow      
 *          CKUIScrollBar 
 *      CAssetEditor --------- 资源工具
 *      CData ---------------- 本地数据操作
 *          CSVLoader
 *          XMLLoader
 *          JsonLoader
 * 
 *      CNet ----------------- 网络操作
 *      CLua ----------------- Lua脚本    
 * 
 * 
 *      ------ Game Base Class ------
 *      CActor          //角色类          
 *      CSkill          //技能类
 *      CEffect         //特效类
 *      CSound          //音效类
 *      CTrigger        //触发器
 *      CGEvent         //游戏事件
 *      
 *      
 * 
 *      ------ Other ----------------
 *      CCameraBase     //摄像机操作
 *      CPlatform       //平台SDK
 *      Spirit          //
 *      
 *      
 */


namespace KGFrame
{

    public delegate void ControlEvent(CUIControl control);
    public delegate void Callback();
    public delegate void Callback<T>(T arg1);
    public delegate void Callback<T, U>(T arg1, U arg2);
    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);


    /// <summary>
    /// KGFrame 工具类
    /// </summary>
    public class Util
    {

        public static Spirit __spgame = null;
        const string objName = "Game";
       

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            GameObject obj = GameObject.Find(objName);
            if (!obj)
            {
                obj = new GameObject(objName);
                obj.isStatic = true;
            } 
            __spgame = obj.GetComponent<Spirit>(); 
            if (__spgame == null)
            {
                __spgame = obj.AddComponent<Spirit>();
            }
        }


        /// <summary>
        /// 时间差(秒)
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(秒)</returns>
        public static double DateSpanSec(long dateBegin, long dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin);
            TimeSpan ts2 = new TimeSpan(dateEnd);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return ts3.TotalMilliseconds;
        }


        /// <summary>
        /// 时间差(TimeSpan)
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(TimeSpan)</returns>
        public static TimeSpan DateSpan(long dateBegin, long dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin);
            TimeSpan ts2 = new TimeSpan(dateEnd); 
            return ts1.Subtract(ts2).Duration();
        }

        /// <summary>
        /// 时间差(小时)
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(小时)</returns>
        public static double DateSpanHours(long dateBegin, long dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin);
            TimeSpan ts2 = new TimeSpan(dateEnd);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return ts3.TotalHours;
        }


        /// <summary>
        /// 时间差(天)
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(天)</returns>
        public static double DateSpanDay(long dateBegin, long dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin);
            TimeSpan ts2 = new TimeSpan(dateEnd);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return ts3.TotalDays;
        }



        /// <summary>
        /// color 转换hex
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color color)
		{
			int r = Mathf.RoundToInt(color.r * 255.0f);
			int g = Mathf.RoundToInt(color.g * 255.0f);
			int b = Mathf.RoundToInt(color.b * 255.0f);
			int a = Mathf.RoundToInt(color.a * 255.0f);
			string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
			return hex;
		}

		/// <summary>
		/// hex转换到color
		/// </summary>
		/// <param name="hex"></param>
		/// <returns></returns>
		public static Color HexToColor(string hex)
		{
			byte br = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
			byte bg = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
			byte bb = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f; 
  
            if (hex.Length == 9)
            { 
                byte cc = byte.Parse(hex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);
                float a = cc / 255f;
                return new Color(r, g, b, a);
            }
            else
            {
                return new Color(r, g, b);
            }  
			
		}


        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }



        /// <summary>
        /// 加载资源
        /// </summary>
        /// <returns>The resource.</returns>
        /// <param name="strPath">String path.</param>
        /// <param name="name">Name.</param>
        /// <param name="parent">Parent.</param>
        public static GameObject GetRes(string strPath, string name = null,  Transform parent = null )
		{
			GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(strPath));
			if (name != null & name != "") {
				obj.name = name;
			}
			if (parent != null)
			{
				obj.transform.parent = parent;
			}
			return obj;
		}
 

		/// <summary>
		/// Gets the res.
		/// </summary>
		/// <returns>The res.</returns>
		/// <param name="strPath">String path.</param>
		/// <param name="name">Name.</param>
		/// <param name="parent">Parent.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetRes<T>(string strPath, string name = null, Transform parent = null )where T : MonoBehaviour, new()
		{
			return GetRes (strPath, name, parent).AddComponent<T>();   
		}


        /// <summary>
        /// 删除所有子节点
        /// </summary>
        /// <param name="tParent"></param>
        public static void RemoveAllChild(Transform tParent)
        {
            for (int i = tParent.childCount-1; i >= 0; i--)
            {
                GameObject.Destroy(tParent.GetChild(i).gameObject);
            }
 
        }




        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objChild"></param>
        /// <returns></returns>
        public static List<GameObject> SearchAll(string name, GameObject objChild)
        {
            List<GameObject> list = new List<GameObject>();

            search_all(name, objChild, ref list);
             
            return list;
        }


        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objChild"></param>
        /// <returns></returns>
        private static void search_all(string name, GameObject objChild, ref List<GameObject> list)
        {
            if (objChild == null)
            {
                return;
            }

            if (objChild.name.IndexOf(name) >= 0)
            {
                list.Add(objChild);
                return;
            }

            foreach (Transform child in objChild.transform)
            {
                if (child.name.IndexOf(name) >= 0)
                {
                    list.Add(child.gameObject);

                }else if (child.childCount > 0)
                {
                    search_all(name, child.gameObject, ref list);
                }
            }

        }



        /// <summary>
        /// 遍历查找子节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objChild"></param>
        /// <returns></returns>
        public static GameObject FindAll(string name, GameObject objChild)
        {
            if (objChild == null)
            {
                return null;
            }

            if (objChild.name == name)
            {
                return objChild;
            }

            foreach (Transform child in objChild.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }

                GameObject obj = FindAll(name, child.gameObject); 
                if (obj!=null && obj.name == name)
                {
                    return obj;
                }
            }

            return null;
        }


        /// <summary>
        /// 世界坐标转UI坐标. (血条)
        /// </summary>
        /// <param name="objTarget"></param>
        /// <returns></returns>
        public static Vector2 WorldToUIPoint(GameObject objTarget)
        {
            Vector2 pos;
            Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Camera.main.WorldToScreenPoint(objTarget.transform.position),
            canvas.worldCamera, out pos);
            return pos;
        }


        /// <summary>
        /// 四元数朝向
        /// Example:
        ///         earth.rotation = Util.LookAt(sun, earth);
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static Quaternion LookAt(Transform t1, Transform t2)
        {
            Vector3 relativePos = t1.position - t2.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            return rotation;
        }



        /// <summary>
        /// 获得相对于tForm一定角度距离的位置.
        /// 可以用于计算围绕
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetAngleRangePostion(Vector3 form_pos, float angleY,  float range)
        { 
            Quaternion rotation = Quaternion.Euler(0, angleY, 0); // 角度
            return rotation * new Vector3(range, 0f, 0f) + form_pos; //距离    ;  
        }


        /// <summary>
        /// 两个物体相对角度
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetAngle(Transform from, Transform to)
        { 
            Vector3 targetDir = from.position - to.position; // 目标坐标与当前坐标差的向量
            float angle = Vector3.Angle(targetDir, -Vector3.right); // 返回当前坐标与目标坐标的角度 
            if (to.position.z > from.position.z)
            {
                angle = 360 - angle;
            }
            return angle; 
        }


        public static float GetAngle(Vector3 from, Vector3 to)
        {
            Vector3 targetDir = from - to; // 目标坐标与当前坐标差的向量
            float angle = Vector3.Angle(targetDir, -Vector3.right); // 返回当前坐标与目标坐标的角度 
            if (to.z > from.z)
            {
                angle = 360 - angle;
            }
            return angle;
        }


        /// <summary>
        /// 平滑朝向物体
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <param name="fSpeed"></param>
        /// <returns></returns>
        public static void SmoothLookUpdate(Transform self, Transform target, float fSpeed = 2.5f)
        {
            Quaternion TargetRotation = Quaternion.LookRotation(target.position - self.position, Vector3.up);
            self.rotation = Quaternion.Slerp(self.rotation, TargetRotation, Time.deltaTime * fSpeed);
        }



        /// <summary>
        /// 测试: 鼠标选择
        /// </summary>
        /// <returns></returns>
        public static RaycastHit OnSampleMouseClick()
        {
            RaycastHit hit = new RaycastHit();
            if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0) == true) && GUIUtility.hotControl == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    return hit;
                }
            }
            return hit;
        }


        #region DelayCall

        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="cb"></param>
        public static void Delay(Callback cb, float fTime)
        {
            if (!__spgame) 
                Init(); 
            __spgame.StartCoroutine(Util.delay(cb, fTime)); 
        }
        public static IEnumerator delay(Callback cb, float f)
        {
            yield return new WaitForSeconds(f);
            cb();
        }
         

        public static  void Delay(Callback<String> cb, String param, float fTime)
        {
            if (!__spgame) 
                Init();

            Debug.Log(__spgame);
            __spgame.StartCoroutine(Util.delay(cb, param, fTime)); 
        }
        public static IEnumerator delay(Callback<String> cb, String param, float f)
        {
            yield return new WaitForSeconds(f);
            cb(param);
        }

        public static void Delay(Callback<int> cb, int param, float fTime)
        {
            if (!__spgame)
                Init();
            __spgame.StartCoroutine(Util.delay(cb, param, fTime));
        }

        public static void Delay(Callback<int, string> cb, int param, string strParam, float fTime)
        {
            if (!__spgame)
                Init();
            __spgame.StartCoroutine(Util.delay(cb, param, strParam, fTime));
        }

        public static void Delay(Callback<float, string> cb, float param, string strParam, float fTime)
        {
            if (!__spgame)
                Init();
            __spgame.StartCoroutine(Util.delay(cb, param, strParam, fTime));
        }

        private static IEnumerator delay(Callback<int, string> cb, int param, string strParam, float fTime)
        {
            yield return new WaitForSeconds(fTime);
            cb(param, strParam);
        }

        private static IEnumerator delay(Callback<float, string> cb, float param, string strParam, float fTime)
        {
            yield return new WaitForSeconds(fTime);
            cb(param, strParam);
        }

        public static void Delay(Callback<float> cb, float param, float fTime)
        {
            if (!__spgame)
                Init();
            __spgame.StartCoroutine(Util.delay(cb, param, fTime));
        }
        public static IEnumerator delay(Callback<float> cb, float param, float f)
        {
            yield return new WaitForSeconds(f);
            cb(param);
        }

        public static void Delay<T>(Callback<T> cb, T param, float fTime)
        {
            if (!__spgame)
                Init();
            __spgame.StartCoroutine(Util.delay(cb, param, fTime));
        }
        public static IEnumerator delay<T>(Callback<T> cb, T param, float f)
        {
            yield return new WaitForSeconds(f);
            cb(param);
        }

        #endregion

    }


}