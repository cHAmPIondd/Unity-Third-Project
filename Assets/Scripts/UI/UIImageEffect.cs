using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIImageEffect : MonoBehaviour {
	private static readonly float RISE_RATE=50;
	private static readonly float SWELL_RATE=2;
	private static readonly float CLEAR_RATE=2;
	public RectTransform RectTransform{ get; private set;}
	[SerializeField] private float m_LifeTime;
	private Image m_Image;
	private float m_DeathTime;
	private Color m_Color;
	void Awake()
	{
		m_Image = GetComponent<Image> ();
		RectTransform=GetComponent<RectTransform>();
		m_Color = m_Image.color;
	}

	// Use this for initialization
	void Start () {

	}
	void OnEnable()
	{
		Init ();
	}
	public void Init()
	{
		m_Image.color = m_Color;
		m_DeathTime = Time.time + m_LifeTime;
		RectTransform.localScale = Vector3.one;
	}
	// Update is called once per frame
	void Update () {
		if (Time.time > m_DeathTime) 
		{
			RectTransform.anchoredPosition3D += new Vector3 (0,1,0)*RISE_RATE*Time.deltaTime;
			RectTransform.localScale += Vector3.one * SWELL_RATE * Time.deltaTime;
			m_Image.color -= new Color(0,0,0,1) * CLEAR_RATE * Time.deltaTime;
			if (m_Image.color.a <= 0) 
			{
				gameObject.SetActive (false);
			}
		}
	}
}
