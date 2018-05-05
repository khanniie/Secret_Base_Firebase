using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block{
	public int x;
	public int z;
	public int height;
	public bool dontplace;
	public string items;
	public UAuth AU;

	public Block(int xx, int yy){
		x = xx;
		z = yy;
		height = 0;
		dontplace = false;
		items = "";
	}
	public void ChangeHeight(int input){
		height += input;
	}

	public void addItem(string block, int scale){
		items += block + "-";
		height += scale;
	}

	public void removeItem(int index, int scale){
		string[] splitString = items.Split('-');
		splitString [index] = "";
		items = splitString.ToString ();
		height -= scale;
	}
}
