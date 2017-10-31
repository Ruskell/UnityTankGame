using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firstLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("testLevel");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
