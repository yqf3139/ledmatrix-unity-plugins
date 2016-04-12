using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class CylinderLeqSeq : LedSeq
{
    enum CylinderParam1 : int { Floor, Round };
    enum CylinderParam2 : int { Step, Distance, Height, Pillar };

    public Vector3[][] positions = null;

    public float step = 7f;
    public float distance = 7f;
    public float height = 5f;
    public float pillar = 7f;
    public int floorCounter = 80;
    public int roundsCounter = 18;
    public int ledsPerFloor = 0;
    public int[] ledsSumPerRound = null;
    public int[] ledsSumPerSurface = null;

    public float diameter, radius, perimeter;

    public CylinderLeqSeq(uint floor = 0, uint round = 0, float step = 0, float distance = 0, float height = 0, float pillar = 0)
    {
        param1[(int)CylinderParam1.Floor] = floor;
        param1[(int)CylinderParam1.Round] = round;

        param2[(int)CylinderParam2.Step] = step;
        param2[(int)CylinderParam2.Distance] = distance;
        param2[(int)CylinderParam2.Height] = height;
        param2[(int)CylinderParam2.Pillar] = pillar;

        floorCounter = (int)floor;
        roundsCounter = (int)round;
        this.step = step;
        this.distance = distance;
        this.height = height;
        this.pillar = pillar;

        init();
    }

    void init()
    {
        float PI = 3.14159f;
        positions = new Vector3[roundsCounter][];
        ledsSumPerRound = new int[roundsCounter];
        ledsSumPerSurface = new int[roundsCounter];
        for (int i = 0; i < roundsCounter; i++)
        {
            ledsSumPerRound[i] = ledsPerFloor;

            diameter = ((i + 1) * step * 2);
            radius = diameter / 2;
            perimeter = diameter * PI;
            int counter = (int)(perimeter / 2f / distance);
            if (i != 0) counter++;
            counter <<= 1;
            //Debug.Log(counter);
            if (i != 0)
            {
                ledsSumPerSurface[i] = ledsSumPerSurface[i - 1] + positions[i - 1].Length * floorCounter;
            }

            float angle = 2 * PI / counter;

            positions[i] = new Vector3[counter];
            for (int j = 0; j < counter; j++)
            {
                positions[i][j] = new Vector3(radius * Mathf.Cos(angle * j), 0f, radius * Mathf.Sin(angle * j));
            }
            ledsPerFloor += counter;
        }
        ledsPerFrame = ledsPerFloor * floorCounter;
        bytesPerFrame = ledsPerFrame * sizeof(uint);

        leddata = new uint[ledsPerFrame];
    }

    public override void open(string path, uint frames)
    {
        open(path, frames, (uint)Shape.Cylinder);
    }

    public override void restoreParams()
    {
        floorCounter = (int)param1[(int)CylinderParam1.Floor];
        roundsCounter = (int)param1[(int)CylinderParam1.Round];
        step = param2[(int)CylinderParam2.Step];
        distance = param2[(int)CylinderParam2.Distance];
        height = param2[(int)CylinderParam2.Height];
        pillar = param2[(int)CylinderParam2.Pillar];

        frameCounter = (int)header[(int)HeaderItem.FrameTotolCount];
        fps = (int)header[(int)HeaderItem.Fps];
        init();
    }

    public override int getLedIdx(int i, int j, int k)
    {
        //int idx = ledsSumPerSurface[j] + positions[j].Length * (floorCounter - 1 - i) + k;
        int idx = ledsSumPerSurface[j] + k * floorCounter + (floorCounter - 1 - i);
        return idx;
        //return i * ledsPerFloor + ledsSumPerRound[j] + k;
    }

}
