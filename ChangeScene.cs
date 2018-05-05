using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

     public void GoTo(string sceneName) 
		{ SceneManager.LoadScene(sceneName); }

	public void Login(){
		GoTo ("SignIn");
	}
	public void NewAcc(){
		GoTo ("NewAcc");
	}
	public void Build(){
		GoTo ("AR_block_build");
	}
	public void Home(){
		GoTo ("HomePage");
	}
}
