using UnityEngine;
using System.Collections;

public class anim : MonoBehaviour {

    // Use this for initialization
    public Animator aa;
	void Start () {
        aa = GetComponent<Animator>();
        aa.speed = 0.4f;
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(0, -0.5f, 0);
        aa.Play("Swim", 0);
    }
}
