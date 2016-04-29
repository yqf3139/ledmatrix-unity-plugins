using UnityEngine;
using System.Collections;

public class FoodController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y > 3)
        {
            Vector3 v = transform.position;
            v.y -= 50 * Time.deltaTime;
            transform.position = v;
        }
	}
}
