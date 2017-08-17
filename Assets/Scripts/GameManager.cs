using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public PlayerController PlayerController{ get;private set;}
	public EnemyAIController EnemyAIController{ get;private set;}
	public Controller CurrentContrller{get;private set;}
	public CharacterMono CurrentCharacter{ get;set;}
	public int CurrentStepIndex{get;set;}
	private bool m_IsFighting;
	void Awake()
	{
		UIManager.instance.Init ();
		instance = this;
		PlayerController = new PlayerController ();
		EnemyAIController = new EnemyAIController ();
		CurrentContrller = PlayerController;
		GameStart ();
	}
	// Use this for initialization
	void Start () {
		FightStart ();
		CurrentContrller.RoundStart ();
	}
	// Update is called once per frame
	void Update () {
		
	}
	private void GameStart()
	{
		PlayerController.AddCharacter (CharacterID.Knight);
		PlayerController.AddCharacter (CharacterID.Cowboy);
		PlayerController.AddCharacter (CharacterID.Knight);
		PlayerController.AddCharacter (CharacterID.Swordman);
		EnemyAIController.AddCharacter (CharacterID.FirstEnemy);
		EnemyAIController.AddCharacter (CharacterID.SecondEnemy);
		EnemyAIController.AddCharacter (CharacterID.FirstEnemy);
	}
	private void FightStart()
	{
		BlockMapManager.instance.CreatedMap (8,8);
		PlayerController.FightStart ();
		EnemyAIController.FightStart ();
		m_IsFighting = true;
	}
	private void FightEnd()
	{
		for (int i=0;i<CharacterMono.allStaticSkillList.Count;i++) 
		{
			if ((CharacterMono.allStaticSkillList[i].skillType & SkillType.FightTime)!=0) 
			{
				CharacterMono.allStaticSkillList.RemoveAt (i);
				i--;
			}
		}
		m_IsFighting = false;
	}
	public void NextStep(int index=1)
	{
		CurrentStepIndex=(CurrentStepIndex+index)%CurrentContrller.StepList.Count;
		StartCoroutine (CurrentContrller.StepList[CurrentStepIndex]());
	}
	public void PreviousStep() 
	{
		CurrentStepIndex=(CurrentStepIndex-1+CurrentContrller.StepList.Count)%CurrentContrller.StepList.Count;
		StartCoroutine (CurrentContrller.StepList[CurrentStepIndex]());
	}
	public void NextTeam()
	{
		if (CurrentContrller == PlayerController) {
			CurrentContrller = EnemyAIController;
		} 
		else {
			CurrentContrller = PlayerController;
		}
		CurrentContrller.RoundStart ();
	}
	public bool GetKeyDown(InputKeyCode keyCode)
	{
		return CurrentContrller.GetKeyDown (keyCode);
	}
}
