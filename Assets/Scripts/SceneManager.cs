using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
	public static SceneManager instance;
	public Transform posSign;
	public Transform knightArrow;
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
