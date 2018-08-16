using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KGFrame;


/*
 *  异步资源读取类
 * 
 * 
 */



namespace KGFrame
{

    /// <summary>
    /// 资源异步读取类
    /// </summary>
    public class CAsyncLoader : MonoBehaviour
    {

        public enum FileType
        {
            Model = 0,
            Text = 1,
        }

        private float fTime;
        private AsyncOperation async;
        private float fProgress = 0; //Range (0 - 1) 
        private bool bIsLoading = false;
        private FileType type;
        public float fProgres { get { return fProgress; } }
        public string strPath;
        public WWW mWWW;
        public Callback<CAsyncLoader> funCompleteCallBack;
        public Callback<CAsyncLoader> funProcessCallBack;
        public int iVersion = 1;                            //资源版本
        public bool bFromLocal = false;                     // 是否是读取本地文件
        public object[] obj_loaded;                         //
        public string text;                                 // 读取的文本

        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="strPath">路径</param>
        /// <param name="funComplete">完成后的回调函数</param> 
        /// <param name="funProcess">过程回调函数</param> 
        public static void Load(string strPath, int iVer, FileType type, Callback<CAsyncLoader> funComplete, Callback<CAsyncLoader> funProcess = null)
        {   
            GameObject mObj = new GameObject("AsyncLoader");
            CAsyncLoader mLoader = mObj.AddComponent<CAsyncLoader>();
            mLoader.type = type;
            mLoader.iVersion = iVer;
            mLoader.funProcessCallBack = funProcess;
            mLoader.funCompleteCallBack = funComplete;
            mLoader.bFromLocal = true;
            mLoader.StartLoad(strPath);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }
 

        //读取模型文件    
        IEnumerator LoadModelFromLocal(string path )
        {
            string s = null;
#if UNITY_ANDROID
        s = "jar:file://"+path;    
#elif UNITY_IPHONE
        s = path;    
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            s = "file://" + path;
#endif
            WWW w = new WWW(s);
            yield return w;
            if (w.isDone)
            { 
                obj_loaded = w.assetBundle.LoadAllAssets();
                if (funCompleteCallBack != null) {
                    funCompleteCallBack(this);
                }
            }
        }


        /// <summary>
        /// WWW过程
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadResourceProcess(string path)
        { 
            if (bFromLocal)
            {
                string s = "";
#if UNITY_ANDROID
        s = "jar:file://"+path;    
#elif UNITY_IPHONE
        s = path;    
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
                s = "file://" + path;
#endif
           
                mWWW = new WWW(s);

            } else {
                // WebPlayer下载路径
                // C:\Documents and Settings\Administrator\Local Settings\Application Data\Unity\WebPlayer\Cache   
                mWWW = WWW.LoadFromCacheOrDownload(path, iVersion); 
            } 

            yield return mWWW;

            if (mWWW.isDone)
            { 
                if (type == FileType.Text)
                {
                    text = mWWW.text;
                }
                if (type == FileType.Model)
                {
                    obj_loaded = mWWW.assetBundle.LoadAllAssets();
                }

                if (funCompleteCallBack != null)
                {
                    funCompleteCallBack(this);
                }
                
                //mWWW.assetBundle.Unload(false);
            }
            else
            { 
                Debug.LogError("Load Resource Error : " + mWWW.error + "   " + path);
            }

            Destory(1);

        }




        /// <summary>
        /// 开始读取
        /// </summary>
        /// <param name="_strPath"></param>
        public void StartLoad(string _path)
        { 
            strPath = _path;
    
            if (strPath.Substring(0, 4).ToLower() == "http")
            {
                bFromLocal = false;
            }
            else
            {
                bFromLocal = true;
            } 
            StartCoroutine(LoadResourceProcess(strPath));
            //StartCoroutine(LoadModelFromLocal(strPath)); 
        }

        /// <summary>
        /// Process Update
        /// </summary>
        void LateUpdate()
        {
            if (bIsLoading)
            {
                if (this.mWWW != null && !this.mWWW.isDone)
                {
                    fProgress = this.mWWW.progress;
                }

                if (null != funProcessCallBack)
                    funProcessCallBack(this);
            }
        }


        /// <summary>
        /// Destory
        /// </summary>
        public void Destory(float fTime = 0)
        { 
            if (null != mWWW && type == FileType.Model)
            {
                mWWW.assetBundle.Unload(false);
            }

            if (null != mWWW)
            { 
                this.mWWW = null;
            }
            GameObject.Destroy(this.gameObject, fTime);
        }

    }




}
