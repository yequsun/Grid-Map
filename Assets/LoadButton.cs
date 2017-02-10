using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour {
    public GameObject bcObj;
    public Text pText;
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
        pText = GameObject.Find("Performance").GetComponent<Text>();

	}
    public void ResetGrid()
    {
        g = null;
    }

    void TaskOnClick()
    {



        //g = new Grid();
        //Grid.WriteToBinaryFile(Application.dataPath + @"\maps\map0_0.bin",g);
        //Debug.Log("complete");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        string pathName = bcObj.GetComponent<ButtonController>().GetPath();
        //Debug.Log(bcObj.GetComponent<ButtonController>().GetPath());
        //g = Grid.ReadFromBinaryFile(pathName);
        g = new Grid(pathName);
        g.TestSearch();
        pText.text = "Time: " + g.astar[0].ToString() + "\nNodes Expanded: " + g.astar[1].ToString() + "\nMax Fringe Size: " + g.astar[2].ToString();
        for (int i = 0; i < 120; i++)
        {
            for(int j = 0; j < 160; j++)
            {
                double f1, g1, h1;
                f1 = g.GetNode(i, j).f;
                g1 = g.GetNode(i, j).g;
                h1 = g.GetNode(i, j).h;

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
                }
                GameObject cur;
                cur = Instantiate(tile, new Vector3(10 * i, 0, 10 * j), Quaternion.identity);
                cur.GetComponent<TileInfo>().row = i;
                cur.GetComponent<TileInfo>().col = j;
                cur.GetComponent<TileInfo>().f = f1;
                cur.GetComponent<TileInfo>().g = g1;
                cur.GetComponent<TileInfo>().h = h1;

            }
        }





    }
}
