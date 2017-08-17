using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class CharacterManager : MonoBehaviour {
	public static CharacterManager instance;
	private Dictionary<CharacterID,GameObject> m_CharacterDic=new Dictionary<CharacterID,GameObject>();
	public GameObject[] characterPrefabs;
	void Awake()
	{
		instance = this;
		//建立人物预设体
		foreach(GameObject GO in characterPrefabs)
		{
			m_CharacterDic.Add (GO.GetComponent<CharacterMono> ().characterID,GO);
		}
	}
	public CharacterMono GetCharacterByID(CharacterID id)
	{
		return Instantiate (m_CharacterDic[id]).GetComponent<CharacterMono>();
	}
	public void AddSkill(CharacterMono character, Skill skill)
	{
		if ((skill.skillType & SkillType.ActiveSkill) != 0)
			character.AddSkill(character.activeSkillList,skill as ActiveSkill);
		
		if ((skill.skillType & SkillType.AttackAfter) != 0)
			character.AddSkill (character.afterAttackSkillList,skill);

		if ((skill.skillType & SkillType.AttackBefore) != 0)
			character.AddSkill(character.beforeAttackSkillList,skill);

		if ((skill.skillType & SkillType.BeAttackBefore) != 0)
			character.AddSkill(character.beforeBeAttackedSkillList,skill);

		if ((skill.skillType & SkillType.FightPre) != 0)
			character.AddSkill(character.fightPreSkillList,skill);

		if ((skill.skillType & SkillType.HitAfter) != 0)
			character.AddSkill(character.afterHitSkillList,skill);

		if ((skill.skillType & SkillType.HitBefore) != 0)
			character.AddSkill(character.beforeHitSkillList,skill);

		if ((skill.skillType & SkillType.InjuredAfter) != 0)
			character.AddSkill(character.afterInjuredSkillList,skill);

		if ((skill.skillType & SkillType.InjuredBefore) != 0)
			character.AddSkill(character.beforeInjuredSkillList,skill);

		if ((skill.skillType & SkillType.MoveAfter) != 0)
			character.AddSkill(character.afterMoveSkillList,skill);

		if ((skill.skillType & SkillType.MoveBefore) != 0)
			character.AddSkill(character.beforeMoveSkillList,skill);

		if ((skill.skillType & SkillType.RoundEndAfter) != 0)
			character.AddSkill(character.afterRoundEndSkillList,skill);

		if ((skill.skillType & SkillType.RoundEndBefore) != 0)
			character.AddSkill(character.beforeRoundEndSkillList,skill);

		if ((skill.skillType & SkillType.RoundStartAfter) != 0)
			character.AddSkill(character.afterRoundStartSkillList,skill);

		if ((skill.skillType & SkillType.RoundStartBefore) != 0)
			character.AddSkill(character.beforeRoundStartSkillList,skill);

		if((skill.skillType & SkillType.BeCounterattackedBefore) != 0)
			character.AddSkill(character.beforeBeCounterattackedSkillList,skill);
		
		if((skill.skillType & SkillType.CounterattackedBefore) != 0)
			character.AddSkill(character.beforeCounterattackSkillList,skill);
		
		if((skill.skillType & SkillType.CanAttackBefore) != 0)
			character.AddSkill(character.beforeCanAttackSkillList,skill);

		if((skill.skillType & SkillType.CanBeAttackedBefore) != 0)
			character.AddSkill(character.beforeCanBeAttackedSkillList,skill);
	}
}
