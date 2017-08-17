using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextEffect : MonoBehaviour {
	private static readonly float RISE_RATE=50;
	private static readonly float SWELL_RATE=2;
	private static readonly float CLEAR_RATE=2;
	public RectTransform RectTransform{ get; private set;}
	private Text m_Text;
	private float m_DeathTime;
	void Awake()
	{
		m_Text = GetComponent<Text> ();
		RectTransform=GetComponent<RectTransform>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > m_DeathTime) 
		{
			RectTransform.anchoredPosition3D += new Vector3 (0,1,0)*RISE_RATE*Time.deltaTime;
			RectTransform.localScale += Vector3.one * SWELL_RATE * Time.deltaTime;
			m_Text.color -= new Color(0,0,0,1) * CLEAR_RATE * Time.deltaTime;
			if (m_Text.color.a <= 0)
				Destroy (gameObject);
		}
	}
	public void SetText(string text,float lifeTime)
	{
		m_Text.text = text;
		m_DeathTime = Time.time + lifeTime;
	}
}
