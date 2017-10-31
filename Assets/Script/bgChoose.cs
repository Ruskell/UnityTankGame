using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgChoose : MonoBehaviour {

	System.Random rand = new System.Random ();
	public Material [] matList = new Material[5];
    public int bgRand;

	void Start()
    {
        matList[0] = Resources.Load("boxBoatCrash", typeof(Material)) as Material;
        //matList[1] = Resources.Load("boxBoatCrash", typeof(Material)) as Material;
        matList[1] = Resources.Load("boxBurnt", typeof(Material)) as Material;
        matList[2] = Resources.Load("boxForestCity", typeof(Material)) as Material;
        matList[3] = Resources.Load("boxOvergrown", typeof(Material)) as Material;
        matList[4] = Resources.Load("boxPlane", typeof(Material)) as Material;
        bgRand = rand.Next(0, 5);
        this.GetComponent<Renderer>().material = matList [bgRand];
	}

}
