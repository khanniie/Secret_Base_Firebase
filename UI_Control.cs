using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Control : MonoBehaviour {
	public GameObject block_holder;
	public GameObject edit_holder;
	public Toggle t;
	//private GameObject[] B;

	void Awake(){
		
	}

//	// Use this for initialization
	void Start () {
	}

	public void toggle_block_interactivity(){
		if (t.isOn) {
			block_holder.SetActive (true);
			edit_holder.SetActive (true);
		} else {
			block_holder.SetActive (false);
			edit_holder.SetActive (false);
		}
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}
}
