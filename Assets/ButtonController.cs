using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {
    public int gridNum;
    public int versionNum;
    string pathname;

	// Use this for initialization
	void Start () {
	}

    void Update()
    {
        pathname = Application.dataPath + @"\maps\map" + gridNum + "_" + versionNum + ".bin";
    }

    public string GetPath()
    {
        return pathname;
    }



}
