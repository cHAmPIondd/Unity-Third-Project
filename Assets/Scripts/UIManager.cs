using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	public static UIManager instance;
	public Camera mainCamera; 
	public UIDropMenu roundMenu;
	public UIDropMenu beforeMoveMenu;
	public UIDropMenu beforeHitMenu;
	public UIQuestionMenu confirmMenu;
	public UIDropMenu skillMenu;
	public RectTransform knightShield;
	[SerializeField]private GameObject m_UIWarningPrefab; 
	[SerializeField]private GameObject m_UISubHealthEffectPrefab;
	void Awake()
	{
		instance = this;
	}
	public void Init ()
	{
		
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	public void Warning(string text,float time=1)
	{
		UIWarning warning=((GameObject)Instantiate (m_UIWarningPrefab, transform)).GetComponent<UIWarning>();
		warning.SetText (text,time);
	}
	public void SubHealthEffect(string text,Vector3 pos,float time=0)
	{
		UITextEffect textEffect=((GameObject)Instantiate (m_UISubHealthEffectPrefab, transform)).GetComponent<UITextEffect>();
		textEffect.SetText (text,time);
		textEffect.RectTransform.position = mainCamera.WorldToScreenPoint (pos);
	}
}
