using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class EnemyAIController :Controller{
	private static readonly int LOW_BLOOD_WEIGHT=10;
	private static readonly int ALONE_WEIGHT=10;
	private static readonly int NOT_COUNTER_WEIGHT=100;//攻击危险程度
	private static readonly int MAP_WEIGHT=10;//攻击位置危险程度
	private Queue<CharacterMono> m_ActionQueue=new Queue<CharacterMono>();
	private Block m_TargetBlock;
	private CharacterMono m_TargetCharacter;
	private int[,] m_CurrentMapWeight;//越高说明离敌人越近
	public EnemyAIController()
	{
		StepList.Add (None);
		StepList.Add (BeforeMove);
		StepList.Add (Move);
		StepList.Add (BeforeHit);
		StepList.Add (Hit);
		StepList.Add (Over);
	}
	public override void AddCharacter(CharacterID id)
	{
		CharacterMono character=CharacterManager.instance.GetCharacterByID (id);
		character.gameObject.SetActive (false);
		character.team = Team.EnemyTeam;
		CharacterList.Add (character);
	}
	public void FightStart()
	{
		for (int i = 0; i < CharacterList.Count; i++) 
		{
			CharacterList [i].gameObject.SetActive (true);
			CharacterList [i].SetPosition (BlockMapManager.instance.CurrentMap [i, BlockMapManager.instance.CurrentMap.GetLength(0)-1]);
		}
		foreach (CharacterMono character in CharacterList) 
		{
			GameManager.instance.StartCoroutine(character.FightPre (new ActionInformation()));
		}
	}
	public void FightEnd()
	{
		for (int i = 0; i < CharacterList.Count; i++) 
		{
			CharacterList [i].gameObject.SetActive (false);
		}
		foreach (CharacterMono character in CharacterList) 
		{
			for (int i=0;i<character.allSkillList.Count;i++) 
			{
				if ((character.allSkillList[i].skillType & SkillType.FightTime)!=0) 
				{
					character.allSkillList.RemoveAt (i);
					i--;
				}
			}
		}
	}
	public override void RoundStart()
	{
		foreach (var chara in CharacterList) 
		{
			GameManager.instance.StartCoroutine (chara.RoundStart(new ActionInformation()));
		}
		ActionSort ();
		GameManager.instance.StartCoroutine (StepList[0]());
	}
	public override void RoundEnd()
	{
		foreach (var chara in CharacterList) 
		{
			GameManager.instance.StartCoroutine (chara.RoundEnd(new ActionInformation()));
		}
		GameManager.instance.NextTeam ();
	}

	private void ActionSort()
	{
		Block[,] map= BlockMapManager.instance.CurrentMap;
		//计算地图占领权重
		m_CurrentMapWeight = new int[map.GetLength (0), map.GetLength (1)];
		foreach (CharacterMono character in GameManager.instance.PlayerController.CharacterList) 
		{
			m_CurrentMapWeight [(int)character.Pos.x, (int)character.Pos.y]++;
			foreach (Block block in character.GetMovableArea()) 
			{
				m_CurrentMapWeight [(int)block.pos.x,(int)block.pos.y]++;
			}
		}
		foreach (CharacterMono character in CharacterList) 
		{
			m_CurrentMapWeight [(int)character.Pos.x, (int)character.Pos.y]--;
			foreach (Block block in character.GetMovableArea()) 
			{
				m_CurrentMapWeight [(int)block.pos.x,(int)block.pos.y]--;
			}
		}

		//每个角色前线分值
		Dictionary<int,int> characterSortScore= new Dictionary<int,int>();//第一个int为第几个角色，第二个int为前线分值
		for(int i=0;i<CharacterList.Count;i++)
		{
			characterSortScore.Add (i, 0);
			for (int j = 1; j <=BlockMapManager.instance.GetMapMaxLength(); j++) 
			{
				List<Block> list = BlockMapManager.instance.GetBlockListWith4 (CharacterList [i].Pos, j, false);
				if (list.Count == 0)
					break;
				foreach (Block block in list) 
				{
					characterSortScore[i] += m_CurrentMapWeight[(int)block.pos.x,(int)block.pos.y]*(BlockMapManager.instance.GetMapMaxLength()-j);
				}
			}
		}
		//排序
		List<CharacterMono> charList=new List<CharacterMono>();
		List<int> charScoreList = new List<int> ();
		for (int i = 0; i < CharacterList.Count; i++) 
		{
			int j = 0;
			for (;j<charList.Count;j++) 
			{
				if (characterSortScore [i] > charScoreList [j]) 
				{
					charList.Insert (j,CharacterList[i]);
					charScoreList.Insert (j,characterSortScore[i]);
					break;
				}
			}
			if (j == charList.Count) 
			{
				charList.Insert (j,CharacterList[i]);
				charScoreList.Insert (j,characterSortScore[i]);
			}
		}
		for (int i=0;i<charList.Count;i++) 
		{
			m_ActionQueue.Enqueue (charList[i]);
		}
	}
	private IEnumerator None()
	{
		if (m_ActionQueue.Count == 0) 
		{
			RoundEnd ();
			yield break;
		}
		GameManager.instance.CurrentCharacter = m_ActionQueue.Dequeue ();
		CharacterMono currentCharacter = GameManager.instance.CurrentCharacter;
		yield return 0;
		//寻找可攻击的敌人以及相对应的攻击位置
		Dictionary<CharacterMono,List<Block>> moveAndHitDic = new Dictionary<CharacterMono,List<Block>> ();
		foreach (Block block in GameManager.instance.CurrentCharacter.GetMovableArea()) 
		{
			foreach(Block block2 in currentCharacter.GetHitableArea(block.pos))
			{
				if(block2.character!=null&&block2.character.team!=currentCharacter.team)
				{
					if (!moveAndHitDic.ContainsKey (block2.character))
						moveAndHitDic [block2.character] = new List<Block> ();
					moveAndHitDic [block2.character].Add (block);
				}
			}
		}
		foreach(Block block2 in currentCharacter.GetHitableArea(currentCharacter.Pos))//加上原地
		{
			if(block2.character!=null&&block2.character.team!=currentCharacter.team)
			{
				if (!moveAndHitDic.ContainsKey (block2.character))
					moveAndHitDic [block2.character] = new List<Block> ();
				moveAndHitDic [block2.character].Add (BlockMapManager.instance.GetBlock(currentCharacter.Pos));
			}
		}
		//敌人与位置分值计算
		List<CharacterMono> hitChar = new List<CharacterMono>();
		List<Block> moveBlock = new List<Block>();
		foreach (var temp in moveAndHitDic) 
		{
			hitChar.Add (temp.Key);
			foreach(var block in temp.Value)
			{
				if (!moveBlock.Contains (block))
					moveBlock.Add (block);
			}
		}

		int[] hitScore = new int[hitChar.Count];
		int[] moveScore = new int[moveBlock.Count];
		for (int i=0;i<hitChar.Count;i++) 
		{
			//低血加权
			hitScore [i] -= hitChar[i].healthPoint * LOW_BLOOD_WEIGHT;
			//落单加权
			hitScore[i]-=m_CurrentMapWeight[(int)hitChar[i].Pos.x,(int)hitChar[i].Pos.y]*ALONE_WEIGHT;
			//不反击加权
			if (!hitChar [i].GetHitableArea ().Contains (moveAndHitDic[hitChar[i]][0]))
				hitScore [i] += NOT_COUNTER_WEIGHT;
		}
		for (int i = 0; i < moveBlock.Count; i++) 
		{
			//位置的危险度
			moveScore[i]-=m_CurrentMapWeight [(int)moveBlock[i].pos.x, (int)moveBlock[i].pos.y] * MAP_WEIGHT;
		}

		//根据目标敌人与位置寻找最合适的行动
		int maxScore = int.MinValue;
		for (int i = 0; i < hitScore.Length; i++) 
		{
			for (int j = i; j < moveScore.Length; j++) 
			{
				if (hitScore [i] + moveScore [j] > maxScore) 
				{
					if (moveAndHitDic [hitChar [i]].Contains (moveBlock [j])) 
					{
						maxScore = hitScore [i] + moveScore [j];
						m_TargetCharacter = hitChar [i];
						m_TargetBlock = moveBlock [j];
					}
				}
			}
		}
		//打不到敌人
		if (maxScore == int.MinValue) 
		{
			int max = int.MinValue;
			List<Block> BlockList = currentCharacter.GetMovableArea ();
			for(int i=0;i<BlockList.Count;i++)
			{
				int score=0;
				for (int j = 1; j <=BlockMapManager.instance.GetMapMaxLength(); j++) 
				{
					List<Block> list = BlockMapManager.instance.GetBlockListWith4 (BlockList[i].pos, j, false);
					if (list.Count == 0)
						break;
					foreach (Block block in list) 
					{
						score += m_CurrentMapWeight[(int)block.pos.x,(int)block.pos.y]*(BlockMapManager.instance.GetMapMaxLength()-j);
					}
				}
				if (score > max) 
				{
					max = score;
					m_TargetBlock = BlockList [i];
				}
			}
		}
		GameManager.instance.NextStep ();
	}
	private IEnumerator BeforeMove()
	{
		yield return 0;
		GameManager.instance.NextStep ();
	}
	private IEnumerator Move()
	{
		if (m_TargetBlock != null) {
			yield return GameManager.instance.StartCoroutine (
				GameManager.instance.CurrentCharacter.Move (new ActionInformation (targetBlock: m_TargetBlock)));
			m_TargetBlock = null;
		}
		GameManager.instance.NextStep ();
	}
	private IEnumerator BeforeHit()
	{
		yield return 0;
		GameManager.instance.NextStep ();
	}
	private IEnumerator Hit()
	{
		if (m_TargetCharacter != null) {
			yield return GameManager.instance.StartCoroutine (
				GameManager.instance.CurrentCharacter.Attack (new ActionInformation (recipient: m_TargetCharacter)));
			m_TargetCharacter = null;
		}
		GameManager.instance.NextStep ();
	}
	private IEnumerator Over()
	{
		yield return 0;
		GameManager.instance.NextStep ();
	}
	public override bool GetKeyDown(InputKeyCode keyCode)
	{
		return true;
	}
}
