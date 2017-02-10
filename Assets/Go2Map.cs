using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Go2Map : MonoBehaviour {
    public string sceneName;
    public Button btn;

	// Use this for initialization
	void Start () {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
		
	}
	
    void TaskOnClick()
    {
        SceneManager.LoadScene(sceneName);
    }
}
