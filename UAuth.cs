using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//A LOT OF THIS IS FROM THE UNITY FIREBASE AUTH QUICKSTART


public class UAuth : MonoBehaviour {
	
	protected Firebase.Auth.FirebaseAuth auth;
	private Firebase.Auth.FirebaseAuth otherAuth;
	protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
		new Dictionary<string, Firebase.Auth.FirebaseUser>();

	public InputField nameField;
	public InputField passwordField;
	public InputField emailField;
	public InputField codeField;
	public bool UserInit = false;

	private string logText = "";
	protected string email = "";
	protected string password = "";
	protected string displayName = "";
	private bool fetchingToken = false;
	private Dbase DB;



	// Options used to setup secondary authentication object.
	private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions {
		ApiKey = "",
		AppId = "",
		ProjectId = ""
	};
		
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther; 

	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
	public virtual void Start() {
		DB = GameObject.Find("database").GetComponent<Dbase>();
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available) {
				InitializeFirebase();
			} else {
				Debug.LogError(
					"Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});
	}

	// Handle initialization of the necessary firebase modules:
	void InitializeFirebase() {
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		auth.IdTokenChanged += IdTokenChanged;
		// Specify valid options to construct a secondary authentication object.
		if (otherAuthOptions != null &&
			!(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
				String.IsNullOrEmpty(otherAuthOptions.AppId) ||
				String.IsNullOrEmpty(otherAuthOptions.ProjectId))) {
			try {
				otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
					otherAuthOptions, "Secondary"));
				otherAuth.StateChanged += AuthStateChanged;
				otherAuth.IdTokenChanged += IdTokenChanged;
			} catch (Exception) {
				DebugLog("ERROR: Failed to initialize secondary authentication object.");
			}
		}
		AuthStateChanged(this, null);
	}

	protected virtual void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	void OnDestroy() {
//		auth.StateChanged -= AuthStateChanged;
//		auth.IdTokenChanged -= IdTokenChanged;
//		auth = null;
//		if (otherAuth != null) {
//			otherAuth.StateChanged -= AuthStateChanged;
//			otherAuth.IdTokenChanged -= IdTokenChanged;
//			otherAuth = null;
//		}
	}

	// Output text to the debug log text field, as well as the console.
	public void DebugLog(string s) {
		Debug.Log(s);
		logText += s + "\n";
	}

	// Display user information.
	void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel) {
		string indent = new String(' ', indentLevel * 2);
		var userProperties = new Dictionary<string, string> {
			{"Display Name", userInfo.DisplayName},
			{"Email", userInfo.Email},
			{"User ID", userInfo.UserId}
		};
		foreach (var property in userProperties) {
			if (!String.IsNullOrEmpty(property.Value)) {
				DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
			}
		}
	}

	// Track state changes of the auth object.
	void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
		Firebase.Auth.FirebaseUser user = null;
		if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
		if (senderAuth == auth && senderAuth.CurrentUser != user) {
			bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
			if (!signedIn && user != null) {
				DebugLog("Signed out " + user.UserId);
			}
			user = senderAuth.CurrentUser;
			userByAuth[senderAuth.App.Name] = user;
			if (signedIn) {
				DebugLog("Signed in " + user.UserId);
				displayName = user.DisplayName ?? "";
			}
		}
	}

//	 Track ID token changes.
	void IdTokenChanged(object sender, System.EventArgs eventArgs) {
		Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
		if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken) {
			senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
				task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
		}
	}
	public void createBuffer(string e, string p){
		auth.CreateUserWithEmailAndPasswordAsync (emailField.text, passwordField.text).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
//			Debug.LogFormat("Firebase user created successfully: {0} ({1})",
//				newUser.DisplayName, newUser.UserId);
			Debug.Log (auth.CurrentUser);
			UserInit = true;
			auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile {
					DisplayName = nameField.text,
			});
			DB.makeUser(auth.CurrentUser.UserId, nameField.text );
			if (auth.CurrentUser != null) {
				SceneManager.LoadScene("AR_block_build");
			} else {
				SceneManager.LoadScene("SignIn");
			}
		});
	}
		
	public void CreateUser() {
		UserInit = false;
		displayName = nameField.text;
		email = emailField.text;
		password = passwordField.text;
		Debug.Log (displayName + "    " + email + "   " + password);
		createBuffer(emailField.text,passwordField.text);

	}

	public void Signin() {
		auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWith(
			task => {
				if(auth.CurrentUser!=null){
					SceneManager.LoadScene("AR_block_build");
				}
				else{
					Debug.Log("time to cry tears of blood");
				}
			});
	}

	public string getUserId(){
		if (auth == null) {
			InitializeFirebase ();
		}
		if (auth.CurrentUser != null) {
			return (auth.CurrentUser.UserId);
		} else {
			SceneManager.LoadScene("SignIn");
			return "failed";
		}
	}

	//GET
	public void getFriendsFromId(string b){
		string id = getUserId ();
		if (!"failed".Equals (id)) {
			DB.getFriends(id, b);
		}
	}
	//ADD
	public void addFriendFromId(string b){
		string id = getUserId ();
		if (!"failed".Equals(id)) {
			DB.addFriend(id, codeField.text, b);
		}
	}

	public string getFriendCode(){
		string id = getUserId ();
		if (!"failed".Equals(id)) {
			string res = DB.getFriendId(id);
			Debug.Log(res);
			return res;
		}
		return "";
	}

	public void BaseExists(string targetname){
		DB.verifyBase (getUserId(), targetname);
	}

	public void setupListeners(GameObject obj){
		if (DB == null) {
			DB = GameObject.Find("database").GetComponent<Dbase>();
		}
		DB.setupListeners ("users/" + getUserId () + "/furniture/" + obj.name, obj);
	}

	public void initBase(string targetname){
		
		DB.initBase (getUserId(), targetname);
	}
	public void initGrid(string targetname, Block[] B){
		string[] S = new string[B.Length];

		for (int i = 0; i < B.Length; i++) {
			S[i] = JsonUtility.ToJson (B [i]);
		}

		DB.initGrid (getUserId(), targetname, S);
	}

	public void fetchBlockInfo(string name){
		Debug.Log ("fetching info");
		DB.fetchBlockInfo (getUserId(), name);
	}
	public void changeItem(string name, int index, string items, int height){
		DB.changeItem (getUserId (), name, index + "", items, height);
	}

}
