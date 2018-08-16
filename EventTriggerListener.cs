using UnityEngine; 
using System.Collections;
using UnityEngine.EventSystems;

namespace KGFrame
{


    /// <summary>
    /// 事件监听
    /// </summary>
    public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        //public delegate void Callback(GameObject go);
        public Callback onclick;
        public Callback ondown;
        public Callback onexit;
        public ControlEvent onClick;
        public ControlEvent onDown;
        public ControlEvent onEnter;
        public ControlEvent onExit;
        public ControlEvent onUp;
        public ControlEvent onSelect;
        public ControlEvent onUpdateSelect;
        public ControlEvent onValueChange;

        public ControlState State = ControlState.Normal;

        public CUIControl mControl;

        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>(); 

            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onClick != null && State == ControlState.Normal)
            {
                onClick(mControl);
            }
            else if (onclick != null && State == ControlState.Normal) onclick();
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onDown != null && State == ControlState.Normal) { onDown(mControl); } else if (ondown != null && State == ControlState.Normal) { ondown(); }
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onEnter != null && State == ControlState.Normal) onEnter(mControl);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onExit != null && State == ControlState.Normal) { onExit(mControl); } else if (onexit != null) { onexit(); }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onUp != null && State == ControlState.Normal) onUp(mControl);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onSelect != null && State == ControlState.Normal) onSelect(mControl);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
            if (onUpdateSelect != null && State == ControlState.Normal) onUpdateSelect(mControl);
        }
        //public void OnValueChange(BaseEventData eventData)
        //{
        //    Debug.Log("vvv");
        //    if (mControl == null) mControl = gameObject.GetComponent<CUIControl>();
        //    if (onValueChange != null) onValueChange(mControl);
        //}
    }
}