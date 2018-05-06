using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Dbase: MonoBehaviour {

	private Firebase.Database.FirebaseDatabase fbref;
	Firebase.Database.DatabaseReference fb;
	public Text yourcode;
	//public Text FriendBox;
	public Text friendList;
	public BlockMove B;

	private void Awake() {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(
			"YOUR_URL";

		fbref = Firebase.Database.FirebaseDatabase.DefaultInstance;
		fb = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

	}
		


	private int randomCode() {

		int rand = UnityEngine.Random.Range(0, 10000);
		while (present("friend_codes/" + rand)) {
			rand = UnityEngine.Random.Range(0, 10000);
		}
		return rand;
	}

	public bool present(string path) {
		fbref.GetReference(path).GetValueAsync().ContinueWith(task => {
			return task.Result.Exists;
		});
		return false;
	}

	public void makeUser(string userid, string displayname) {
		string coderef = "" + randomCode();

		fb.Child("friend_codes").Child(coderef).SetValueAsync(userid);
		fb.Child("users").Child(userid).Child("name").SetValueAsync(displayname);
		fb.Child("users").Child(userid).Child("friend_code").SetValueAsync(coderef);
	}

	public void addFriend(string userid, string code, string b) {
		string friendid;
		string friendname;
		fbref.GetReference("friend_codes/" + code).GetValueAsync().ContinueWith(task => {
			//get id of friend
			friendid = task.Result.Value.ToString();

			fbref.GetReference("users/" + friendid + "/name").GetValueAsync().ContinueWith(task2 => {
				//get name of friend
				friendname = task2.Result.Value.ToString();
				//set friend name into database
				fb.Child("users").Child(userid).Child("bases").
					Child(b).Child("friends").Push().SetValueAsync(friendid);
				//get base info from user
				fb.Child("users").Child(userid).Child("bases").
					Child(b).GetValueAsync().ContinueWith(task4 => {
						//set base info into friend
						fb.Child("users").Child(friendid).Child("bases").Child(b).SetValueAsync(task4.Result.Value);
				});
			});
		});
	}

	public bool verifyFriend(string friend_code) {
		fbref.GetReference("friend_codes/" + friend_code).GetValueAsync().ContinueWith(task => {
			return task.Result.Exists;
		});
		return false;
	}

	public void verifyBase(string userId, string targetName) {
		fbref.GetReference("users/" + userId + "/bases/" + targetName).GetValueAsync().ContinueWith(task => {
			Debug.Log(task.Result);
			B.ContinueBaseAction(task.Result.Exists, targetName);
		});
	}

	public string getName(string userId) {
		fbref.GetReference("users/" + userId + "/name").GetValueAsync().ContinueWith(task => {
			return task.Result.Value;
		});
		return "";
	}

	public string getFriendId(string userId) {
		fbref.GetReference("users/" + userId + "/friend_code").GetValueAsync().ContinueWith(task => {
			Debug.Log(task.Result.Value);
			yourcode.text = "Your Friend Code:" + task.Result.Value;
			return task.Result.Value;
		});
		return "";
	}


	public List < string > getFriends(string friendid, string b) {

		List < string > friend_arr = new List < string > ();
		string friends = "";

		fb.Child("users").Child(friendid).Child("bases").Child(b).Child("friends").GetValueAsync().ContinueWith(task => {
			DataSnapshot res = task.Result;

			if (res.Exists && res.ChildrenCount > 0) {
				foreach(var childSnapshot in res.Children) {
					friends += childSnapshot.Value + "\n";
					friend_arr.Add(childSnapshot.Value.ToString());
				}
			}
			friendList.text = friends;
			return friend_arr;
		});
		return friend_arr;
	}

	public void setupListeners(string path, GameObject obj) {
		fbref.GetReference(path).ValueChanged += (object sender, ValueChangedEventArgs args) => {
			if (args.DatabaseError != null) {
				Debug.LogError(args.DatabaseError.Message);
				return;
			}
			float scale = float.Parse(args.Snapshot.Value.ToString());
			obj.transform.localScale += new Vector3(scale, scale, scale);
		};
	}
	public void initBase(string userid, string targetname) {
		targetname = targetname.Replace("\n", String.Empty);
		fbref.GetReference("users/" + userid + "/name").GetValueAsync().ContinueWith(task => {
			string name = task.Result.Value.ToString();
			fb.Child("users").Child(userid).Child("bases").Child(targetname).Child("friends").Push().SetValueAsync(userid);
		});
	}

	public void initGrid(string userid, string targetname, string[] blocks) {
		targetname = targetname.Replace("\n", String.Empty);
		for (int i = 0; i < blocks.Length; i++) {
			fb.Child("users").Child(userid).Child("bases").Child(targetname).Child("blocks").Child(i + "").SetRawJsonValueAsync(blocks[i]);
		}
	}

	public void fetchBlockInfo(string userId, string name) {

		fb.Child("users").Child(userId).Child("bases").Child(name).Child("blocks").GetValueAsync().ContinueWith(task => {
			DataSnapshot res = task.Result;
			List < string > S = new List < string > ();
			if (res.Exists && res.ChildrenCount > 0) {
				foreach(var childSnapshot in res.Children) {
					string temp = childSnapshot.GetRawJsonValue();
					S.Add(temp);
				}
			}
			B.ContinueWithBlocks(S);
		});
	}

	public void changeItem(string id, string name, string index, string items, int height) {
		Debug.Log("working.... : " + index + "   ");
		fbref.GetReference("users/" + id + "/bases/" + name + "/blocks/" + index + "/height").SetValueAsync(height);
		fbref.GetReference("users/" + id + "/bases/" + name + "/blocks/" + index + "/items").SetValueAsync(items);
		setFriend(id, name, index, height, items);
	}

	public void setFriend(string friendid, string targetname, string index, int value, string items) {
		fb.Child("users").Child(friendid).Child("bases").Child(targetname).Child("friends").GetValueAsync().
			ContinueWith(task => {
			DataSnapshot res = task.Result;
			if (res.Exists && res.ChildrenCount > 0) {
				foreach(var childSnapshot in res.Children) {
					fbref.GetReference("users/" + childSnapshot.Value.ToString() +"/bases/" + targetname +
						"/blocks/" + index + "/height").SetValueAsync(value);
					fbref.GetReference("users/" + childSnapshot.Value.ToString() +"/bases/" + targetname +
						"/blocks/" + index + "/items").SetValueAsync(items);
				}
			}
		});
	}



}



//		private void OnDestroy()  
//		{
//			_counterRef.ValueChanged -= OnCountUpdated;
//		}

//		public float getObjectSize(string userId, string objname){
//			fbref.GetReference ("users/" + userId + "/" + objname).GetValueAsync().ContinueWith
//				(task => {
//				return float.Parse(task.Result.Value.ToString);
//			});
//			return 0;
//		}

//Debug.Log ("fetching info");
//Block[] BB = new Block[100];
//		for (int i = 0; i < 100; i++) {
//			fbref.GetReference ("users/" + userId + "/bases/" + name + "/blocks/" + i).GetValueAsync ().ContinueWith
//			(task => {
//				DataSnapshot res = task.
//			});
//		}

//		public void addFriend(string friendid, string code){
//		string id2;
//		string name;
//		string name2;
//		   fbref.GetReference ("friend_codes/" +  code).GetValueAsync().ContinueWith
//				(task => {
//				id2 = task.Result.Value.ToString();
//				fbref.GetReference ("users/" + id2 + "/name").GetValueAsync().ContinueWith
//					(task2 => {
//						name2 = task2.Result.Value.ToString();
//						fbref.GetReference ("users/" + friendid + "/name").GetValueAsync().ContinueWith
//						(task3 => {
//							name = task3.Result.Value.ToString();
//							fb.Child("users").Child(friendid).Child("friends").Child(id2).SetValueAsync(name2);
//							fb.Child("users").Child(id2).Child("friends").Child(friendid).SetValueAsync(name);
//						});
//					});
//			});
//		}

//
//public void setObjectSize(string userId, string objname, float value){
//	fbref.GetReference ("users/" + userId + "/furniture/" + objname).SetValueAsync(value);
//	setFriendVal(userId, objname, value);
//}
