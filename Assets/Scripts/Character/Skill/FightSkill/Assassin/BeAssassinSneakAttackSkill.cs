using UnityEngine;
using System.Collections;
using Include;

public class BeAssassinSneakAttackSkill : Skill {
	//受伤后非static
	public BeAssassinSneakAttackSkill()
		:base(SkillType.InjuredAfter|SkillType.FightTime)
	{

	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		Direction direction =  CharacterMono.GetDirection (info.Sender.Pos, info.Recipient.Pos);
		if ( direction== info.Recipient.Direction)
			info.Damage+=info.Damage/2;
		else if ((int)direction == -(int)info.Recipient.Direction)
			info.Damage-=info.Damage/2;
		info.RemoveSkillInfoList.Add (new RemoveSkillInfo (info.Recipient.afterInjuredSkillList,this,info.Recipient));
		yield break;
	}
}
