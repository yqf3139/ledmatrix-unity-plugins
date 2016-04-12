using UnityEngine;
using System.Collections.Generic;

public class RepeatAnimationController : MonoBehaviour {

    Animation a;

    IEnumerable<string> nextAnimation
    {
        get
        {
            while (true)
            {
                foreach (AnimationState aa in a)
                {
                    yield return aa.name;
                }
            }
        }
    }

    IEnumerator<string> ii;

    // Use this for initialization
    void Start () {
        Debug.Log("StagController Start");
        a = GetComponent<Animation>();
        ii = nextAnimation.GetEnumerator();
        ii.MoveNext();
        a.Play(ii.Current);
        // animate in batchmode
        a.cullingType = AnimationCullingType.AlwaysAnimate;
    }

    // Update is called once per frame
    void Update () {

        if (a.isPlaying)
        {
        }
        else
        {
            ii.MoveNext();
            a[ii.Current].wrapMode = WrapMode.Once;
            a.Play(ii.Current);
            Debug.LogError(ii.Current);
        }
    }
}
