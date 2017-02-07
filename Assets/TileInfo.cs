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
        infoText = GameObject.Find("Info").GetComponent<Text>();
        f = System.Math.Round(f, 2);
        g = System.Math.Round(g, 2);
        h = System.Math.Round(h, 2);
    }

    void OnMouseOver()
    {
        //infoText.text = "Type: " + type + "\n" + "(" + col + "," + row + ")\n" + "f: " + f + "\n" + "g: " + g + "h: " + h;
        infoText.text = "row " + row.ToString() + " col " + col.ToString() + "\nf "+f.ToString() +" g " + g.ToString() +" h "+h.ToString();
    }
}
