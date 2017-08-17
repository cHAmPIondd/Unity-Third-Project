using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemoveSkillInfo{
	private List<Skill> SkillList{ get; set;}
	private Skill Skill{ get; set;}
	private CharacterMono Character{ get; set;}
	public RemoveSkillInfo(List<Skill> list,Skill skill,CharacterMono character=null)
	{
		SkillList = list;
		Skill = skill;
		Character = character;
	}
	public void RemoveSkill()
	{
		if (Character != null)
			Character.RemoveSkill (SkillList,Skill);
		else
			CharacterMono.RemoveStaticSkill (SkillList,Skill);
	}
}
