using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class AssassinLonelinessSkill : Skill {
	//受伤后非static技能
	public AssassinLonelinessSkill()
		:base("孤独",SkillType.BeAttackBefore)
	{

	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		List<Block> list=BlockMapManager.instance.GetBlockListWith8 (info.Recipient.Pos, 1, false);
		foreach (Block block in list) 
		{
			if (block.character != null && block.character.team == info.Recipient.team) 
			{
				yield break;
			}
		}
		info.Damage+=info.Damage/2;
	}
}
