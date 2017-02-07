using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour {
    Button btn;
    GameObject loadBtn;

	// Use this for initialization
	void Start () {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        loadBtn = GameObject.Find("LoadBtn");
	}
	
	// Update is called once per frame
    void TaskOnClick()
    {
        loadBtn.GetComponent<LoadButton>().ResetGrid();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
