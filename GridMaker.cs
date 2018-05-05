using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour {
	public GameObject gridPrefab;
	public int row = 10;
	public int col = 10;
	Transform selected_tile;
	public Transform selected_tile_2;
	public bool in_range;
	public int prev_x;
	public int prev_z;
	private Block[] heights;
	private bool heights_init;
	public Slider s;
	public GameObject grid;
	private bool editmode;
	public Toggle editmode_switch;
	private int scale; 
	public GameObject B;
	public bool grid_init;

	void Start(){
		//editmode = false;
		grid_init = false;
		editmode_switch.isOn = false;
		toggle_grid ();
	}


	public void StartScript (int on) {
		scale = 2;
		prev_x = -1 * scale;
		prev_z = -1 * scale;
		heights_init = false;
		if (!grid_init) {
			Debug.Log ("running start script, making new grid");
			grid_init = true;
			MakeGrid ();
		}

		//editmode_switch.isOn = true;
			//toggle_grid ();
	}

	void MakeGrid(){
		Transform grid_t = grid.transform;
		for (int r = 0; r < row; r++)
		{
			for (int c = 0; c < col; c++)
			{	 Instantiate (gridPrefab, scale * (new Vector3(r -row/2, 0, c- col/2)), new Quaternion(0, 0, 0, 1), grid_t);
				GameObject clone = Instantiate (gridPrefab, 2 * (new Vector3(r -row/2, 0, c- col/2)), new Quaternion(0, 0, 0, 1), grid_t);
				 clone.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			}
		}

		B.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		B.transform.localPosition = new Vector3(0f, row/2, -3f);

	}

	void SetTargetInvisible(GameObject Target)
	{
		foreach (Transform aaa in Target.transform)
			if (aaa.gameObject.GetComponent<Renderer>())
				aaa.gameObject.GetComponent<Renderer>().enabled = false;
	}

	void SetTargetVisible(GameObject Target)
	{
		foreach (Transform aaa in Target.transform)
			if (aaa.gameObject.GetComponent<Renderer>())
				aaa.gameObject.GetComponent<Renderer>().enabled = true;
	}

	public void toggle_grid(){
		Debug.Log ("toggle toggle");
		if (editmode_switch.isOn) {
 			grid.SetActive (true);

			selected_tile_2.gameObject.SetActive (true);
			editmode = true;


			//			editmode = true;-0
//			grid.GetComponent<Renderer>().enabled = true;
//			GameObject.Find ("selected_2").GetComponent<Renderer>().enabled = true;
//			SetTargetVisible(GameObject.Find ("selected_2"));
//			SetTargetVisible (grid);

		} else {
			editmode =false;
			grid.SetActive (false);
			selected_tile_2.gameObject.SetActive (false);

//			SetTargetInvisible(GameObject.Find ("selected_2"));
//			SetTargetInvisible (grid);
		}
	}

	public void init_heights(Block[] b){
		heights = b;
		heights_init = true;
	}

	public int single_dim_index(int x, int z){
		return x * 10 + z;
	}

	public void positionPlane(){
		float pos = s.value;
		B.transform.localPosition = new Vector3(0f, 0f, pos);
	}

	void Update () {

		if (!editmode) {
			return;
		}

		Vector3 Cposition = Camera.main.gameObject.transform.position;

		float rot = Camera.main.gameObject.transform.eulerAngles.y;

		int x = (int)Mathf.Round(Cposition.x);
		int z = (int)Mathf.Round(Cposition.z) * 2;

		float y = 0.01f;

			
		//front
		if (rot > 337.5f || rot <= 22.5f) {
			z++;
		}
		//front right
		else if (rot > 22.5f && rot <= 67.5f) {
			x++;
			z++;
		}
		//right
		else if (rot > 67.5f && rot <= 112.5f) {
			x++;
		}
		//right bottom
		else if (rot > 112.5f && rot <= 157.5f) {
			x++;
			z--;
		}
		//bottom
		else if (rot > 157.5f && rot <= 202.5f) {
			z--;
			//z2--;
		}
		//bottom left
		else if (rot > 202.5f && rot <= 247.5f) {
			z--;
			x--;
		}
		//left
		else if (rot > 247.5f && rot <= 292.5f) {
			x--;
		}
		//front left
		else if (rot > 292.5f && rot <= 337.5f) {
			z++;
			x--;
		}
			
		if (x >= -row / 2 && x < row / 2 && z >= -col / 2 && z < col / 2) {
			if (heights_init) {
				y += heights[single_dim_index(x + row/2, z + col/2)].height;
				//Debug.Log (y);
			}
			prev_x = x;
			prev_z = z;
			selected_tile_2.localPosition = (new Vector3 (2 * x, y, 2 * z));
			in_range = true;
		} else {
			in_range = false;
		}
	}
}

//int x2 = (int)Mathf.Round(relativePosition.x) + row/2;
//int z2 = (int)Mathf.Round(relativePosition.z) + col/2;
//Vector3 relativePosition = Camera.main.transform.InverseTransformDirection
//	(Cposition - GameObject.Find ("Building").transform.position);
////Debug.Log (Cposition + "  rel:" + relativePosition) ;
