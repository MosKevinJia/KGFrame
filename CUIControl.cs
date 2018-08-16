using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;  



/*
 * 控件基类 UGUI
 * 
 * 所有控件通用方法  
 *                  SetValue  
 *                  GetValue
 *                 
 *                  GetText
 *                  SetText
 * 
 * 所有控件通用事件
 *                 OnChangeValue
 *                  
 * 
 */

namespace KGFrame
{
    /// <summary>
    /// 控件类型
    /// </summary>
    public enum ControlType
    {
        Button = 0,         //按钮
        Label = 1,          //文本标签
        Window = 2,         //弹出窗口
        Plane,              //面板
        Image,		        //图像(头像)
        ScrollBar,          //滚动条(装备栏)
        ProgressBar,        //进度条(血条)
        Marquee,            //跑马灯
        SlideBar,           //滑动条
        InputField          //输入框
    }

    /// <summary>
    /// 控件状态
    /// </summary>
    public enum ControlState
    {
        Normal = 0,
        Press = 1,
        Selected,
        Hover,
        Focus,
        Disable
    }


    /// <summary>
    /// 图片类型
    /// </summary>
    public enum ImageType
    {
        Default = 0,                //默认
        Background = 1,             //一般背景
        BackgroundOuter,            //外围背景(外框)
        Handle,                     //操作控制部分 
        Press,
        Selected,
        Hover,
        Focus,
        Disable,
        Progress,
        Progress2

    }


    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EType
    {
        PointerEnter = 0,
        PointerExit = 1,
        PointerDown = 2,
        PointerUp = 3,
        PointerClick = 4,
        Drag = 5,
        Drop = 6,
        Scroll = 7,
        UpdateSelected = 8,
        Select = 9,
        Deselect = 10,
        Move = 11,
        InitializePotentialDrag = 12,
        BeginDrag = 13,
        EndDrag = 14,
        Submit = 15,
        Cancel = 16,
        ValueChange = 100,
    }

   



    /// <summary>
    /// 通用控件类
    /// 
    /// 
    /// </summary>
    public class CUIControl : Spirit
    {
        private decimal             _dValue; 
        private string              _strValue;  
        public ControlType          Type                = ControlType.Button;                                  //控件类型a
        public ControlState         State               = ControlState.Normal;                                 //控件默认状态

        public string               BindValue;
        private RectTransform       rect;


        public bool show
        {
            get { return bShow; }
            set
            {
                bShow = value;
                this.gameObject.SetActive(bShow);
            }
        }

        public Vector3 DefaultPosition
        {
            get { return vDefaultPos; }
        }

        


        //private Dictionary<string, object> dictValue = new Dictionary<string, object>();
        public  ControlEvent        evtOnChange         = null;
        private Transform           Internal_Text       = null;
        private Transform           Internal_Image      = null;
        private Button              button              = null;
        private InputField          input               = null;
        private Slider              slider              = null;
        private Text                text                = null;
		private Image 				image 				= null;
        private bool                bInit               = false;
        private bool                bShow               = true;
        private EventTriggerListener mEventListener     = null;
        private Vector3             vDefaultPos         = Vector3.zero;

 



        /// <summary>
        /// 初始化控件 
        /// 
        /// 如果要增加控件, 在这里添加初始化.
        /// </summary>
        protected virtual void SerializeControl()
        {

            if (bInit)
                return;

            switch (Type)
            {
                case ControlType.Button:
                    Internal_Text = this.transform.Find("Text");
                    Internal_Image = this.transform;
                    button = this.GetComponent<Button>();
                    break;
                case ControlType.Image:
                    Internal_Image = this.transform;
					image = Internal_Image.GetComponent<Image>();
                    break;
                case ControlType.Label:
                    Internal_Text = this.transform; 
                    text = Internal_Text.GetComponent<Text>();
                    break;
                case ControlType.Marquee:
                    break;
                case ControlType.Plane:
                    break;
                case ControlType.ProgressBar:
                    break;
                case ControlType.ScrollBar:
                    break;
                case ControlType.SlideBar:
                    slider = this.GetComponent<Slider>(); 
                    break;
                case ControlType.InputField: 
                    input = this.GetComponent<InputField>();
                    break;
                case ControlType.Window:
                    break;
                default:
                    break;
            }
            vDefaultPos = this.transform.localPosition;
            bInit = true; 
        }

        public virtual CUIControl SetParent(string path)
        {
            Transform t = GameObject.Find(path).transform;
            if (t != null)
            {
                this.transform.parent = t;
            }
            return this;
        }


