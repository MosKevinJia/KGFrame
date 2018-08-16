using UnityEngine;
using System; 
using System.Collections;
using System.Collections.Generic;
using KGFrame;

/*
 * KGFrame 资源池类 (全局单列) by Kevin.Jia
 * 
 * ***********本类适合创建大量重复回收的GameObject, 单个大的Prefab不适合使用.***************
 * 
 * Example:
 *          int iType = 101;  //类型Type
 * 添加资源 
 *          ResStore.Add(iType, Resources.Load("Character/Actor01") as GameObject);  //添加到缓存
 *          
 * 得到资源 
 *          CActor m_hunter = ResStore.Get<CActor>(iType, false);          // 可以被回收
 *          CActor m_hunter = ResStore.Get<CActor>(iType, true);    // 用户自己管理
 *          
 * 回收资源 
 *          (自己管理的变量, 引用完必须设为null)
 *          ResStore.Recycle((Spirit)m_hunter);     //
 *          m_hunter = null;                        //释放
 *        
 * 删除单个资源
 *          ResStore.Remove((Spirit)m_hunter);
 *          ResStore.Remove((Spirit)m_hunter,true); //强制删除用户使用的资源
 *          
 * 删除全部资源 
 *          ResStore.RemoveAll(iType);
 *          ResStore.RemoveAll(iType, true);        //强制删除用户使用的资源
 *          
 * 清空缓存资源
 *          ResStore.ClearType(iType);              //删除没有使用的资源
 *                 
 * 删除资源类型
 *          ResStore.DestoryType(iType);            //释放资源
 */
namespace KGFrame
{

	public class ResStore {
        private static readonly ResStore Instance = new ResStore();                                                          // 主动创建实例                                           
        private static Dictionary<int, ResContainer> mStore = new Dictionary<int,ResContainer>();                            // 仓库
        private static int _iInstanceId = 0;
        
        private ResStore(){
        }

        public static ResStore GetInstance()
        {
            return Instance;
        }

        public static int NEW_ID
        {
            get { return ResStore._iInstanceId++; }
        }

        #region Add
        /// <summary>
		///  添加资源
		/// </summary>
        /// <param name="iType"></param>
		/// <param name="obj"></param>
		public static void Add(int iType, GameObject obj){
             
            if (!mStore.ContainsKey(iType))
            {
                mStore.Add(iType, new ResContainer(obj));
            }
            else
            {
				mStore [iType] = new ResContainer (obj);
                Debug.LogWarning("Error : ResourcesType : already exist " + iType);
            }
		}

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="obj"></param>
        public static void Add(string sType, GameObject obj)
        {
            Add(sType.GetHashCode(), obj);
        }

        #endregion


        #region Get
        /// <summary>
        /// 得到资源
        /// </summary>
        /// <typeparam name="T">类型Spirit模版</typeparam>
        /// <param name="iType">类型Id</param>
        /// <param name="bUserUse">是否用户自己管理</param>
        /// <returns></returns>
        public static T Get<T>(int iType, bool bUserUse = false) where T : Spirit, new()
        { 
            if (!mStore.ContainsKey(iType))
            {
                //Debug.LogError("ResStore error :  Cannot find resource type " + iType);
                return null;
            } 

            ResContainer arrContainer = mStore[iType];
			RES_Struct res = arrContainer.GetNewRes();

            if (!res.bAddComponented)
            {
                res.com = (T)res.obj.AddComponent<T>();
                res.com.iInstanceType = iType;
                res.com.iInstanceId = res.iInstanceId; 
                res.bAddComponented = true;
			}
            res.bUserUse = bUserUse;
            res.bUsed = true;
            return (T)res.com;
		}


