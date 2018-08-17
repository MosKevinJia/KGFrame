using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; 

/*
 * KGFrame  UI 类
 * 
 * Editor : 2017-11-27
 * 
 * 界面与游戏逻辑分离, 大幅度减少耦合
 * 使用简单\方便扩展
 * 所有控件统一方法: SetValue   GetValue  SetText  GetText
 * 常用Alpha Scale Tween 特效
 * 极大的减轻UI制作的工作量
 * 
 * 增加i18n 多语言切换
 * 
 * 
 */


/*
 *  要解决UI编程的几个问题:
 *   
 *  1.UI逻辑与UI本身交织在一起,   UI改了, UI逻辑也要改.  复杂的交叉调用到后期就越来越麻烦
 *  2.不同的控件的获得值的方法不一样. 所以UI控件一改, 大量的逻辑代码也要改动.  所以,应该把所有控件的基础方法统一. 
 *   
 * 
 */

namespace KGFrame
{

    /// <summary>
    /// 方向类型
    /// </summary>
    public enum DirType
    {
        Top = 0,
        Right = 1,
        Left = 2,
        Bottom = 3
    }


    public class CUI
    {

        public static GameObject            mMaskPlane;
        public static Canvas                mCanvas;

        public static int                   ___lastSiblingIndex;                     
        public static Transform             ___lastParent;
        public static CUIControl            ___lastControl;
        public static ControlEvent          ___evtMaskOutCallback;

       
        /// <summary>
        /// 初始化类
        /// </summary>
        public static void Init()
        {
        
        }

