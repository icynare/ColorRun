using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class XEventTrigger : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void VoidDelegateExt(GameObject go, PointerEventData data);
    public delegate void DragDelegate(PointerEventData data, GameObject go);
    public VoidDelegate onClick;
    public VoidDelegateExt onClickExt;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onDown;
    public VoidDelegateExt onDownExt;
    public VoidDelegate onUp;
    public VoidDelegateExt onUpExt;
    public VoidDelegate onSelect;

    public VoidDelegate onUpdateSelect;
    public DragDelegate onBeginDrag;
    public DragDelegate onEndDrag;
    public DragDelegate onDrag;
    public DragDelegate onDrop;
    public bool isScale = false;

    private Vector3 minScale = new Vector3(0.95f, 0.95f, 0.95f);

    static public XEventTrigger Get(GameObject go, bool withScale = true)
    {
        XEventTrigger listener = go.GetComponent<XEventTrigger>();
        if (listener == null) listener = go.AddComponent<XEventTrigger>();
        listener.isScale = withScale;
        return listener;
    }

    static public XEventTrigger Get(XUIViewBase view , bool withScale = true)
	{
        XEventTrigger listener = view.Root.GetComponent<XEventTrigger>();
        if (listener == null) listener = view.Root.AddComponent<XEventTrigger>();
		listener.isScale = withScale;
		return listener;
	}

    static public XEventTrigger Get(XUIItemBase view, bool withScale = true)
	{
		XEventTrigger listener = view.Root.GetComponent<XEventTrigger>();
		if (listener == null) listener = view.Root.AddComponent<XEventTrigger>();
		listener.isScale = withScale;
		return listener;
	}

    public override void OnPointerClick(PointerEventData eventData)
    {
        //AudioManager.Instance.PlayUIAudio("click");
        if (onClick != null) onClick(gameObject);
        if (onClickExt != null) onClickExt(gameObject, eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isScale)
            gameObject.transform.DOScale(minScale, 0.1f).SetEase(Ease.InOutQuad).SetAutoKill();
        if (onDown != null) onDown(gameObject);
        if (onDownExt != null) onDownExt(gameObject, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);

    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (isScale)
            gameObject.transform.DOScale(Vector3.one, 0.2f);
        if (onUp != null) onUp(gameObject);
        if (onUpExt != null) onUpExt(gameObject, eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(eventData, gameObject);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(eventData, gameObject);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(eventData, gameObject);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) onDrop(eventData, gameObject);
    }
}