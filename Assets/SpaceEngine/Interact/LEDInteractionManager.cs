using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class LEDInteractionManager {

    static System.Random rand = new System.Random(DateTime.Now.Millisecond);

    public float updateInfoTime = 0;
    public float updatePeopleTime = 0;
    public float updateInfoInterval = 0.1f;
    public float updatePeopleInterval = 1f;

    public int peopleCountOnStartup = 10;
    public float peopleStayTimeBase = 10;
    public float peopleStayTimeRange = 10;
    public int peopleCountMax = 50;
    public int peopleCountMin = 5;
    public int peopleEmitCount = 1;

    LedMatrix emulator;
    IInteractionInput input;
    Bounds emulatorBounds;
    GameObject[] rings = new GameObject[3];
    GameObject[] models = new GameObject[4];

    List<RandomWalkPeople> people = new List<RandomWalkPeople>();
    Vector3 pos;

    float r, r0, r1, r2;

    Transform arrow;

    public LEDInteractionManager(LedMatrix emulator, IInteractionInput input)
    {
        this.emulator = emulator;
        this.input = input;
        emulatorBounds = emulator.getLedBound();

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

        float near = r0 * 100f;
        float far =  r2 * 100f;

        //GameObject woman = GameObject.Find("Human_Women");
        models[0] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Woman"), Vector3.zero, Quaternion.identity);
        models[1] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Girl"), Vector3.zero, Quaternion.identity);
        models[2] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Man"), Vector3.zero, Quaternion.identity);
        models[3] = (GameObject)GameObject.Instantiate(Resources.Load("Human_Boy"), Vector3.zero, Quaternion.identity);

        foreach (GameObject m in models)
            m.SetActive(false);

        for (int i = 0; i < peopleCountOnStartup; i++)
            emitPeople();

    }

    void emitPeople()
    {
        int gender = rand.Next(2);
        int age = rand.Next(3, 30);
        int idx = gender * 2 + (age < 15 ? 1 : 0);
        GameObject go = GameObject.Instantiate(models[idx], pos, Quaternion.identity) as GameObject;
        go.SetActive(true);

        go.name = "People" + RandomWalkPeople.NextID;

        RandomWalkPeople walker  = go.GetComponent<RandomWalkPeople>();

        walker.init(pos, r0 * 100f, r1 * 100f, r2 * 100f, input);
        walker.attr(gender, age, (age < 15 ? 1 : 0), Time.realtimeSinceStartup + peopleStayTimeBase + rand.NextDouble() * peopleStayTimeRange);

        go.transform.position = walker.getTarget(1);
        walker.setTarget(walker.getTarget(1));
        people.Add(walker);
    }

    public void UpdateInfo()
    {
        updateInfoTime += Time.deltaTime;
        updatePeopleTime += Time.deltaTime;

        if (updatePeopleTime > updatePeopleInterval)
        {
            updatePeopleTime = 0;
            float now = Time.realtimeSinceStartup;

            people.RemoveAll(p => {
                if (p.lifeEndTime < now)
                    p.leave();
                return p.lifeEndTime < now;
            });

            for (int i = 0; i < peopleEmitCount; i++)
            {
                emitPeople();
            }

            while (people.Count > peopleCountMax)
            {
                RandomWalkPeople p = people.ElementAt(0);
                people.RemoveAt(0);
                p.leave();
            }

            while (people.Count < peopleCountMin)
            {
                emitPeople();
            }
        }

        if (updateInfoTime > updateInfoInterval)
        {
            updateInfoTime = 0;

            int total = 0;
            int male = 0;
            int age = 0;
            int children = 0;

            Vector3 dir = new Vector3();
            foreach (RandomWalkPeople p in people)
            {
                if (!p.visible())
                {
                    continue;
                }
                total++;
                dir += Vector3.Normalize( p.transform.position - pos );
                male += p.gender;
                age += p.age;
                children += p.children;
            }

            dir = Vector3.Normalize(dir);

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
