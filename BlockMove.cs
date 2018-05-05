using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockMove : MonoBehaviour {
	private bool holding;
	private GameObject curr_block;
	public GameObject big_mommy;
	public GridMaker G;
	public Block[] blocks;
	private int scale;
	private UAuth Au;
	private bool editmode;
	public GameObject building;
	public GameObject newButton;
	public string current_target_name;

	public int single_dim_index(int x, int z){
		return x * 10 + z;
	}

	public void block(string name){
		if (holding) {
			return;
		}
		GameObject block = Resources.Load("furniture/" + name, typeof(GameObject)) as GameObject;
		holding = true;
		curr_block = Instantiate (block, Camera.main.transform.position + Camera.main.transform.forward * 5, 
			new Quaternion(0, 0, 0, 1));
		curr_block.transform.eulerAngles = new Vector3 (0,
			Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
		curr_block.transform.SetParent(Camera.main.transform);
	}

	public void place(){
		
		if (curr_block == null)
			return;
		if (G.in_range) {
			holding = false;
			int x = G.prev_x;
			int z = G.prev_z;

			curr_block.transform.parent = big_mommy.transform;
			curr_block.transform.localPosition = new Vector3 (scale * x, 
					blocks[single_dim_index(x + G.row/2, z + G.col/2)].height, scale * z);
			curr_block.transform.localRotation = new Quaternion (0, 0, 0, 1);
			Block b = blocks [single_dim_index (x + G.row / 2, z + G.col / 2)];
			b.addItem(curr_block.name, scale);
			Au.changeItem (current_target_name, (x +  G.row / 2)*10+(z +  G.col / 2), b.items, b.height);

			curr_block = null;
		}
	}

	void Start(){
		editmode = false;
	}


	public void trash(){
		GameObject.Destroy (curr_block);
		curr_block = null;
		holding = false;
	}
		
	public void StartScript () {
		Debug.Log ("clicked");
		newButton.SetActive (false);
		scale = 2;
		holding = false;
		curr_block = null;

		Au.initBase(current_target_name);

		//G = GameObject.Find("Grid").GetComponent<GridMaker>();
		G.StartScript (1);
		blocks = new Block[100];
		for (int i = 0; i < G.row; i++) {
			for (int j = 0; j < G.col; j++) {
				blocks [single_dim_index(i, j)] = new Block (i, j);
			}
		}
		Au.initGrid (current_target_name, blocks);

		G.init_heights (blocks);
		editmode = true;

	}
	public void ContinueScript(string targetname){
		holding = false;
		curr_block = null;
		scale = 2;
		Au.fetchBlockInfo (targetname);
		//editmode = true;
		G.StartScript (1);
	}
	public void ContinueWithBlocks(List<string> S){
		Block[] B = new Block[100];
		string[] arr = S.ToArray();
		for (int i = 0; i < arr.Length; i++) {
			Block temp = JsonUtility.FromJson<Block> (arr [i]);
			B [temp.x * 10 + temp.z] = temp;
		}
		blocks = B;
		setBlocks (blocks);
	}

	public void printinfo(Block[] B){
		for (int i = 0; i < B.Length; i++) {
			Debug.Log ("x:  " + B[i].x + " z: " + B[i].z + "items: " + B[i].items);
		}
	}

	public void setBlocks(Block[] B){
		for (int i = 0; i < B.Length; i++) {
			string items = B[i].items;
			if (!(items.Equals (""))) {
				Debug.Log("special");
			}
			string[] splitString = items.Split(new [] { '-' }, StringSplitOptions.RemoveEmptyEntries);
			for (int j = 0; j < splitString.Length; j++) {
				splitString [j] = splitString [j].Replace("(Clone)", "");
				GameObject block = Resources.Load("furniture/" + splitString [j], typeof(GameObject)) as GameObject;
				GameObject b = Instantiate (block, new Vector3 (scale * B[i].x, 0, scale * B[i].z), 
					new Quaternion(0, 0, 0, 1));
   			    b.transform.parent = big_mommy.transform;
				b.transform.localPosition = new Vector3 (scale * (B[i].x - G.row/2), j, scale * (B[i].z- G.col/2));
				b.transform.localRotation = new Quaternion (0, 0, 0, 1);
			}
		}
	}

	public void targetFound(string targetName){
		Au = GameObject.Find("Auth").GetComponent<UAuth>();
		targetName = targetName.Replace("\n", String.Empty);
		Au.BaseExists(targetName);
	}

	public void ContinueBaseAction(bool res, string targetName){
		Debug.Log ("called continue");
		if (res) {
			Debug.Log (targetName + "exists");
			ContinueScript (targetName);
		}
		else{
			newButton.SetActive(true);
		}
		current_target_name = targetName;
	}
	public void AddFriend(){
		if(current_target_name != null && !current_target_name.Equals(""))
			Au.addFriendFromId (current_target_name);
	}
	// Update is called once per frame
	void Update(){
		if(!holding && editmode){
			if (Input.GetMouseButtonDown (0) ||  (Input.touchCount > 0)) {    
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit, 100)) {
					// whatever tag you are looking for on your game object
					if(hit.collider.tag == "Block" && !holding) {
						GameObject temp = hit.transform.gameObject; 
						if (temp == curr_block)
							return;
							
						int x = (int)temp.transform.localPosition.x;
						int z = (int)temp.transform.localPosition.z;
						Block b = blocks [single_dim_index (x / 2 + G.row / 2, z / 2 + G.col / 2)];
						b.removeItem((int)temp.transform.localPosition.y, scale);
						Au.changeItem (current_target_name, x*10+z, b.items, b.height);

						temp.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 6;
						temp.transform.eulerAngles = new Vector3 (0,
						Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
						temp.transform.SetParent(Camera.main.transform);

						curr_block = temp;
						holding = true;
				 }
				}    
			}
		}
	}
		
}
