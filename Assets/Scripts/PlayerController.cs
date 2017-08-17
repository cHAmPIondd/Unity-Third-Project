using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class PlayerController:Controller{
	private List<CharacterMono> m_CoolingCharacterList=new List<CharacterMono>();	

	private Vector2 m_ControllerPos;
	private Block m_CurrentBlock;

	private int m_SelectedIndex;

	public PlayerController()
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
		CharacterList.Add (character);
	}
	public void FightStart()
	{	
		for (int i = 0; i < CharacterList.Count; i++) 
		{
			CharacterList [i].gameObject.SetActive (true);
			CharacterList [i].SetPosition (BlockMapManager.instance.CurrentMap [i, 0]);
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
		SceneManager.instance.posSign.gameObject.SetActive (true);
		GameManager.instance.StartCoroutine (StepList[0]());
	}
	public override void RoundEnd()
	{
		foreach (var chara in CharacterList) 
		{
			GameManager.instance.StartCoroutine (chara.RoundEnd(new ActionInformation()));
		}
		SceneManager.instance.posSign.gameObject.SetActive (false);
		m_CoolingCharacterList.Clear ();
		GameManager.instance.NextTeam ();
	}
	private IEnumerator None()
	{
		while(true)
		{
			yield return 0;
			PosSignMoveUpdate ();
			if (GetKeyDown (InputKeyCode.OK)) //选中Block
			{
				m_CurrentBlock = BlockMapManager.instance.GetBlock (m_ControllerPos);
				if (m_CurrentBlock.character != null
					&&m_CurrentBlock.character.team == Team.PlayerTeam
					&&!m_CoolingCharacterList.Contains (m_CurrentBlock.character)) //可操作人物
				{
					GameManager.instance.CurrentCharacter = m_CurrentBlock.character;
					GameManager.instance.NextStep ();
					return false;
				} 
				else //空白区块
				{
					UIDropMenu menu = UIManager.instance.roundMenu;
					menu.SetPosition(m_CurrentBlock.transform.position);
					BeforeSelectedIndexUpdate (menu);
					while (true) //RoundMenu选择循环
					{
						yield return 0;
						SelectedIndexUpdate (menu);
						if (GetKeyDown (InputKeyCode.OK)) {
							AfterSelectedIndexUpdate (menu);
							if (m_SelectedIndex == 0) {//回合结束 
								UIQuestionMenu menu2=UIManager.instance.confirmMenu;
								menu2.SetText ("确定结束回合?");
								BeforeSelectedIndexUpdate (menu2);
								while (true)
								{
									yield return 0;
									SelectedIndexUpdate (menu2);
									if (GetKeyDown (InputKeyCode.OK)) 
									{
										AfterSelectedIndexUpdate(menu2);
										if (m_SelectedIndex == 0) {//确定 
											RoundEnd ();										
											yield break;
										}
										else//取消
										{
											yield return 0;
											break;
										}
									}
									if (GetKeyDown (InputKeyCode.Return)) 
									{
										yield return 0;
										AfterSelectedIndexUpdate(menu2);
										break;
									}
								}
							}
							break;
						}
						if (GetKeyDown (InputKeyCode.Return)) 
						{
							yield return 0;
							AfterSelectedIndexUpdate (menu);
							break;
						}
					}
				}
			}
			if (GetKeyDown (InputKeyCode.Return)) 
			{
				UIQuestionMenu menu2=UIManager.instance.confirmMenu;
				menu2.SetText ("确定结束回合?");
				BeforeSelectedIndexUpdate (menu2);
				while (true)
				{
					yield return 0;
					SelectedIndexUpdate (menu2);
					if (GetKeyDown (InputKeyCode.OK)) 
					{
						AfterSelectedIndexUpdate(menu2);
						if (m_SelectedIndex == 0) {//确定 
							RoundEnd ();										
							yield break;
						}
						else//取消
						{
							break;
						}
					}
					if (GetKeyDown (InputKeyCode.Return)) 
					{
						yield return 0;
						AfterSelectedIndexUpdate(menu2);
						break;
					}
				}
			}
		}
	}
	private IEnumerator BeforeMove()
	{
		UIDropMenu menu = UIManager.instance.beforeMoveMenu;
		menu.SetPosition (GameManager.instance.CurrentCharacter.transform.position);
		BeforeSelectedIndexUpdate (menu);
		while (true) 
		{
			yield return 0;
			SelectedIndexUpdate (menu);
			if (GetKeyDown (InputKeyCode.OK)) 
			{
				AfterSelectedIndexUpdate(menu);
				if (m_SelectedIndex == 0) {//移动 
					GameManager.instance.NextStep ();
					yield break;
				} 
				else if (m_SelectedIndex == 1) {//技能
					if (GameManager.instance.CurrentCharacter.activeSkillList.Count == 0) //没技能
					{
						UIManager.instance.Warning ("此角色没有主动技能");
						BeforeSelectedIndexUpdate (menu);
					}
					else//有技能
					{
						UIDropMenu menu2 = UIManager.instance.skillMenu;
						menu2.SetPosition(GameManager.instance.CurrentCharacter.transform.position);
						for (int i = 0; i < GameManager.instance.CurrentCharacter.activeSkillList.Count; i++) 
						{
							menu2.AddElement (GameManager.instance.CurrentCharacter.activeSkillList[i].name);
						}
						BeforeSelectedIndexUpdate (menu2);
						while (true)  //SkillMenu循环
						{
							yield return 0;
							SelectedIndexUpdate (menu2);
							if (GetKeyDown (InputKeyCode.OK)) 
							{
								if(GameManager.instance.CurrentCharacter.activeSkillList [m_SelectedIndex].CanExert ())
								{
									menu2.Clear ();
									AfterSelectedIndexUpdate (menu2);
									ActiveSkill skill = GameManager.instance.CurrentCharacter.activeSkillList [m_SelectedIndex];
									yield return GameManager.instance.StartCoroutine(skill.StepList[0]());
									yield break;
								}
							}
							if (GetKeyDown (InputKeyCode.Return)) 
							{
								yield return 0;
								menu2.Clear ();
								AfterSelectedIndexUpdate (menu2);
								BeforeSelectedIndexUpdate (menu);
								break;
							}
						}
					}
				}
				else if (m_SelectedIndex == 2) //取消
				{
					GameManager.instance.PreviousStep();
					yield break;
				}
			}
			if(GetKeyDown (InputKeyCode.Return)) {
				GameManager.instance.PreviousStep();
				AfterSelectedIndexUpdate(menu);
				yield break;
			}
		}
	}
	private IEnumerator Move()
	{
		//设置攻击和移动区域
		List<Block> moveList=GameManager.instance.CurrentCharacter.GetMovableArea ();
		List<Block> hitList=GameManager.instance.CurrentCharacter.GetHitableArea ();
		foreach (Block block in moveList) 
		{
			block.SetMovable ();
		}
		foreach (Block block in hitList) 
		{
			block.SetHitable ();
		}
		foreach (Block block1 in moveList) 
		{
			foreach (Block block2 in hitList) 
			{
				if (block1 == block2) 
				{
					block1.SetHitableAndMovable ();
				}
			}
		}
		//移动
		while (true) 
		{
			yield return 0;
			PosSignMoveUpdate ();
			if (GetKeyDown (InputKeyCode.OK)) 
			{
				Block block=BlockMapManager.instance.GetBlock (m_ControllerPos);
				if (block.character == GameManager.instance.CurrentCharacter) //不移动
				{
					UIQuestionMenu menu=UIManager.instance.confirmMenu;
					menu.SetText ("确定放弃移动?");
					BeforeSelectedIndexUpdate (menu);
					//确定放弃移动
					while (true)
					{
						yield return 0;
						SelectedIndexUpdate (menu);
						if (GetKeyDown (InputKeyCode.OK)) 
						{
							AfterSelectedIndexUpdate(menu);
							if (m_SelectedIndex == 0) {//确定 
								BlockMapManager.instance.SetMapNormal ();
								GameManager.instance.NextStep ();
								yield break;
							}
							else//取消
							{
								break;
							}
						}
						if (GetKeyDown (InputKeyCode.Return)) 
						{
							yield return 0;
							AfterSelectedIndexUpdate(menu);
							break;
						}
					}
				}
				else if (moveList.Contains (block))//可移动 
				{
					Vector2 pos = GameManager.instance.CurrentCharacter.Pos;
					GameManager.instance.CurrentCharacter.SetPosition (block);
					UIQuestionMenu menu=UIManager.instance.confirmMenu;
					menu.SetText ("确定移动?");
					BeforeSelectedIndexUpdate (menu);
					//确认移动
					while (true)
					{
						yield return 0;
						SelectedIndexUpdate (menu);
						if (GetKeyDown (InputKeyCode.OK)) 
						{
							AfterSelectedIndexUpdate(menu);
							if (m_SelectedIndex == 0)//确定 
							{
								GameManager.instance.CurrentCharacter.SetPosition (BlockMapManager.instance.GetBlock(pos));
								BlockMapManager.instance.SetMapNormal ();
								yield return GameManager.instance.StartCoroutine(GameManager.instance.CurrentCharacter.Move (new ActionInformation(targetBlock:block)));
								GameManager.instance.NextStep ();
								yield break;
							}
							else//取消
							{
								break;
							}
						}
						if (GetKeyDown (InputKeyCode.Return)) 
						{
							yield return 0;
							AfterSelectedIndexUpdate(menu);
							break;
						}
					}
					GameManager.instance.CurrentCharacter.SetPosition (BlockMapManager.instance.GetBlock(pos));
				}
				else //无法移动
				{
					UIManager.instance.Warning ("无法移动到这里");
				}
			}
			if (GetKeyDown (InputKeyCode.Return)) 
			{
				BlockMapManager.instance.SetMapNormal ();
				GameManager.instance.PreviousStep();
				return false;
			}
		}
	}
	private IEnumerator BeforeHit()
	{
		UIDropMenu menu = UIManager.instance.beforeHitMenu;
		menu.SetPosition(GameManager.instance.CurrentCharacter.transform.position);
		BeforeSelectedIndexUpdate (menu);
		while (true) //BeforeHitMenu循环
		{
			yield return 0;
			SelectedIndexUpdate (menu);
			if (GetKeyDown (InputKeyCode.OK)) 
			{
				AfterSelectedIndexUpdate (menu);
				if (m_SelectedIndex == 0) {//攻击
					GameManager.instance.NextStep ();
					yield break;
				}
				else if (m_SelectedIndex == 1) {//技能
					if (GameManager.instance.CurrentCharacter.activeSkillList.Count == 0) //没技能
					{
						UIManager.instance.Warning ("此角色没有主动技能");
						BeforeSelectedIndexUpdate (menu);
					}
					else//有技能
					{
						UIDropMenu menu2 = UIManager.instance.skillMenu;
						menu2.SetPosition(GameManager.instance.CurrentCharacter.transform.position);
						for (int i = 0; i < GameManager.instance.CurrentCharacter.activeSkillList.Count; i++) 
						{
							menu2.AddElement (GameManager.instance.CurrentCharacter.activeSkillList[i].name);
						}
						BeforeSelectedIndexUpdate (menu2);
						while (true)  //SkillMenu循环
						{
							yield return 0;
							SelectedIndexUpdate (menu2);
							if (GetKeyDown (InputKeyCode.OK)) 
							{
								if(GameManager.instance.CurrentCharacter.activeSkillList [m_SelectedIndex].CanExert ())
								{
									menu2.Clear ();
									AfterSelectedIndexUpdate (menu2);
									ActiveSkill skill = GameManager.instance.CurrentCharacter.activeSkillList [m_SelectedIndex];
									yield return GameManager.instance.StartCoroutine(skill.StepList[0]());
									yield break;
								}
							}
							if (GetKeyDown (InputKeyCode.Return)) 
							{
								yield return 0;
								menu2.Clear ();
								AfterSelectedIndexUpdate (menu2);
								BeforeSelectedIndexUpdate (menu);
								break;
							}
						}
					}
				}
				else if(m_SelectedIndex ==2)//结束
				{
					GameManager.instance.NextStep (2);
					yield break;
				}
			}
		}
	}
	private IEnumerator Hit()
	{
		//设置攻击区域
		List<Block> hitList=GameManager.instance.CurrentCharacter.GetHitableArea ();
		foreach (Block block in hitList) 
		{
			block.SetHitable ();
		}
		//攻击
		while (true) 
		{
			yield return 0;
			PosSignMoveUpdate ();
			if (GetKeyDown (InputKeyCode.OK)) 
			{
				Block block = BlockMapManager.instance.GetBlock (m_ControllerPos);
				if (!hitList.Contains (block))
				{
					UIManager.instance.Warning ("不在攻击范围内");
					continue;
				}
				if (block.character == null)
				{
					UIManager.instance.Warning ("没有目标可攻击");
					continue;
				}
				if ( block.character.team == GameManager.instance.CurrentCharacter.team)
				{
					UIManager.instance.Warning ("无法伤害队友");
					continue;
				}
				UIQuestionMenu menu=UIManager.instance.confirmMenu;
				menu.SetText ("确定攻击?");	
				BeforeSelectedIndexUpdate (menu);
				//确认攻击
				while (true)
				{
					yield return 0;
					SelectedIndexUpdate (menu);
					if (GetKeyDown (InputKeyCode.OK)) 
					{
						AfterSelectedIndexUpdate(menu);
						if (m_SelectedIndex == 0)//确定 
						{
							BlockMapManager.instance.SetMapNormal ();
							yield return GameManager.instance.StartCoroutine(GameManager.instance.CurrentCharacter.Attack (new ActionInformation(recipient:block.character)));
							GameManager.instance.NextStep ();
							yield break;
						}
						else //取消
						{
							break;
						}
					}
					if (GetKeyDown (InputKeyCode.Return)) 
					{
						yield return 0;
						AfterSelectedIndexUpdate(menu);
						break;
					}
				}
			}
			if (GetKeyDown (InputKeyCode.Return)) 
			{
				BlockMapManager.instance.SetMapNormal ();
				GameManager.instance.PreviousStep ();
				return false;
			}
		}

	}
	private IEnumerator Over()
	{
		yield return 0;
		m_CoolingCharacterList.Add (GameManager.instance.CurrentCharacter);
		GameManager.instance.NextStep ();
	}
	//以下为Step 调用的函数
	private void PosSignMoveUpdate()
	{
		if (!Input.anyKeyDown)
			return;
		//处理方向输入
		if (GetKeyDown (InputKeyCode.Down)) 
		{
			m_ControllerPos.y--;
		}
		if (GetKeyDown (InputKeyCode.Left)) 
		{
			m_ControllerPos.x--;
		}
		if (GetKeyDown (InputKeyCode.Right)) 
		{
			m_ControllerPos.x++;
		}
		if (GetKeyDown (InputKeyCode.Up)) 
		{
			m_ControllerPos.y++;
		}
		//处理越界
		Block[,] map = BlockMapManager.instance.CurrentMap;
		if (m_ControllerPos.x < 0)
			m_ControllerPos.x = map.GetLength (0) - 1;
		if (m_ControllerPos.x > map.GetLength (0) - 1)
			m_ControllerPos.x = 0;
		if (m_ControllerPos.y < 0)
			m_ControllerPos.y = map.GetLength (1) - 1;
		if (m_ControllerPos.y > map.GetLength (1) - 1)
			m_ControllerPos.y = 0;
		//更新m_PosSign位置
		Block block=BlockMapManager.instance.GetBlock (m_ControllerPos);
		SceneManager.instance.posSign.transform.position = new Vector3(block.transform.position.x,0.01f,block.transform.position.z);
	}
	private void BeforeSelectedIndexUpdate(UIMenu menu)
	{
		menu.gameObject.SetActive (true);
		menu.SetAllNormal ();
		m_SelectedIndex = 0;
		menu.ElementList [m_SelectedIndex].SetSelected ();
	}	
	private void SelectedIndexUpdate(UIMenu menu)
	{		
		//处理方向输入
		if (GetKeyDown (InputKeyCode.Down)) {
			menu.ElementList[m_SelectedIndex].SetNormal();
			m_SelectedIndex++;
		} 
		else if (GetKeyDown (InputKeyCode.Up)) {
			menu.ElementList[m_SelectedIndex].SetNormal();
			m_SelectedIndex--;
		}
		else 
		{
			return;
		}
		//处理越界
		if (m_SelectedIndex < 0)
			m_SelectedIndex = menu.ElementList.Count - 1;
		if (m_SelectedIndex > menu.ElementList.Count - 1)
			m_SelectedIndex = 0;
		//更新Selected位置
		menu.ElementList[m_SelectedIndex].SetSelected();
	}
	private void AfterSelectedIndexUpdate(UIMenu menu)
	{
		menu.gameObject.SetActive (false);
	}
	public override bool GetKeyDown(InputKeyCode keycode)
	{
		if (!Input.anyKeyDown)
			return false;
		if (keycode == InputKeyCode.Down) 
		{
			if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) 
			{
				return true;
			}
			return false;
		}
		if (keycode == InputKeyCode.Left) 
		{
			if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) 
			{
				return true;
			}
			return false;
		}
		if (keycode == InputKeyCode.OK) 
		{
			if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.Return)) 
			{
				return true;
			}
			return false;
		}
		if (keycode == InputKeyCode.Return) 
		{
			if (Input.GetKeyDown (KeyCode.Tab)) 
			{
				return true;
			}
			return false;
		}

		if (keycode == InputKeyCode.Right) 
		{
			if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) 
			{
				return true;
			}
			return false;
		}

		if (keycode == InputKeyCode.Up) 
		{
			if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) 
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
