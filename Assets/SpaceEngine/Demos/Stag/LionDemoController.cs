using UnityEngine;
using System.Collections;

public class LionDemoController : AnimalDemoController {

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void PlayDefault()
    {
        a.Play("idle");
    }

    public override void OnInteractionInput(WorldEvent e)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (a.isPlaying && !a.IsPlaying("idle"))
        {
            return;
        }
        switch (e.gesture)
        {
            case KinectGestures.Gestures.Stop:
                //a.Play("idle1");
                break;
            case KinectGestures.Gestures.Wave:
                a.Play("walk");
                a.PlayQueued("idle");
                break;
            case KinectGestures.Gestures.Jump:
                a.Play("run");
                a.PlayQueued("idle");
                break;
            case KinectGestures.Gestures.Push:
                //a.Play("hornAttack1");
                //a.PlayQueued("Take 001");
                break;
            case KinectGestures.Gestures.Pull:
                //a.Play("hornAttack2");
                //a.PlayQueued("Take 001");
                break;
            default:
                break;
        }

    }

    public override void Eat(GameObject food, Vector3 dir)
    {
        if (isEatting)
        {
            return;
        }
        if (!isFeeding)
        {
            return;
        }
        base.Eat(food, dir);
        a.Play("bite");
        a.PlayQueued("bite");
        a.PlayQueued("idle");
    }

    public override bool isActionDone()
    {
        return !(a.isPlaying && !a.IsPlaying("idle"));
    }
}
