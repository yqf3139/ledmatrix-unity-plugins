using UnityEngine;
using System.Collections;

public class GiraffeDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0.5f, 0);
        float z = transform.position.z - 0.05f;
        if (z < -15f) z = 15;
        transform.position = new Vector3(0, transform.position.y, z);
    }
}
