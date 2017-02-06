using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour {
    public GameObject unblocked;
    public GameObject blocked;
    public GameObject highway;
    public GameObject hard;
    string[] lines;
	// Use this for initialization
	void Start () {
        lines = System.IO.File.ReadAllLines(Application.dataPath + @"\maps\map1.txt");
        for(int i = 10; i < lines.Length; i++)
        {
            int a = 0;
            for (int j = 0; j < lines[i].Length; j++)
            {
                char cur = lines[i][j];
                GameObject tile = null;
                switch (cur)
                {
                    case '0':
                        tile = blocked;
                        a++;
                        break;
                    case '1':
                        tile = unblocked;
                        a++;
                        break;
                    case '2':
                        tile = hard;
                        a++;
                        break;
                    case 'a':
                        tile = highway;
                        a++;
                        break;
                    case 'b':
                        tile = highway;
                        a++;
                        break;
                }
                if (tile != null)
                {
                    Instantiate(tile, new Vector3(10 * a, 0, -10 * i), Quaternion.identity);
                }
            }
        }
	}

    void Generate(int[,] mapArray)
    {
        for(int i = 0; i < mapArray.GetLength(0); i++)
        {
            for(int j = 0; j < mapArray.GetLength(1); j++)
            {
                GameObject tile;
                if (mapArray[i, j] == 0)
                {
                    tile = unblocked;
                }
                else
                {
                    tile = blocked;
                }
                GameObject go= Instantiate(tile, new Vector3(10 * i, 0, 10 * j), Quaternion.identity);
                TileInfo info = go.GetComponent<TileInfo>();

            }
        }
    }
}
