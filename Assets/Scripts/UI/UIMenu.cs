using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMenu : MonoBehaviour {
	protected RectTransform m_RectTransform;
	public List<UIElement> ElementList{ get; private set;}
	// Use this for initialization
	void Awake () {
		m_RectTransform = GetComponent<RectTransform> ();
		ElementList = new List<UIElement> ();
		ElementList.AddRange (GetComponentsInChildren<UIElement> ());
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	}
	public void SetAllNormal()
	{
		foreach (UIElement element in ElementList)
			element.SetNormal ();
	}
}
