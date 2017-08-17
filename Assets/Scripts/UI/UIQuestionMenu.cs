using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIQuestionMenu : UIMenu {
	[SerializeField]private Text m_Text;
	public void SetText(string text)
	{
		m_Text.text = text;
	}
}
