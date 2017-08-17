using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIElement : MonoBehaviour {
	private static readonly Color COLOR_NORMAL=new Color(1,1,1,1);
	private static readonly Color COLOR_SELECTED=new Color(0.1f,1,0,1);
	private RectTransform m_RectTransform;
	public RectTransform RectTransform { 
		get {
			if (m_RectTransform == null)
				m_RectTransform = GetComponent<RectTransform> ();
			return m_RectTransform;
		}
		private set{m_RectTransform = value;}}
	[HideInInspector]private Text m_Text;
	[HideInInspector]private Image m_Image;
	// Use this for initialization
	void Awake () {
		m_Image = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetText(string text)
	{
		if(m_Text==null)
			m_Text = GetComponentInChildren<Text> ();
		m_Text.text = text;
	}
	public void SetPosition(Vector2 pos)
	{
		RectTransform.anchoredPosition= pos;
	}

	public void SetNormal()
	{
		m_Image.color = COLOR_NORMAL;
	}
	public void SetSelected()
	{
		m_Image.color = COLOR_SELECTED;
	}

}
