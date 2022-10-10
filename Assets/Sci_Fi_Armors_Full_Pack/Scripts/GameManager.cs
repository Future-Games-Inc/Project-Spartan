using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


public static string currentName;
	// Use this for initialization
	void Start ()
	{
		GameObject current = Instantiate(Resources.Load("Armor1")) as GameObject;
	string start = "Armor1";
	SetCurrent(start);
	 Debug.Log("Current model set to "+start);
	}
	
	public static string GetCurrent()
	{
	  return currentName;
    }
	public static void SetCurrent(string newCurrentName){
	currentName = newCurrentName;
	}
}
