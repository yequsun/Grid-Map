using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionButton : MonoBehaviour {
    public GameObject buttonController;
    public int id;
    Button btn;
    
    // Use this for initialization
    void Start () {
        buttonController = GameObject.Find("ButtonController");
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void TaskOnClick()
    {
        buttonController.GetComponent<ButtonController>().versionNum = id;
    }
}
