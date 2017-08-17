using UnityEngine;
using System.Collections;

public class UIDropMenu : UIMenu {

	private static readonly float DISTANCE_HEIGHT=30;
	public GameObject UIElementPrefab;
	public void AddElement(string text)
	{
		UIElement element=((GameObject)Instantiate (UIElementPrefab, transform)).GetComponent<UIElement>();
		element.SetText (text);
		if (ElementList.Count != 0) 
		{
			element.RectTransform.anchoredPosition =
				ElementList [ElementList.Count - 1].RectTransform.anchoredPosition - new Vector2 (0, DISTANCE_HEIGHT);
		} 
		else 
		{
			element.SetPosition(new Vector2 (0, 0));
		}
		ElementList.Add (element);
	}
	public void Clear()
	{
		foreach(UIElement element in ElementList)
		{
			Destroy(element.gameObject);
		}
		ElementList.Clear ();
	}
	public void SetPosition(Vector3 pos)
	{
		m_RectTransform.position= UIManager.instance.mainCamera.WorldToScreenPoint (pos);
	}
}
