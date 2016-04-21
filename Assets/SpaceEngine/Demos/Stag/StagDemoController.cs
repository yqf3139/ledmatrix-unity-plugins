using UnityEngine;
using System.Collections.Generic;

public class StagDemoController : MonoBehaviour, IMeshEventListener {

    MeshSkinObject self;
    Animation a;
    Vector3 target;

    // Use this for initialization
    void Start () {
        a = GetComponent<Animation>();
        self = GetComponentInChildren<MeshSkinObject>();
        self.listener = this;

        foreach (AnimationState aa in a)
        {
            a[aa.name].wrapMode = WrapMode.Once;
        }
    }
	
	// Update is called once per frame
	void Update () {
        self.transform.eulerAngles = Vector3.Lerp(self.transform.eulerAngles, target, Time.deltaTime);
    }

    public void MeshObjectUpdate(MeshObjectUpdateStatus s)
    {

    }

    public void OnCrowdInfo(CrowdInfo[] infos)
    {

    }

    public void OnCrowdInfoSummry(CrowdInfoSummry summary)
    {
        float sin = Mathf.Asin(summary.crowdDirection.z);
        float cos = Mathf.Acos(summary.crowdDirection.x);

        float angle = cos / Mathf.PI * 180;
        if (sin < 0)
        {
            angle = 360 - angle;
        }

        target = new Vector3(0, (angle + 90f) % 360, 0);

        if (Mathf.Abs (target.y - self.transform.eulerAngles.y) > 180 )
        {
            if (target.y > self.transform.eulerAngles.y)
                target.y -= 360;
            else
                target.y += 360;
        }
    }

    public void OnInteractionInput(WorldEvent e)
    {
        if (a.isPlaying)
        {
            return;
        }
        switch (e.gesture)
        {
            case KinectGestures.Gestures.Stop:
                a.Play("idle1");
                break;
            case KinectGestures.Gestures.Wave:
                a.Play("walk");
                a.PlayQueued("Take 001");
                break;
            case KinectGestures.Gestures.Jump:
                a.Play("run");
                a.PlayQueued("Take 001");
                break;
            case KinectGestures.Gestures.Push:
                a.Play("hornAttack1");
                a.PlayQueued("Take 001");
                break;
            case KinectGestures.Gestures.Pull:
                a.Play("hornAttack2");
                a.PlayQueued("Take 001");
                break;
            default:
                break;
        }

    }
}