        /// <summary>
        /// 得到控件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CUIControl Get(GameObject obj, ControlType controlType = ControlType.Button)
        { 
            CUIControl control = obj.GetComponent<CUIControl>();
            if (control == null)
            {
                control = obj.AddComponent<CUIControl>();
                control.Type = controlType;
            }
            return control;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static T Get<T>(GameObject obj, ControlType controlType = ControlType.Button) where T : CUIControl, new()
        {
            T control = obj.AddComponent<T>();
            control.Type = controlType;
            return control;
        }


        /// <summary>
        /// 获得
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static CUIControl Get(string strName, ControlType controlType = ControlType.Button)
        {
            return Get(GameObject.Find(strName), controlType);
        }


        public static T Get<T>(string strName, ControlType controlType = ControlType.Button) where T : CUIControl, new()
        {
            return Get<T>(GameObject.Find(strName), controlType);
        }




        /// <summary>
        /// 从资源中读取
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="controlType"></param>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static CUIControl GetRes(string strPath, Transform canvas = null , ControlType controlType = ControlType.Button )
        {
            GameObject objGameUI = Object.Instantiate(Resources.Load<GameObject>(strPath));
            CUIControl cc = objGameUI.AddComponent<CUIControl>();
            cc.Type = controlType;
            if (canvas == null)
            {
                objGameUI.transform.parent = GameObject.Find("Canvas").transform;
            }
            return cc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strPath"></param>
        /// <param name="canvas"></param>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static T GetRes<T>(string strPath, Transform canvas = null, ControlType controlType = ControlType.Button) where T : CUIControl, new()
        {
            GameObject objGameUI = Object.Instantiate(Resources.Load<GameObject>(strPath));
            T cc = objGameUI.AddComponent<T>();
            cc.Type = controlType;
            if (canvas == null)
            {
                objGameUI.transform.parent = GameObject.Find("Canvas").transform;
            }
            return cc; 
        }






            /// <summary>
            /// 显示
            /// </summary>
            /// <param name="control">Control.</param>
            public static CUIControl Show(CUIControl control, bool bShow = true){

			if (!bShow) {
				Hide (control);
			}

			control.gameObject.SetActive (true);
			Image img =  control.GetComponent<Image>();
			CanvasGroup cgroup = control.GetComponent<CanvasGroup>();
			if (cgroup != null) { 
				cgroup.alpha = 1;
			}
			if (img != null) { 
				img.color =  new Color (img.color.r, img.color.g, img.color.b, 1);
			}
			return control;
		}


		/// <summary>
		/// 隐藏
		/// </summary>
		/// <param name="control">Control.</param>
		public static CUIControl Hide(CUIControl control){ 
            control.gameObject.SetActive (false);
			return control;
		}



		/// <summary>
		/// 设置颜色
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="color">Color.</param>
		public static CUIControl SetColor(CUIControl control, Color color){
			Image img =  control.GetComponent<Image>();
			if (img != null) { 
				img.color = color;
			}
            else
            {
                Text txt = control.GetComponent<Text>();
                if (txt != null)
                {
                    txt.color = color;
                }
            }
            return control;
		}



		/// <summary>
		/// 设置透明度
		/// </summary>
		/// <returns>The alpha.</returns>
		/// <param name="fAlpha">F alpha.</param>
		public static CUIControl SetAlpha(CUIControl control, float fAlpha){
			control.gameObject.SetActive (true);
			CanvasGroup cgroup = control.GetComponent<CanvasGroup>();
			if (cgroup != null) { 
				cgroup.alpha = fAlpha;
			} else {
				Image img = control.GetComponent<Image> ();
				if (img != null) { 
					img.color =  new Color (img.color.r, img.color.g, img.color.b, fAlpha);
                }
                else
                { 
                    Text txt = control.GetComponent<Text>();
                    if (txt != null)
                    {
                        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, fAlpha); 
                    } 
                }
			}  
			return control;
		}






        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static CUIControl FadeIn(CUIControl control, float fTime = 0.5f)
        {
            return Fade(control, 0, 1f, fTime);
        }

        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static CUIControl FadeOut(CUIControl control, float fTime = 0.5f)
        {
            return Fade(control, 1f, 0, fTime);
        }



        /// <summary>
        /// 渐变
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fFrom"></param>
        /// <param name="fTo"></param>
        /// <param name="fTime"></param>
        /// <returns></returns>
        public static CUIControl Fade(CUIControl control, float fFrom, float fTo, float fTime = 0.5f)
        { 

            CanvasGroup cgroup = control.GetComponent<CanvasGroup>();
            if (cgroup != null)
            {
                cgroup.alpha = fFrom;
                control.show = true;
                LeanTween.value(control.gameObject, fFrom, fTo, fTime).setOnUpdate((float val) => {
                    cgroup.alpha = val;
                });
            }
            else
            {
                Image img = control.GetComponent<Image>();
                if (img != null)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, fFrom);
                    control.show = true;
                    LeanTween.value(control.gameObject, fFrom, fTo, fTime).setOnUpdate((float val) => {
                        img.color = new Color(img.color.r, img.color.g, img.color.b, val);
                    });
                }
                else
                {
                    Text txt = control.GetComponent<Text>();
                    if (txt != null)
                    {
                        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, fFrom);
                        control.show = true;
                        LeanTween.value(control.gameObject, fFrom, fTo, fTime).setOnUpdate((float val) => {
                            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, val);
                        });
                    }
                }

            }

            return control;
        }


        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fScaleFrom"></param>
        /// <param name="fScaleTo"></param>
        /// <param name="fTime"></param>
        /// <param name="ltType"></param>
        /// <returns></returns>
        public static CUIControl Scale(CUIControl control, float fScaleFrom = 0.6f, float fScaleTo = 1, float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeOutElastic)
        {
            control.show = true;
            control.transform.localScale = new Vector3(fScaleFrom, fScaleFrom, fScaleFrom);
            LeanTween.scale(control.gameObject, new Vector3(fScaleTo, fScaleTo, fScaleTo), fTime).setEase(ltType);
            return control;
        }


        /// <summary>
        /// 缩放进入
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fTime"></param>
        /// <returns></returns>
        public static CUIControl ScaleIn(CUIControl control,float fScaleFrom = 0.6f,  float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeOutElastic)
        { 
            control.show = true;
            control.transform.localScale = new Vector3(fScaleFrom, fScaleFrom, fScaleFrom);
            LeanTween.scale(control.gameObject, Vector3.one, fTime).setEase(ltType); 
            return control;
        }


        /// <summary>
        /// 缩放出
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fScaleTo"></param>
        /// <param name="fTime"></param>
        /// <param name="ltType"></param>
        /// <returns></returns>
        public static CUIControl ScaleOut(CUIControl control, float fScaleTo = 0.6f, float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeInExpo)
        {
            LeanTween.scale(control.gameObject, new Vector3(fScaleTo, fScaleTo, fScaleTo), fTime).setEase(ltType).setOnComplete(()=>{
                control.show = false;
            });
            return control;
        }



        /// <summary>
        /// 滑动
        /// </summary>
        /// <param name="control"></param>
        /// <param name="vIn"></param>
        /// <param name="stType"></param>
        /// <param name="fTime"></param>
        /// <param name="ltType"></param>
        /// <returns></returns>
        public static CUIControl Slide(CUIControl control, Vector3 vFrom, Vector3 vTo, float fAlphaForm = 0, float fAlphaTo = 1, float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeOutCirc)
        {
            control.show = true;
            CUI.MoveTo(control, vFrom, vTo, fTime, ltType);
            CUI.Fade(control, fAlphaForm, fAlphaTo, fTime);
            return control;
        }


        /// <summary>
        /// 屏幕外滑动进入
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fTime"></param>
        /// <param name="stType"></param>
        /// <param name="ltType"></param>
        /// <returns></returns>
        public static CUIControl SlideIn(CUIControl control, Vector3 vIn, DirType stType = DirType.Top, float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeOutCirc)
        { 
            control.show = true;
            Vector3 vFrom = Vector3.zero;
            RectTransform rt = control.GetComponent<RectTransform>(); 
            Rect rect = rt.rect;

            switch (stType)
            {
                default:
                case DirType.Top:
                    vFrom = new Vector3(0, Screen.height * 0.5f + rect.height * 0.5f, 0);
                    break;
                case DirType.Bottom:
                    vFrom = new Vector3(0, -Screen.height * 0.5f - rect.height * 0.5f, 0);
                    break;
                case DirType.Right:
                    vFrom = new Vector3(Screen.width * 0.5f + rect.width * 0.5f, 0, 0);
                    break;
                case DirType.Left:
                    vFrom = new Vector3(-Screen.width * 0.5f - rect.width * 0.5f, 0, 0);
                    break;
            }  
            CUI.MoveTo(control, vFrom, vIn, fTime, ltType); 
            return control;
        }

        /// <summary>
        /// 屏幕外滑动退出
        /// </summary>
        /// <param name="control"></param>
        /// <param name="stType"></param>
        /// <param name="fTime"></param>
        /// <param name="ltType"></param>
        /// <returns></returns>
        public static CUIControl SlideOut(CUIControl control, DirType stType = DirType.Top, float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeInCirc)
        {
            control.show = true;
            Vector3 vFrom = control.transform.localPosition;
            Vector3 vTo;
            RectTransform rt = control.GetComponent<RectTransform>();
            Rect rect = rt.rect;

            switch (stType)
            {
                default:
                case DirType.Top:
                    vTo = new Vector3(0, Screen.height * 0.5f + rect.height * 0.5f, 0);
                    break;
                case DirType.Bottom:
                    vTo = new Vector3(0, -Screen.height * 0.5f - rect.height * 0.5f, 0);
                    break;
                case DirType.Right:
                    vTo = new Vector3(Screen.width * 0.5f + rect.width * 0.5f, 0, 0);
                    break;
                case DirType.Left:
                    vTo = new Vector3(-Screen.width * 0.5f - rect.width * 0.5f, 0, 0);
                    break;
            }

            LeanTween.move(rt, vTo, fTime).setEase(ltType).setOnComplete(() => { control.show = false; });
            return control;
        }


        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="control"></param>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <param name="fTime"></param>
        /// <param name="ltType"></param>
        /// <returns></returns>
        public static CUIControl MoveTo(CUIControl control,Vector3 vFrom, Vector3 vTo, float fTime = 0.25f, LeanTweenType ltType = LeanTweenType.easeOutElastic)
        { 
            control.show = true;
            RectTransform rt = control.GetComponent<RectTransform>();
            //rt.localPosition = vFrom;
            //rt.position = vFrom;
            rt.anchoredPosition = new Vector2(vFrom.x, vFrom.y);
            LeanTween.move(rt, vTo, fTime).setEase(ltType); 
            return control;
        }

		 


        /// <summary>
        /// Popup 模式窗口
        /// </summary>
        /// <param name="control"></param>
        /// <param name="evtCallback"></param>
        /// <param name="fAlpha"></param>
        /// <returns></returns>
        public static CUIControl ShowPopup(CUIControl control, ControlEvent evtCallback, float fAlpha = 0.5f)
        {
            ShowModel(control, fAlpha);
            ___evtMaskOutCallback = evtCallback;
            Get(mMaskPlane, ControlType.Plane).SetEvent(EType.PointerDown, MaskHide);
            return control;
        }


        /// <summary>
        /// 显示模式窗口
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static CUIControl ShowModel(CUIControl control, float fAlpha = 0.5f)
        {
            CreateMask(fAlpha);
            control.show = true;
            ___lastControl = control;
            ___lastParent = control.transform.parent;
            ___lastSiblingIndex = control.transform.GetSiblingIndex();

            control.transform.parent = mCanvas.transform;
            control.transform.SetSiblingIndex(mCanvas.transform.childCount);
            return control;
        }


        /// <summary>
        ///  创建全屏遮罩Mask
        /// </summary>
        /// <param name="fAlpha"></param>
        public static void CreateMask(float fAlpha = 0.5f)
        {
            if (mCanvas == null)
                mCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            if (mMaskPlane == null)
            {
                mMaskPlane = new GameObject("Mask");
                mMaskPlane.transform.parent = mCanvas.transform;
                RectTransform rt = mMaskPlane.AddComponent<RectTransform>();
                mMaskPlane.AddComponent<CanvasRenderer>();
                Image img = mMaskPlane.AddComponent<Image>();
                img.type = Image.Type.Sliced;
                //img.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                img.color = new Color(0, 0, 0, fAlpha);
                rt.sizeDelta = new Vector2((float)Screen.width, (float)Screen.height);
                rt.localPosition = Vector3.one;
                rt.localScale = new Vector3(2, 2, 2);
            }
            mMaskPlane.transform.SetSiblingIndex(mCanvas.transform.childCount);
            Get(mMaskPlane).show = true;

        }

        public static void MaskHide(CUIControl control = null)
        {
            if (mMaskPlane == null)
                return;

            Get(mMaskPlane).show = false;
            ___lastControl.transform.parent = ___lastParent;
            ___lastControl.transform.SetSiblingIndex(___lastSiblingIndex);

            if (___evtMaskOutCallback != null)
                ___evtMaskOutCallback(___lastControl);
        }

        ///// <summary>
        ///// 显示窗口
        ///// </summary>
        ///// <typeparam name="CUIWindow"></typeparam>
        ///// <param name="mWindow"></param>
        ///// <returns></returns>
        //public static CUIWindow ShowWindow<CUIWindow>(CUIWindow mWindow)
        //{
        //    return default(CUIWindow);
        //}

        ///// <summary>
        ///// 隐藏窗口
        ///// </summary>
        ///// <typeparam name="CUIWindow"></typeparam>
        ///// <param name="mWindow"></param>
        //public static void HideWindow<CUIWindow>(CUIWindow mWindow)
        //{ 
        //}




    }

}