        /// <summary>
        /// 获得基类 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iType"></param>
        /// <param name="bUserUse"></param>
        /// <returns></returns>
        public static T GetBase<T>(int iType, bool bUserUse = false) where T : Spirit, new()
        {
            //#if UNITY_EDITOR || UNITY_DEBUG
            if (!mStore.ContainsKey(iType))
            {
                Debug.LogError("ResStore error :  Cannot find resource type " + iType);
                return null;
            }
            //#endif

            ResContainer arrContainer = mStore[iType];
            RES_Struct res = arrContainer.GetNewRes();

            res.bUserUse = bUserUse;
            res.bUsed = true;  

            if (!res.bAddComponented)
            {
                res.com = (T)res.obj.GetComponent<T>();
                if (!res.com)
                {
                    res.com = (T)res.obj.AddComponent<T>();
                }
                res.com.iInstanceType = iType;
                res.com.iInstanceId = res.iInstanceId;
                res.bAddComponented = true;
                return (T)res.com;
            }
            else {
                return (T)res.obj.GetComponent<T>(); 
            }
            
        }


        /// <summary>
        /// 获得对象(需要手动删除)
        /// </summary>
        /// <param name="iType"></param>
        /// <returns></returns>
        public static GameObject Get(int iType)
        {
            ResContainer arrContainer = mStore[iType];
            RES_Struct res = arrContainer.GetNewRes(); 
            res.bUserUse = true;
            res.bUsed = true;
            return res.obj;
        }

        /// <summary>
        /// 获得对象(需要手动删除)
        /// </summary>
        /// <param name="sType"></param>
        /// <returns></returns>
        public static GameObject Get(string sType)
        {
            return Get(sType.GetHashCode());
        }


        #endregion


        #region Recycle
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="sRes"></param>
        public static void Recycle(Spirit sRes)
        { 
            ResContainer arrContainer = mStore[sRes.iInstanceType];
            arrContainer.Recycle(sRes.iInstanceId);
            sRes = null;
        }
        #endregion


        #region Remove
        /// <summary>
        /// 删除单个资源
        /// </summary>
        /// <param name="sRes">资源Spirit</param>
        /// <param name="bRemoveUserUse">是否强制删除用户使用的资源</param>
        public static void Remove(Spirit sRes, bool bRemoveUserUse = false)
        {
            ResContainer arrContainer = mStore[sRes.iInstanceType];
            arrContainer.Remove(sRes.iInstanceId, bRemoveUserUse);
            sRes = null;
        }
        #endregion

        #region RemoveAll
        /// <summary>
        /// 删除全部类型资源
        /// </summary>
        /// <param name="iKey">类型</param>
        public static void RemoveAll(int iKey, bool bRemoveUserUse = false)
        {
            ResContainer arrContainer = mStore[iKey];
            arrContainer.RemoveAll(bRemoveUserUse); 
        }
        #endregion


        #region 
        public static void DestoryType(int iKey)
        {
            ((ResContainer)mStore[iKey]).Destory();
            mStore.Remove(iKey);
        }


        public static void DestoryAllType()
        {
            foreach (int iKey in mStore.Keys)
            {
                DestoryType(iKey);
            }
        }


		
		public static void ClearType(int iKey){
			((ResContainer)mStore [iKey]).Clear ();
		}


		
		public static void ClearAll(){
			foreach (int iKey in mStore.Keys) {
				((ResContainer)mStore [iKey]).Clear ();
			}
		}
		
		public static int GetCount(int iKey){
			return ((ResContainer)mStore [iKey]).GetCount();
		}
		
		public static int GetCountAll(){
			int icount = 0;
			foreach (int iKey in mStore.Keys) {
				icount+= ((ResContainer)mStore [iKey]).GetCount();
			}
			return icount;
		}
		
		
		public static void SetActive(int iKey, bool b=false){
			((ResContainer)mStore [iKey]).SetActiveAll (b);
		}
		
		public static void SetActiveAll(bool b=false){
			foreach (int iKey in mStore.Keys) {
				SetActive(iKey, b);
			}
        }
        #endregion
    }


    #region RES_Struct
    /*
     * 资源容器结构
     */
    public class RES_Struct
    {
        public GameObject   obj;
        public Spirit       com;
        public int          iInstanceId;
        public bool         bAddComponented;
        public bool         bUsed;
        public bool         bUserUse;
    }
    #endregion 


