using UnityEngine;
using System.Collections;
using System;

public interface UpSwimFish
{
    void swim();
}

public class FishUpSwimController : MonoBehaviour, UpSwimFish {

    // Use this for initialization
    public Animator aa;

    public Vector3 start = new Vector3();
    public Vector3 end = new Vector3(0, 10, 0);

    public float speed = 0.5f;
    public float progress = 0f;

    public void swim()
    {
        transform.position = start;
        progress = 0;
        gameObject.SetActive(true);
    }

    void Start()
    {
        aa = GetComponentInParent<Animator>();
        aa.speed = 0.8f;
        aa.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        transform.position = end;
        progress = 1;
    }

    // Update is called once per frame
    void Update()
    {
        progress += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(start, end, progress);
        if (V3Equal(end, transform.position))
        {
            gameObject.SetActive(false);
        }
        aa.Play("Swim", 0);
    }

    static bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
