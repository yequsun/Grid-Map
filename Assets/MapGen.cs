using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapGen : MonoBehaviour {
    public int id;
    string pathname;
    Button btn;

    public GameObject unblocked;
    public GameObject blocked;
    public GameObject highway;
    public GameObject hard;
    public GameObject path;

    // Use this for initialization
    void Start () {
        pathname =Application.dataPath+ @"\maps\map";
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
	}
	
	// Update is called once per frame

    void TaskOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Grid g = new Grid();
        pathname = pathname + id.ToString() + "_";
        for (int i = 0; i < 10; i++)
        {
            g.SetStartAndGoal();
            //g.TestSearch();
            //string subpathname = pathname + i.ToString()+".bin";
            string subpathname = pathname + i.ToString() + ".txt";
            //Grid.WriteToBinaryFile(subpathname, g);
            g.OutputGrid(subpathname);
            //Debug.Log("one complete");
        }

        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < 160; j++)
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
                }
                GameObject cur;
                cur = Instantiate(tile, new Vector3(10 * i, 0, 10 * j), Quaternion.identity);
            }
        }
    }
}