        public virtual CUIControl Show()
        {
            this.show = true;
            return this;
        }

        public virtual CUIControl Hide()
        {
            this.show = false;
            return this;
        }

        public virtual CUIControl FadeIn(float fTime = 0.5f)
        {
            CUI.FadeIn(this, fTime);
            return this;
        }

        public virtual CUIControl FadeOut(float fTime = 0.5f)
        {
            CUI.FadeOut(this, fTime);
            return this;
        }

        public virtual CUIControl Fade(float fFrom, float fTo, float fTime = 0.5f)
        {
            CUI.Fade(this,fFrom, fTo, fTime);
            return this;
        }


        /// <summary>
        /// 得到文本
        /// </summary>
        /// <returns></returns>
        public virtual string GetText()
        {
            SerializeControl();

            if (this.Type == ControlType.Label)
            {
                return text.text;
            }
            return _strValue;
        }

		public virtual float GetValue()
        {
			return (float)_dValue;
        }


        /// <summary>
        /// 获得子对象
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public virtual CUIControl Get(string strName, ControlType controlType = ControlType.Button)
        {
            Transform trans = this.transform.Find(strName);
            if (!trans)
            {
                return null;
            }
            else 
            {
                CUIControl ui = trans.GetComponent<CUIControl>();
                if (ui)
                {
                    return ui;
                }
                else
                {   ui = trans.gameObject.AddComponent<CUIControl>();
                    ui.Type = controlType;
                    return ui;
                } 
            } 
        }


        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="dValue"></param>
        /// <returns></returns>
        public virtual CUIControl SetValue(float fValue)
        {
            SerializeControl();

            if (this.Type == ControlType.SlideBar)
            {  
                slider.value = fValue;
				_dValue = (decimal)fValue; 
            } 
            return this;
        }





        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="uiObj"></param>
        /// <param name="vPos"></param>
        /// <returns></returns>
		public virtual CUIControl SetPos(Vector3 vPos, bool bAnchored = true)
        { 
            if (!rect)
            {
                rect = this.GetComponent<RectTransform>();
            }
			if (bAnchored) {
				rect.anchoredPosition3D = vPos; 
			} else {
				rect.offsetMax = new Vector2(vPos.x, vPos.y);  
			}  
            return this;
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="vSize"></param>
        /// <returns></returns>
        public virtual CUIControl SetSize(Vector2 vSize)
        {
            if (!rect)
            {
                rect = this.GetComponent<RectTransform>();
            } 
            rect.sizeDelta = new Vector2(vSize.x, vSize.y);  
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vScale"></param>
        /// <returns></returns>
        public virtual CUIControl SetScale(Vector3 vScale)
        {
            if (!rect)
            {
                rect = this.GetComponent<RectTransform>();
            }
            rect.localScale = vScale;
            return this;
        }


        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public virtual CUIControl SetText(string strText)
        {
 
            SerializeControl();

            if (this.Type == ControlType.Label)
            { 
                text.text = strText;
            }
            else if (this.Type == ControlType.InputField)
            { 
                _strValue = input.text = strText; 
            }
            return this;
        }
         


        /// <summary>
        /// 设置图像
        /// </summary>
        /// <param name="obj"></param>
        public virtual CUIControl SetImage(Sprite img)
        {
            SerializeControl();
            
            if (this.Type == ControlType.Image)
            {
                image.sprite = img;
            }
            return this;
        }

        /// <summary>
        /// 加载资源图片
        /// </summary>
        /// <param name="strImage"></param>
        /// <returns></returns>
        public virtual CUIControl SetImageRes(string strImage)
        {
            SerializeControl();

            if (this.Type == ControlType.Image)
            {
                image.sprite = Resources.Load(strImage, typeof(Sprite)) as Sprite;
            }
            return this;
        }


        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="color">Color.</param>
        public virtual CUIControl SetColor(Color color)
		{
			SerializeControl();

			if (this.Type == ControlType.Label)
			{ 
				text.color = color;
			}
			else if (this.Type == ControlType.Image)
			{ 
				image.color = color;
			}
			return this;
		}


        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="eState"></param>
        public virtual CUIControl SetState(ControlState eState)
        {
            if (mEventListener == null)
                mEventListener = this.GetComponent<EventTriggerListener>();

            mEventListener.State = State = eState;

            bool bEnable = true;
            if (eState == ControlState.Disable)
                bEnable = false;

            switch (this.Type)
            {
                case ControlType.Button:
                    button.interactable = bEnable;
                    break;
                default:
                    break;
            }

            
            return this;
        }


		 





        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="evtCallback"></param>
        /// <returns></returns>
        public virtual CUIControl OnClick(ControlEvent evtCallback)
        { 
            return SetEvent(EType.PointerClick, evtCallback);
        }

        public virtual CUIControl OnClick(Callback callback)
        {
            return SetEvent(EType.PointerClick, callback);
        }

        public virtual CUIControl OnDown(ControlEvent evtCallback)
        {
            return SetEvent(EType.PointerDown, evtCallback);
        }
        public virtual CUIControl OnDown(Callback callback)
        {
            return SetEvent(EType.PointerDown, callback);
        }

        public virtual CUIControl OnExit(Callback callback)
        {
            return SetEvent(EType.PointerExit, callback);
        }


        /// <summary>
        /// 值改变
        /// </summary>
        /// <param name="evtCallback">Evt callback.</param>
        public virtual CUIControl OnChange(ControlEvent evtCallback)
		{ 
			return SetEvent(EType.ValueChange, evtCallback);
		} 



		/// <summary>
		/// 设置事件
		/// </summary>
		/// <returns>The event.</returns>
		/// <param name="eEvtType">E evt type.</param>
		/// <param name="callback">Callback.</param>
        public virtual CUIControl SetEvent(EType eEvtType, Callback callback)
        {

            SerializeControl();

            switch (eEvtType)
            {
                case EType.PointerClick:
                    //bt.onClick.AddListener(delegate() { evtCallback(this.gameObject); });
                    if (State == ControlState.Normal)
                        EventTriggerListener.Get(this.gameObject).onclick = callback;
                    break;
                case EType.PointerDown:
                    if (State == ControlState.Normal)
                        EventTriggerListener.Get(this.gameObject).ondown = callback;
                    break;
                case EType.PointerExit:
                    //if (State == ControlState.Normal)
                        EventTriggerListener.Get(this.gameObject).onexit = callback;
                    break;
                //case EType.UpdateSelected:
                //    if (State == ControlState.Normal)
                //        EventTriggerListener.Get(this.gameObject).onUpdateSelect = evtCallback;
                //    break;
                //                case EType.ValueChange:
                //                    if (this.Type == ControlType.SlideBar)
                //                    {
                //                        slider.onValueChanged.AddListener(ValueChangeEvt);
                //						evtOnChange = callback;
                //                    }
                //                    else if (this.Type == ControlType.InputField)
                //                    {
                //                        input.onValueChange.AddListener(ValueChangeEvt);
                //						evtOnChange = callback;
                //                    }
                //                    break;
                default:
                    break;
            }
            return this;
        }


        /// <summary>
        /// 设置事件
        /// </summary>
        /// <param name="eEvtType"></param>
        /// <param name="evtCallback"></param>
        public virtual CUIControl SetEvent(EType eEvtType, ControlEvent evtCallback)
        {
            SerializeControl();

            switch (eEvtType)
            {
                case EType.PointerClick:
                    if (State == ControlState.Normal)
                        EventTriggerListener.Get(this.gameObject).onClick = evtCallback;
                    break;
                case EType.PointerDown:
                    if (State == ControlState.Normal)
                        EventTriggerListener.Get(this.gameObject).onDown = evtCallback;
                    break;
                case EType.UpdateSelected: 
                    if(State == ControlState.Normal)
                        EventTriggerListener.Get(this.gameObject).onUpdateSelect = evtCallback;
                    break;
                case EType.PointerExit: 
                    EventTriggerListener.Get(this.gameObject).onExit = evtCallback;
                    break;
                case EType.ValueChange:
                    if (this.Type == ControlType.SlideBar)
                    { 
                        slider.onValueChanged.AddListener(ValueChangeEvt);
                        evtOnChange = evtCallback; 
                    }
                    else if (this.Type == ControlType.InputField)
                    { 
                        input.onValueChange.AddListener(ValueChangeEvt);
                        evtOnChange = evtCallback; 
                    }
                    break;
                default:
                    break;
            }
            return this; 
        }


        /// <summary>
        /// 值更新
        /// </summary>
        /// <param name="fValue"></param>
        private void ValueChangeEvt(float fValue)
        {   
            _dValue = (decimal)fValue; 
            if (null != evtOnChange)
                evtOnChange(this); 
        }

        /// <summary>
        /// 值更新
        /// </summary>
        /// <param name="strValue"></param>
        private void ValueChangeEvt(string strValue)
        {
            _strValue = strValue;
            if (null != evtOnChange)
                evtOnChange(this);
        }





    }

}