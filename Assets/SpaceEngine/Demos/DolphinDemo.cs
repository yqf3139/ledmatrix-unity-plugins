using UnityEngine;
using System.Collections;

public class DolphinDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float y = transform.position.y - 0.1f;
        if (y < -7f) y = 35;
        transform.position = new Vector3(0, y, transform.position.z);
    }
}