    #region ResContainer
    /*
	 *  资源容器
	 * 
	 */
	internal class ResContainer{ 

        public      ArrayList       list = new ArrayList();

		public ResContainer(GameObject obj){ 
            AddObject(obj); 
		}


        public RES_Struct AddObject(GameObject obj, bool bInstantiate = false)
        {
            RES_Struct rl = new RES_Struct();
            rl.obj = bInstantiate ? MonoBehaviour.Instantiate(obj) as GameObject : obj;
            rl.obj.name = obj.name;
            rl.obj.SetActive(false);
            rl.bUsed = false;
            rl.bAddComponented = false;
            rl.iInstanceId = ResStore.NEW_ID;
            list.Add(rl);
            return rl;
        }


        public RES_Struct GetNewRes()
        { 
            RES_Struct res = null;
            GetUnUseRes(ref res);
            if (null == res)
            {
                res = AddObject(((RES_Struct)list[0]).obj, true); 
			}
            return res;
		}


        public void Recycle(int iId)
        {
            for (int i = 1; i < list.Count; i++)
            {
                RES_Struct res = (RES_Struct)list[i];
                if (res.iInstanceId == iId)
                {
                    SetUnUse(ref res);
                    return;
                }
            } 
        }

        public void Remove(int iId, bool bRemoveUserUse = false)
        {
            for (int i = 1; i < list.Count; i++)
            {
                RES_Struct res = (RES_Struct)list[i];
                if (res.iInstanceId == iId && bRemoveUserUse)
                {   
                    GameObject.Destroy(res.obj);
                    list.RemoveAt(i);
                    return;
                }
            }
        }


        public void SetUnUse(ref RES_Struct res)
        {
            res.bUsed = false;
            res.obj.SetActive(false);
        }
		
		

        public void  GetUnUseRes(ref RES_Struct res)
        {
			for (int i=1; i<list.Count; i++) { 
                RES_Struct resn = (RES_Struct)list[i];
                if (!resn.bUsed && !resn.bUserUse)
                {
                    res = resn;
                    return;
				}
			} 
		}


		
		public void Clear(){
			for (int i=1; i<list.Count; i++) {
                RES_Struct res = (RES_Struct)list[i];
                if (!res.bUsed && !res.bUserUse)
                {
                    MonoBehaviour.Destroy(((RES_Struct)list[i]).obj);
					list.RemoveAt(i); 
					i--;
				}
			}
		}


		
		public void RemoveAll(bool bRemoveUserUse = false){
            int iBegin = 1;
            for (int i = list.Count - 1; i >= iBegin; i--)
            {
                RES_Struct res = (RES_Struct)list[i];
                if (bRemoveUserUse || !res.bUserUse)
                {
                    MonoBehaviour.Destroy(res.obj);
                    list.RemoveAt(i);
                }
			}
		}


        public void Destory()
        {
            int iBegin = 1;
            for (int i = list.Count - 1; i >= iBegin; i--)
            {
                MonoBehaviour.Destroy(((RES_Struct)list[i]).obj);
            } 
 
            iBegin = 0;
            ((RES_Struct)list[iBegin]).obj = null;
            list.RemoveRange(iBegin, list.Count - 1);
            Resources.UnloadUnusedAssets();
        }


		
		public int GetCount(){
            return list.Count;
		}
		
		public void SetActive(int iIndex, bool b=false){
			((GameObject)list [iIndex]).SetActive (b);
		}
		
		public void SetActiveAll(bool b=false){
			for (int i=1; i<list.Count; i++) {  
				SetActive(i,b);
			}
		}
		
	}

    #endregion

    #region Spirit
    /////////////////
    /// <summary>
    /// 资源基类
    /// </summary>
    public class Spirit : MonoBehaviour
    {
        [System.NonSerialized]
        public int      iInstanceId;                      // Id
        [System.NonSerialized]
        public int      iInstanceType;                    // 类型
    }

    //public interface ISpirit
    //{
    //      int iInstanceId { get; set; }                      // Id 
    //      int iInstanceType { get; set; }                    // 类型 
    //}
    #endregion


}

