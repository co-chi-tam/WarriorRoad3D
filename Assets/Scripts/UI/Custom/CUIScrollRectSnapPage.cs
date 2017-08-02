using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("UI/Scroll Rect Snap Page")]
public class CUIScrollRectSnapPage : CUIScrollRectCustom {

	#region Properties

	[SerializeField]
	private int m_CurrentPage;
	public int CurrentPage {
		get { return m_CurrentPage; }
		set { m_CurrentPage = value; }
	}
	[SerializeField]
	private int m_Pages;
	public int Pages {
		get { return m_Pages; }
	}

	[SerializeField]
	[Range (10, 200)]
	private float m_SnapSpeed = 75f;

	private float m_fStep;
	private bool m_bHaveMove;
	private float m_fTargetScroll;
	private int m_PreviousPage;

	public Action<int> OnSnapPage;

	[SerializeField]
	private List<PageActiveInfo> m_PageActive = new List<PageActiveInfo> ();

	private struct PageActiveInfo {
		public bool Active;
		public float StepScroll;
	}

	#endregion

	#region Implementation 

	protected override void OnEnable() {
		base.OnEnable();
		CalculateScrollRect();
		SnapRectScroll (m_CurrentPage);  
		MoveToPage (m_CurrentPage);
	}

	public CUIScrollRectSnapPage () : base ()
	{
		this.m_bHaveMove = false;
	}

	protected override void Start() {
		var itemPerPage				= 1f;
		m_fStep 					= itemPerPage / (this.m_Pages - 1);
		m_fTargetScroll				= 0f;
		CalculateScrollRect();
		SnapRectScroll (m_CurrentPage);
	}

	protected override void LateUpdate() {
		base.LateUpdate();
		if (this.m_bHaveMove) {
			this.ScrollToTarget();
		}
	}

	private float m_TimeDrag;
	private Vector3 m_PosDrag;

	public override void OnBeginDrag (PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		if (m_ActiveScroll == false) return;
		this.m_bHaveMove = false;
		m_PosDrag = Input.mousePosition;
		m_TimeDrag = Time.time;
	}

	public override void OnDrag (PointerEventData eventData)
	{
		base.OnDrag (eventData);
		if (m_ActiveScroll == false) return;
		this.m_bHaveMove = false;
	}

	public override void OnScroll (PointerEventData eventData)
	{
		base.OnScroll (eventData);
		if (m_ActiveScroll == false) return;
		this.m_bHaveMove = true;
	}

	public override void OnEndDrag (PointerEventData eventData)
	{
		base.OnEndDrag (eventData);
		if (m_ActiveScroll == false) return;
		this.m_bHaveMove = true;

		if ((Time.time - m_TimeDrag) < 0.75f) {
			var direction = Vector3.zero;
			direction = Input.mousePosition - m_PosDrag;
			var tempPage = 0;
			if (this.horizontal) {
				if (direction.x > 5) {
					tempPage = m_CurrentPage <= 0 ? 0 : m_CurrentPage - 1;
				} else if (direction.x < -5) {
					tempPage = m_CurrentPage >= m_Pages - 1 ? m_CurrentPage : m_CurrentPage + 1;
				}
			} else if (this.vertical) {
				if (direction.y > 5) {
					tempPage = m_CurrentPage <= 0 ? 0 : m_CurrentPage - 1;
				} else if (direction.y < -5) {
					tempPage = m_CurrentPage >= m_Pages - 1 ? m_CurrentPage : m_CurrentPage + 1;
				}
			}

			if (m_PageActive.Count != 0) { 
				if (m_PageActive[tempPage].Active == true) {
					m_CurrentPage = tempPage;
					m_fTargetScroll = m_PageActive[tempPage].StepScroll;
				}
			}
			m_TimeDrag = Time.time;
		} else {
			var scrollValue = ScrollCurrentValue();
			var tempPage 	= FindNearest(scrollValue);
			if (m_PageActive.Count != 0) { 
				if (m_PageActive[tempPage].Active == true) {
					m_CurrentPage 	= tempPage;
					m_fTargetScroll = m_PageActive[tempPage].StepScroll;
				}
			}
		}

		if (OnSnapPage != null && m_PreviousPage != m_CurrentPage) {
			if (m_PageActive.Count != 0) { 
				if (this.horizontal) {
					if (m_PageActive[m_CurrentPage].Active) {
						OnSnapPage (m_CurrentPage);
					}
				} else if (this.vertical) { 
					if (m_PreviousPage != m_CurrentPage) {
						OnSnapPage (Pages - m_CurrentPage);
					}
				}
			}
		}

		if (m_PreviousPage != m_CurrentPage) {
			m_PreviousPage = m_CurrentPage;
		}
	}
	#endregion

