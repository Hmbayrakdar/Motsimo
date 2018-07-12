using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    public GameObject kutu;
    float x = 0.1f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void buyu()
    {
        kutu.transform.localScale += new Vector3(0 , x , 0);
    }
}
