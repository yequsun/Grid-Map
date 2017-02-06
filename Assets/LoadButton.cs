using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {
    public GameObject bcObj;
    Grid g;
    Button btn;

    public GameObject unblocked;
    public GameObject blocked;
    public GameObject highway;
    public GameObject hard;
    public GameObject path;
	// Use this for initialization
	void Start () {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        bcObj = GameObject.Find("ButtonController");

	}

    void TaskOnClick()
    {



        g = new Grid();
        g.TestSearch();
        Grid.WriteToBinaryFile(Application.dataPath + @"\maps\map0_0.bin",g);
        Debug.Log("complete");

        
        string pathName = bcObj.GetComponent<ButtonController>().GetPath();
        g = Grid.ReadFromBinaryFile(pathName);
        for(int i = 0; i < 120; i++)
        {
            for(int j = 0; j < 160; j++)
            {

                GameObject tile = unblocked;
                if (!g.GetNode(i, j).isOptimal)
                {
                    switch (g.GetNode(i, j).GetCell())
                    {
                        case '0':
                            tile = blocked;
                            break;
                        case '1':
                            tile = unblocked;
                            break;
                        case '2':
                            tile = hard;
                            break;
                        case 'a':
                            tile = highway;
                            break;
                        case 'b':
                            tile = highway;
                            break;
                    }
                }
                else
                {
                    tile = path;
                    Debug.Log("find a path");
                }
                GameObject cur;
                cur = Instantiate(tile, new Vector3(10 * i, 0, 10 * j), Quaternion.identity);
                cur.GetComponent<TileInfo>().row = i;
                cur.GetComponent<TileInfo>().col = j;
                cur.GetComponent<TileInfo>().f = g.GetNode(i, j).f;
                cur.GetComponent<TileInfo>().g = g.GetNode(i, j).g;
                cur.GetComponent<TileInfo>().h = g.GetNode(i, j).h;

            }
        }
        Debug.Log(g.nodesExpanded);
        Debug.Log(g.GetNode(g.g_x, g.g_y).parent);




    }
}
