using UnityEngine;
using System.Collections;

public class anim : MonoBehaviour {

    // Use this for initialization
    public Animator aa;
	void Start () {
        aa = GetComponent<Animator>();
        aa.speed = 0.4f;
        aa.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    // Update is called once per frame
    void Update () {
        aa.Play("Swim", 0);
    }
}
