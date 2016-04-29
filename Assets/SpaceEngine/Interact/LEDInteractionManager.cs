using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class CrowdInfo
{

}

public class CrowdInfoSummry
{
    public Vector3 crowdDirection;
    public float genderRatio;
    public float childrenRatio;
    public float ageAverage;
    public int total;
}

public interface IInteractionInput
{
    void OnInteractionInput(WorldEvent e);
    void OnCrowdInfo(CrowdInfo[] infos);
    void OnCrowdInfoSummry(CrowdInfoSummry summary);
}

public class LEDInteractionManager : MonoBehaviour {

    public float updateInfoInterval = 0.1f;

    protected IInteractionInput input;
    protected Bounds emulatorBounds;
    protected Transform arrow;
    protected float r, r0, r1, r2;
    protected float updateInfoTime = 0;
    protected Vector3 pos;
    protected GameObject[] rings = new GameObject[3];
    protected List<WalkPeople> people = new List<WalkPeople>();
    protected GameObject[] models = new GameObject[4];

    bool init = true;

    // Use this for initialization
    void Start () {
	
	}

    // Use this for initialization
    virtual protected void start()
    {
        input = DefaultVoxManager.getDefault();
        emulatorBounds = DefaultVoxManager.getDefault().ledWorld;
        arrow = GameObject.Find("Arrow").transform;

        GameObject ring = GameObject.Find("RingPro");

        pos = new Vector3(emulatorBounds.center.x, 0, emulatorBounds.center.z);
        for (int i = 0; i < rings.Length; i++)
        {
            rings[i] = GameObject.Instantiate(ring, pos, Quaternion.identity) as GameObject;
            rings[i].name = "InteractionRing" + i;
        }
        ring.SetActive(false);

        r = emulatorBounds.extents.x * 1.414f / 100f;
        r0 = r + 0.5f;
        r1 = r + 4f;
        r2 = r + 6f;

        rings[0].transform.localScale = Vector3.one * r0;
        rings[1].transform.localScale = Vector3.one * r1;
        rings[2].transform.localScale = Vector3.one * r2;


        //float near = r0 * 100f;
        //float far = r2 * 100f;

        //GameObject woman = GameObject.Find("Human_Women");
        models[0] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Woman"), Vector3.zero, Quaternion.identity);
        models[1] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Girl"), Vector3.zero, Quaternion.identity);
        models[2] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Man"), Vector3.zero, Quaternion.identity);
        models[3] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Boy"), Vector3.zero, Quaternion.identity);

        foreach (GameObject m in models)
            m.SetActive(false);
    }

    // Update is called once per frame
    public virtual void Update () {
        if (init)
        {
            init = false;
            start();
        }

        updateInfoTime += Time.deltaTime;
        if (updateInfoTime > updateInfoInterval)
        {
            updateInfoTime = 0;

            int total = 0;
            int male = 0;
            int age = 0;
            int children = 0;

            Vector3 dir = new Vector3();
            foreach (WalkPeople p in people)
            {
                if (!p.visible())
                {
                    continue;
                }
                total++;
                dir += Vector3.Normalize(p.transform.position - pos);
                male += p.gender;
                age += p.age;
                children += p.children;
            }

            if (total == 0) dir = Vector3.right;
            else dir = Vector3.Normalize(dir);

            input.OnCrowdInfo(null);
            input.OnCrowdInfoSummry(new CrowdInfoSummry()
            {
                total = total,
                ageAverage = (float)age / total,
                genderRatio = (float)male / total,
                childrenRatio = (float)children / total,
                crowdDirection = dir,
            });

            arrow.position = pos + dir * r0 * 100f;
        }
    }
}
