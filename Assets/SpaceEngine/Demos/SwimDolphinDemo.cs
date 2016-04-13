using UnityEngine;
using System.Collections;
using System;
using UnityEngine;

public class SwimDolphinDemo : MonoBehaviour, IMeshEventListener {

    static System.Random rand = new System.Random();

    Bounds objectsWorld;
    Vector3 fishvelocity;

    public void MeshObjectOnEvent(WorldEvent e)
    {
        fishvelocity = 5 * WalkVelocity(e.position, transform.position);
        Quaternion q = transform.rotation;
        Vector3 v = e.position - objectsWorld.center;
        v.y = 0;
        v = Vector3.Normalize(v);
        float angle = v.z > 0 ? Mathf.Acos(v.x) : 2 * Mathf.PI - Mathf.Acos(v.x);
        q.eulerAngles = new Vector3(90, 360f - 360f * angle / (2 * Mathf.PI), 0);
        transform.rotation = q;
    }

    public void MeshObjectUpdate(MeshObjectUpdateStatus status)
    {
        if (status != MeshObjectUpdateStatus.IN)
        {
            fishvelocity = 5 * RandomWalkVelocity(objectsWorld, transform.position);
            Quaternion q = transform.rotation;
            Vector3 v = fishvelocity;
            v.y = 0;
            v = Vector3.Normalize(v);
            float angle = v.z > 0 ? Mathf.Acos(v.x) : 2 * Mathf.PI - Mathf.Acos(v.x);
            //q.y = 270f - 360f * angle;
            q.eulerAngles = new Vector3(90, 360f - 360f * angle / (2 * Mathf.PI), 0);
            transform.rotation = q;
            Debug.Log("new dir");
        }
    }

    // Use this for initialization
    void Start () {
        objectsWorld = DefaultVoxManager.getDefault().objectsWorld;
        fishvelocity = 5 * RandomWalkVelocity(objectsWorld, transform.position);
        MeshNormalObject self = GetComponent<MeshNormalObject>();
        self.listener = this;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += fishvelocity * Time.deltaTime;
    }

    public static Vector3 WalkVelocity(Vector3 des, Vector3 p)
    {
        return Vector3.Normalize(des - p);
    }

    public static Vector3 RandomWalkVelocity(Bounds b, Vector3 p)
    {
        Vector3 tmp = b.max - b.min;
        Vector3 des = b.min + new Vector3(tmp.x * (float)rand.NextDouble(), tmp.y * (float)rand.NextDouble(), tmp.z * (float)rand.NextDouble());
        return Vector3.Normalize(des - p);
    }
}
