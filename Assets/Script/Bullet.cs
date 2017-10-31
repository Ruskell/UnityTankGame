using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    bool hit = false;
    float time = 0;

	// Use this for initialization
	void Start () {
        DestroyObject(gameObject, 5);
        print("Bullet Created");
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime * 30;
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "careDrop") return; 
        if (!hit && time > 1)
        {
            hit = true;
            if ((col.gameObject.tag == "tankRight" || col.gameObject.tag == "turretRight") && !GameObject.FindGameObjectWithTag("tankRight").GetComponent<tankManager>().active)
            {
                if (PlayerPrefs.GetInt("level") == 1) GameObject.FindGameObjectWithTag("tankRight").GetComponent<tankManager>().updateHealth(40);
                else GameObject.FindGameObjectWithTag("tankRight").GetComponent<tankManager>().updateHealth(-40);
                print("Bullet Hit tankRight!");
            }
            else if ((col.gameObject.tag == "tankLeft" || col.gameObject.tag == "turretLeft") && !GameObject.FindGameObjectWithTag("tankLeft").GetComponent<tankManager>().active)
            {
                if (PlayerPrefs.GetInt("level") == 1) GameObject.FindGameObjectWithTag("tankLeft").GetComponent<tankManager>().updateHealth(40);
                else GameObject.FindGameObjectWithTag("tankLeft").GetComponent<tankManager>().updateHealth(-40);
                print("Bullet Hit tankLeft!");
            }
            else print("ELSE Bullet Hit " + col.name);
            Destroy(gameObject);
            GameObject.Find("mainCam").GetComponent<mainMenu>().swapTurn();
        }
        else print("trigger called when it shouldnt be");
    }
}
