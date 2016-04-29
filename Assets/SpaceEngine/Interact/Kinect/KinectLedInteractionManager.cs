using UnityEngine;
using System.Collections;
using System;

public class KinectLedInteractionManager : LEDInteractionManager {

    public int WalkerNum = 1;

    protected override void start()
    {
        base.start();

        KinectWalkPeople walker = null;
        for (int i = 0; i < WalkerNum; i++)
        {
            GameObject go = Instantiate(models[0], pos, Quaternion.identity) as GameObject;
            go.SetActive(true);
            walker = go.AddComponent<KinectWalkPeople>();

            go.name = "People" + WalkPeople.NextID;

            walker.init(pos, r0 * 100f, r1 * 100f, r2 * 100f, input);
            walker.attr(0, 20, 0);
            walker.kinectPos = pos + new Vector3(0, 0, r0 * 100f);
            walker.transform.position = pos + new Vector3(600, 0, 0);
            //walker.transform.LookAt(pos);
            //Vector3 lookAtAngle = transform.eulerAngles;
            //walker.transform.eulerAngles = new Vector3(0, lookAtAngle.y, lookAtAngle.z);
            people.Add(walker);
        }
        if (WalkerNum == 1)
        {
            MouseOrbitImproved cam = Camera.main.GetComponent<MouseOrbitImproved>();
            cam.autoMove = cam.manualMove = false;
            cam.followTarget = walker.transform;
        }
    }

    // Use this for initialization
    void Start () {

    }

    public override void Update()
    {
        base.Update();
    }
}
