using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class AssassinHideSkill : Skill {
	 //被攻击前非static技能
	public AssassinHideSkill()
		:base("隐匿",SkillType.BeAttackBefore)
	{
		
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		List<Block> list=BlockMapManager.instance.GetBlockListWith8 (info.Recipient.Pos, 1, false);
		foreach(Block block in list)
		{
			if(block.character!=null&&block.character.team==info.Recipient.team)
			{
				info.IsStop = true;
				GameManager.instance.PreviousStep (); 
				yield break;
			}
		}
	}
}
