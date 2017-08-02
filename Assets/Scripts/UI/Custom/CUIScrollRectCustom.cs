using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[AddComponentMenu("UI/ScrollRect Custom")]
public class CUIScrollRectCustom : ScrollRect {

	#region Properties
	protected RectTransform m_RectTransform;

	[SerializeField]
	[Range(0f, 100f)]
	protected float m_PercentX = 0f;
	public float PercentX {
		get { return m_PercentX; }
		set { m_PercentX = value; }
	}
	[SerializeField]
	[Range(0f, 100f)]
	protected float m_PercentY = 0f;
	public float PercentY {
		get { return m_PercentY; }
		set { m_PercentY = value; }
	}
	[SerializeField]
	[Range(0f, 100f)]
	protected float m_PercentWidth = 100f;
	public float PercentWidth {
		get { return m_PercentWidth; }
		set { m_PercentWidth = value; }
	}

	[SerializeField]
	[Range(0f, 100f)]
	protected float m_PercentHeight = 100f;
	public float PercentHeight {
		get { return m_PercentHeight; }
		set { m_PercentHeight = value; }
	}

	[SerializeField]
	protected bool m_ActiveScroll = true;
	public bool ActiveScroll {
		get { return m_ActiveScroll; }
		set { m_ActiveScroll = value; }
	}

	public Action<int> OnEventStartScrollDrag;
	public Action<int> OnEventScrollDrag;
	public Action<int> OnEventEndScrollDrag;

	public Action<float> OnEventEndScrollValue;

	public Action<PointerEventData> OnEventBeginDrag;
	public Action<PointerEventData> OnEventDrag;
	public Action<PointerEventData> OnEventEndDrag;
	public Action<PointerEventData> OnEventInitializePotentialDrag;

	#endregion

	#region Implementation  

	protected override void OnEnable ()
	{
		base.OnEnable ();
		MoveToFirst();
	}

	protected override void Start() {
		base.Start();
		MoveToFirst();
	}

	public override void OnInitializePotentialDrag (PointerEventData eventData)
	{
		base.OnInitializePotentialDrag (eventData);
		if (OnEventInitializePotentialDrag != null) {
			OnEventInitializePotentialDrag (eventData);
		}
	}

	public override void OnBeginDrag (PointerEventData eventData)
	{
		if (m_ActiveScroll == false) return;
		base.OnBeginDrag(eventData);
		if (OnEventStartScrollDrag != null) {
			var scrollValue = ScrollCurrentValue();
			var page 	= FindNearest(scrollValue);
			OnEventStartScrollDrag (page);
		}
		if (OnEventBeginDrag != null) {
			OnEventBeginDrag (eventData);
		}
	}

	public override void OnDrag (PointerEventData eventData)
	{
		if (m_ActiveScroll == false) return;
		base.OnDrag (eventData);
		if (OnEventScrollDrag != null) {
			var scrollValue = ScrollCurrentValue();
			var page 	= FindNearest(scrollValue);
			OnEventScrollDrag (page);
		}
		if (OnEventDrag != null) {
			OnEventDrag (eventData);
		}
	}

	public override void OnScroll (PointerEventData eventData)
	{
		if (m_ActiveScroll == false) return;
		base.OnScroll (eventData);
		if (OnEventScrollDrag != null) {
			var scrollValue = ScrollCurrentValue();
			var page 	= FindNearest(scrollValue);
			OnEventScrollDrag (page);
		}
		if (OnEventDrag != null) {
			OnEventDrag (eventData);
		}
	}

	public override void OnEndDrag (PointerEventData eventData)
	{
		if (m_ActiveScroll == false) return;
		base.OnEndDrag (eventData);
		if (OnEventEndScrollDrag != null) {
			var scrollValue = ScrollCurrentValue();
			var page 	= FindNearest(scrollValue);
			OnEventEndScrollDrag (page);
		}
		if (OnEventEndScrollValue != null) {
			OnEventEndScrollValue (ScrollCurrentValue());
		}
		if (OnEventEndDrag != null) {
			OnEventEndDrag (eventData);
		}
	}

	#endregion

	#region Main Method

	public virtual void MoveToFirst() {
		this.horizontalNormalizedPosition = 0;
		this.verticalNormalizedPosition = 1;
	}

	public virtual void MoveToEnd() {
		this.horizontalNormalizedPosition = 1;
		this.verticalNormalizedPosition = 0;
	}

	public virtual void MoveToLast() {
		this.horizontalNormalizedPosition = 1;
		this.verticalNormalizedPosition = 0;
	}

	protected virtual int FindNearest(float scrollValue)
	{
		return 0;
	}

	protected virtual float ScrollCurrentValue() {
		if (this.horizontal) {
			return this.horizontalNormalizedPosition;
		} else if (this.vertical) {
			return this.verticalNormalizedPosition;
		}
		return ScrollVectorCurrentValue().magnitude;
	}

	private Vector2 ScrollVectorCurrentValue() {
		return this.normalizedPosition;
	}

	public float GetPercentX() {
		return m_PercentX;
	}
	public float GetPercentY() {
		return m_PercentY;
	}
	public float GetPercentWidth() {
		return m_PercentWidth;
	}
	public float GetPercentHeight() {
		return m_PercentHeight;
	}

	#endregion

}