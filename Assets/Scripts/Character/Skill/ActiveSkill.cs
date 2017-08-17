using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;
public class ActiveSkill : Skill {
	public List<Step> StepList{ get; private set;}
	public ActiveSkill(string n="None",SkillType skillT=SkillType.ActiveSkill,int orderIndex=0)
		:base(n,skillT,orderIndex)
	{
		StepList = new List<Step> ();
	}
	public virtual bool CanExert()
	{
		return true;
	}
}
