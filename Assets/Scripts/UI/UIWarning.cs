using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIWarning : MonoBehaviour {
	private Image m_Image;
	private Text m_Text;
	private float m_DeathTime;
	// Use this for initialization
	void Awake () {
		m_Image = GetComponent<Image> ();
		m_Text = GetComponentInChildren<Text> ();
	}

	void Start()
	{
		GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
	}
	// Update is called once per frame
	void Update () {
		if (Time.time > m_DeathTime) 
		{
			m_Image.color -= new Color (0,0,0,Time.deltaTime);
			m_Text.color-=new Color (0,0,0,Time.deltaTime);
			if (m_Image.color.a <= 0)
				Destroy (gameObject);
		}
	}
	public void SetText(string text,float lifeTime)
	{
		m_Text.text = text;
		m_DeathTime = Time.time + lifeTime;
	}

}
