using UnityEngine;
using System.Collections;
using System;

public class KinectWalkPeople : WalkPeople, KinectGestures.GestureListenerInterface
{
    KinectManager manager;

    public Vector3 kinectPos;
    Int64 userID;
    bool tracked = false;

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
    {
        return true;
    }

    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (id == userIndex)
        {
            text.transform.LookAt(text.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
            text.SetActive(true);
            tmesh.text = "" + gesture;

            input.OnInteractionInput(new WorldEvent
            {
                gesture = gesture,
                position = transform.position
            });
        }
        return true;
    }

    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {

    }

    public void UserDetected(long userId, int userIndex)
    {
        Debug.Log(""+ userId+" "+userIndex);
        if (id == userIndex)
        {
            // as an example - detect these user specific gestures
            KinectManager manager = KinectManager.Instance;
            manager.DetectGesture(userId, KinectGestures.Gestures.Jump);
            manager.DetectGesture(userId, KinectGestures.Gestures.Wave);
            manager.DetectGesture(userId, KinectGestures.Gestures.Pull);
            manager.DetectGesture(userId, KinectGestures.Gestures.Push);
            manager.DetectGesture(userId, KinectGestures.Gestures.Stop);
            tracked = true;
            userID = userId;
        }
    }

    public void UserLost(long userId, int userIndex)
    {
        if (id == userIndex)
        {
            tracked = false;
        }
}

    // Use this for initialization
    void Start () {
        manager = KinectManager.Instance;
        manager.gestureListeners.Add(this);
    }

    // Update is called once per frame
    void Update () {
        // get 1st player
        if (tracked)
        {
            Vector3 posPointMan = manager.GetUserPosition(userID);
            if (!Vector3.zero.Equals(posPointMan))
            {
                posPointMan.x = -posPointMan.x;
                transform.position = kinectPos + posPointMan * 200f + new Vector3(0,-80,0);

                transform.LookAt(center);
                Vector3 lookAtAngle = transform.eulerAngles;
                transform.eulerAngles = new Vector3(0, lookAtAngle.y, lookAtAngle.z);
            }
        }
    }
}
