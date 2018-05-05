using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;

public class Init : MonoBehaviour {
	private UAuth Au;
	public Text friendbox;
	public Text friendcode;
	// Use this for initialization
	void Start () {
		Au = GameObject.Find("Auth").GetComponent<UAuth>();
		//Au.getFriendsFromId ();
		//Debug.Log (temp);
		//friendbox.text = temp;
		friendcode.text = Au.getFriendCode();
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}
}
