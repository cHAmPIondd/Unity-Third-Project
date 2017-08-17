using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;
public abstract class Controller  {
	public List<CharacterMono> CharacterList{ get; protected set;}
	public List<Step> StepList{ get;protected set;}
	public Controller()
	{
		CharacterList=new List<CharacterMono>();
		StepList = new List<Step> ();
	}
	public abstract void RoundStart();
	public abstract void RoundEnd();
	public abstract void AddCharacter (CharacterID id);
	public abstract bool GetKeyDown(InputKeyCode keycode);

}
