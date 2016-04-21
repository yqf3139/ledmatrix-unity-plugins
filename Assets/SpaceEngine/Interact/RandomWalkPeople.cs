using UnityEngine;
using System.Collections;
using System;

public enum RandomWalkPeopleStatus { WALK, STOP }

public class RandomWalkPeople : MonoBehaviour {

    public static int NextID = 0;

    public int id = 0;
    public int gender = 0;
    public int age = 20;
    public int children = 0;
    public double lifeEndTime;

    Vector3 center;
    float stopTime = 0;
    float stopThreshold = 10f;
    float speed = 100f;
    float far;
    float near;
    float mid;
    float far2;
    float near2;
    float mid2;

    IInteractionInput input;

    Vector3 towards;
    Vector3 target;
    Vector3 start;
    RandomWalkPeopleStatus status = RandomWalkPeopleStatus.WALK;

    Array actionValues = Enum.GetValues(typeof(KinectGestures.Gestures));

    KinectGestures.Gestures[] useableGesture = new KinectGestures.Gestures[] {
        //KinectGestures.Gestures.None,
        //KinectGestures.Gestures.RaiseRightHand,
        //KinectGestures.Gestures.RaiseLeftHand,
        KinectGestures.Gestures.Stop,
        KinectGestures.Gestures.Wave,
        //KinectGestures.Gestures.SwipeLeft,
        //KinectGestures.Gestures.SwipeRight,
        //KinectGestures.Gestures.SwipeUp,
        //KinectGestures.Gestures.SwipeDown,
        KinectGestures.Gestures.Jump,
        KinectGestures.Gestures.Push,
        KinectGestures.Gestures.Pull,
    };

    KinectGestures.Gestures action
    {
        get
        {
            return useableGesture[rand.Next(useableGesture.Length)];
        }
    }

    System.Random rand = new System.Random(NextID);

    GameObject text;
    TextMesh tmesh;

    public void init (Vector3 center, float r0, float r1, float r2, IInteractionInput input)
    {
        rand = new System.Random(NextID);
        id = NextID;
        NextID++;

        this.center = center;
        near = r0;
        mid = r1;
        far = r2;
        near2 = near*near;
        mid2 = mid*mid;
        far2 = far*far;

        this.input = input;

        text = new GameObject("text");
        tmesh = text.AddComponent<TextMesh>();
        tmesh.text = "test";
        tmesh.fontSize = 500;
        text.transform.parent = transform;
        text.transform.position = transform.position + new Vector3(0f, 200f, 0f);
        text.SetActive(false);
    }

    public void attr(int gender, int age, int children, double lifeEndTime)
    {
        this.gender = gender;
        this.age = age;
        this.children = children;
        this.lifeEndTime = lifeEndTime;
    }

    public void leave()
    {
        Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        switch (status)
        {
            case RandomWalkPeopleStatus.WALK:
                float dis = dis2();

                // if reach boundary, change new dir
                if (dis > far2)
                {
                    setTarget(getTarget(1));
                }
                else if (dis < near2)
                {
                    setTarget(getTarget(-1));
                }

                transform.position = transform.position + speed * Time.deltaTime * towards;

                // if reach target, stop for a while
                if (reachTarget(start, target))
                {
                    stopTime = 0;
                    status = RandomWalkPeopleStatus.STOP;

                    stopThreshold = 4f + (float)rand.NextDouble() * 10f;

                    transform.LookAt(center);
                    Vector3 lookAtAngle = transform.eulerAngles;
                    transform.eulerAngles = new Vector3(0, lookAtAngle.y, lookAtAngle.z);
                    if (dis < mid2)
                    {
                        doAction();
                    }
                }
                break;
            case RandomWalkPeopleStatus.STOP:
                stopTime += Time.deltaTime;
                if (stopThreshold < stopTime)
                {
                    status = RandomWalkPeopleStatus.WALK;
                    text.SetActive(false);
                    setTarget(getTarget(1));
                }
                break;
            default:
                break;
        }

    }

    public bool visible()
    {
        return dis2() < mid2;
    }

    void doAction()
    {
        text.transform.LookAt(text.transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
        text.SetActive(true);

        KinectGestures.Gestures a = action;

        switch (a)
        {
            case KinectGestures.Gestures.None:
                break;
            case KinectGestures.Gestures.RaiseRightHand:
                break;
            case KinectGestures.Gestures.RaiseLeftHand:
                break;
            case KinectGestures.Gestures.Stop:
                break;
            case KinectGestures.Gestures.Wave:
                break;
            case KinectGestures.Gestures.SwipeLeft:
                break;
            case KinectGestures.Gestures.SwipeRight:
                break;
            case KinectGestures.Gestures.SwipeUp:
                break;
            case KinectGestures.Gestures.SwipeDown:
                break;
            case KinectGestures.Gestures.Jump:
                break;
            case KinectGestures.Gestures.Push:
                break;
            case KinectGestures.Gestures.Pull:
                break;
            default:
                break;
        }

        input.OnInteractionInput(
            new WorldEvent { position = transform.position, gesture = a });

        tmesh.text = "" + a;
    }

    private bool reachTarget(Vector3 start, Vector3 target)
    {
        float xk = transform.position.x;
        float zk = transform.position.z;
        float xmax = Mathf.Max(start.x, target.x);
        float zmax = Mathf.Max(start.z, target.z);
        float xmin = Mathf.Min(start.x, target.x);
        float zmin = Mathf.Min(start.z, target.z);
        return !( xmin <= xk && xk <= xmax && zmin <= zk && zk <= zmax );
    }

    public Vector3 getTarget(int status)
    {
        if (status == -1)
        {
            Vector3 p = transform.position - center;
            Vector3 res;
            do
            {
                float radius = (float)(near + (far - near) * rand.NextDouble());
                float angle = (float)(rand.NextDouble() * 2 * Mathf.PI);
                float x = radius * Mathf.Cos(angle);
                float z = radius * Mathf.Sin(angle);
                res = new Vector3(x, 0, z);
            } while ( p.x * res.x + p.z * res.z - near2 < 0);
            return res + center;
        }
        else
        {
            float radius = (float)(near + (far - near) * rand.NextDouble());
            float angle = (float)(rand.NextDouble() * 2 * Mathf.PI);
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            return new Vector3(x, 0, z) + center;
        }
    }

    public void setTarget(Vector3 target)
    {
        this.target = target;
        start = transform.position;
        speed = (float)rand.NextDouble() * 50f + 100f;

        towards = Vector3.Normalize(target - transform.position);
        transform.LookAt(target);
        Vector3 lookAtAngle = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, lookAtAngle.y, lookAtAngle.z);
    }

    float dis2()
    {
        Vector3 pos = transform.position - center;
        return pos.x * pos.x + pos.z * pos.z;
    }
    //people[i].transform.LookAt(emulatorBounds.center);
    //Vector3 lookAtAngle = people[i].transform.eulerAngles;
    //people[i].transform.eulerAngles = new Vector3(0, lookAtAngle.y, lookAtAngle.z);
}
