using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class SwordmanInvisibleSkill : Skill {
	//开战前准备非static隐身技能入口
	public List<CharacterMono> characterList=new List<CharacterMono>();
	public SwordmanInvisibleSkill()
		:base("隐身",SkillType.FightPre)
	{
		
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		CharacterMono.AddStaticSkill (CharacterMono.staticAfterMoveSkillList, new BeSwordmanInvisibleSkill (info.Sender,this));
		yield break;
	}
	public void UpdateInvisible(CharacterMono one,bool isInside)
	{
		if (!characterList.Contains (one)) 
		{
			if(isInside)
				characterList.Add (one);
		}
		else
		{
			if(!isInside)
				characterList.Remove (one);
		}

		if (characterList.Count == 0)
			SetInvisible ();
		else
			SetVisible ();
	}
	private void SetInvisible()
	{
		Debug.Log ("Inside");
	}
	private void SetVisible()
	{
		Debug.Log ("NotInside");
	}		
}
