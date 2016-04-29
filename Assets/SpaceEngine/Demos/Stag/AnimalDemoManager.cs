using UnityEngine;
using System.Collections;
using System;

public interface AnimalEatDoneCallback
{
    void EatDoneCallback();
}

public class AnimalDemoManager : MonoBehaviour, IMeshEventListener, AnimalEatDoneCallback
{

    GameObject food;

    int ptr = 0;

    public void MeshObjectUpdate(MeshObjectUpdateStatus s)
    {
        
    }

    public void OnCrowdInfo(CrowdInfo[] infos)
    {

    }

    public void OnCrowdInfoSummry(CrowdInfoSummry summary)
    {

    }

    public void OnInteractionInput(WorldEvent e)
    {
        switch (e.gesture)
        {
            case KinectGestures.Gestures.SwipeLeft:
                OnSwitchAnimal(true);
                break;
            case KinectGestures.Gestures.SwipeRight:
                OnSwitchAnimal(false);
                break;
            case KinectGestures.Gestures.RaiseRightHand:
                if (AnimalDemoController.controllers[ptr].isActionDone())
                    AnimalDemoController.controllers[ptr].FeedAttention(Vector3.Normalize(e.position));
                break;
            case KinectGestures.Gestures.SwipeDown:
                if (AnimalDemoController.controllers[ptr].isActionDone())
                {
                    AnimalDemoController.controllers[ptr].Eat(food, e.position);
                }
                break;
            default:
                break;
        }
    }

    public void EatDoneCallback()
    {
        
    }

    public void OnSwitchAnimal(bool toLeft)
    {
        Debug.Log("to left" + toLeft);

        int ori = ptr;
        ptr = toLeft ? ptr - 1 : ptr + 1;

        if (ptr == -1 || ptr == AnimalDemoController.controllers.Length)
        {
            ptr = ori;
            return;
        }

        Debug.Log("from " + ori + " to " + ptr);

        AnimalDemoController.controllers[ori].gameObject.SetActive(false);
        AnimalDemoController.controllers[ori].self.gameObject.SetActive(false);

        AnimalDemoController.controllers[ptr].gameObject.SetActive(true);
        AnimalDemoController.controllers[ptr].self.gameObject.SetActive(true);
        AnimalDemoController.controllers[ptr].PlayDefault();
    }

    // Use this for initialization
    void Start () {
        food = GameObject.Find("Food");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