	#region Main Method

	public void CalculateScrollRect() {
		this.m_Pages = 0;
		m_PageActive			= new List<PageActiveInfo> ();
		var itemPerPage			= 1f;
		for (int i = 0; i < this.content.childCount; i++) {
			this.m_Pages = this.content.GetChild(i).gameObject.activeSelf ? this.m_Pages + 1 : this.m_Pages;
		}
		this.m_CurrentPage 		= this.vertical ? (this.m_Pages <= 0 ? 0 : this.m_Pages - 1) : this.m_CurrentPage;
		this.m_fStep			= itemPerPage / (this.m_Pages - 1);
		for (int i = 0; i < this.m_Pages; i++) {
			m_PageActive.Add (new PageActiveInfo() { Active = true, StepScroll = i * m_fStep });
		}
		this.m_fTargetScroll	= this.vertical ? 1f : 0f;
		this.m_bHaveMove = false;
		this.SnapRectScroll (this.m_CurrentPage);  
	}

	private void ScrollToTarget() {
		var timeInterpolate = m_SnapSpeed * this.decelerationRate * 0.025f;
		if (this.horizontal) {
			this.horizontalNormalizedPosition = 
				Mathf.Lerp(this.horizontalNormalizedPosition, m_fTargetScroll, timeInterpolate);
		} else if (this.vertical) {
			this.verticalNormalizedPosition = 
				Mathf.Lerp(this.verticalNormalizedPosition, m_fTargetScroll, timeInterpolate);
		}
	}

	protected override int FindNearest(float scrollValue)
	{
		base.FindNearest(scrollValue);
		float distance = Mathf.Infinity;
		int output = 0;
		for (int i = 0; i < m_Pages; i++)
		{
			var byPage = m_PageActive[i].StepScroll;
			if (Mathf.Abs(byPage - scrollValue) < distance)
			{
				distance = Mathf.Abs(byPage - scrollValue);
				output = i;
			}
		}
		if (m_PageActive.Count != 0) {
			if (m_PageActive [output].Active == false) {
				return m_CurrentPage;
			}
		}
		return output;
	}

	private void SnapRectScroll (int page) {
		var pageToScroll = page == 0 ? 0.0f : (page < 0 || page >= m_PageActive.Count) ? m_fStep * page : m_PageActive[page].StepScroll;
		if (this.horizontal) {
			this.horizontalNormalizedPosition = pageToScroll;
		} else if (this.vertical) {
			this.verticalNormalizedPosition = pageToScroll;
		}
	}

	public override void MoveToFirst() {
		this.horizontalNormalizedPosition = 0;
		this.verticalNormalizedPosition = 1;
	}

	public void SetActivePage(int index, bool value) {
		if (m_PageActive == null) return;
		if (index >= m_PageActive.Count) return;
		m_PageActive[index] = new PageActiveInfo () { Active = value, StepScroll = index * m_fStep };
	}

	public void MoveToPage(int page) {
		if (this.horizontal) {
			page = page >= m_Pages ? m_Pages - 1 : page;
		} else if (this.vertical) {
			page = m_Pages - (page >= m_Pages ? m_Pages - 1 : page);
		}
		//		var scrollValue = ScrollCurrentValue ();
		m_fTargetScroll = (page < 0 || page >= m_PageActive.Count) ? m_fStep * page : m_PageActive[page].StepScroll;
		m_CurrentPage = page;
		this.m_bHaveMove = true;
	}

	public bool IsActivePage(int index) {
		if (m_PageActive.Count == 0) return false;
		return m_PageActive[index].Active;
	}

	#endregion

}