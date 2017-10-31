using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour {
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void fadeUIout()
    {
        this.GetComponent<Image>().CrossFadeAlpha(0, 2f, false);
        //this.GetComponent<Button>().enabled = false;
        //this.GetComponentInChildren<Image>().CrossFadeAlpha(0, 2f, false);
    }

    public void fadeUIin()
    {
        this.GetComponent<Image>().CrossFadeAlpha(1, 2f, false);
        if (this.GetComponent<Button>() != null) this.GetComponent<Button>().interactable = true;
    }

}
