using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour {
    public Text infoText;
    public char type;
    public int row;
    public int col;
    public double f;
    public double g;
    public double h;

	// Use this for initialization
	void Start () {
        f = -1;
        g = -1;
        h = -1;
        infoText = GameObject.Find("Info").GetComponent<Text>();
	}

    void OnMouseOver()
    {
        //infoText.text = "Type: " + type + "\n" + "(" + col + "," + row + ")\n" + "f: " + f + "\n" + "g: " + g + "h: " + h;
        infoText.text = "row " + row.ToString() + " col " + col.ToString() + "\nf "+f +" g " + g +" h "+h;
    }
}
