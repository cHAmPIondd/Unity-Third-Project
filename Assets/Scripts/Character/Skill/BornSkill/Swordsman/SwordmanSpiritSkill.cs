using UnityEngine;
using System.Collections;
using Include;

public class SwordmanSpiritSkill : Skill {
	//攻击前非static
	public SwordmanSpiritSkill()
		:base("侠客",SkillType.AttackBefore)
	{
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		if (info.Recipient.age == Age.Child || info.Recipient.sex == Sex.Female) 
		{
			info.IsStop = true;
			GameManager.instance.PreviousStep (); 
			yield break;
		}
	}
}
