using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RandomInteractionManager : LEDInteractionManager
{
    static System.Random rand = new System.Random(DateTime.Now.Millisecond);

    public float updatePeopleInterval = 1f;
    public int peopleCountOnStartup = 10;
    public float peopleStayTimeBase = 10;
    public float peopleStayTimeRange = 10;
    public int peopleCountMax = 50;
    public int peopleCountMin = 5;
    public int peopleEmitCount = 1;

    float updatePeopleTime = 0;

    // Use this for initialization
    protected override void start()
    {
        base.start();

        for (int i = 0; i < peopleCountOnStartup; i++)
            emitPeople();
    }

    public override void Update()
    {
        base.Update();

        updatePeopleTime += Time.deltaTime;

        if (updatePeopleTime > updatePeopleInterval)
        {
            updatePeopleTime = 0;
            float now = Time.realtimeSinceStartup;

            people.RemoveAll(p => {
                RandomWalkPeople pp = p as RandomWalkPeople;
                if (pp.lifeEndTime < now)
                    pp.leave();
                return pp.lifeEndTime < now;
            });

            for (int i = 0; i < peopleEmitCount; i++)
            {
                emitPeople();
            }

            while (people.Count > peopleCountMax)
            {
                RandomWalkPeople p = people.ElementAt(0) as RandomWalkPeople;
                people.RemoveAt(0);
                p.leave();
            }

            while (people.Count < peopleCountMin)
            {
                emitPeople();
            }
        }

    }

    void emitPeople()
    {
        int gender = rand.Next(2);
        int age = rand.Next(3, 30);
        int idx = gender * 2 + (age < 15 ? 1 : 0);
        GameObject go = Instantiate(models[idx], pos, Quaternion.identity) as GameObject;
        go.SetActive(true);
        RandomWalkPeople walker = go.AddComponent<RandomWalkPeople>();

        go.name = "People" + WalkPeople.NextID;

        walker.init(pos, r0 * 100f, r1 * 100f, r2 * 100f, input);
        walker.attr(gender, age, (age < 15 ? 1 : 0), Time.realtimeSinceStartup + peopleStayTimeBase + rand.NextDouble() * peopleStayTimeRange);

        go.transform.position = walker.getTarget(1);
        walker.setTarget(walker.getTarget(1));
        people.Add(walker);
    }

}
