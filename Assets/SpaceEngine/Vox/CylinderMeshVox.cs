using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

public class CylinderMeshVox : MeshVox
{
    [DllImport("testdll")]
    protected static unsafe extern void constructCylinder(
        int _floorCounter, int _roundsCounter, float _step, float _distance, float _height, float _pillar);
    
    CylinderLeqSeq cylinderledseq;
    int floor, round;
    int[] counters;

    public CylinderMeshVox(CylinderLeqSeq seq, Bounds b, HashSet<IMeshObject> objs)
        :base(seq, b, objs)
    {
        cylinderledseq = seq;
        floor = seq.floorCounter;
        round = seq.roundsCounter;
        counters = new int[seq.positions.Length];
        for (int i = 0; i < counters.Length; i++)
        {
            counters[i] = seq.positions[i].Length;
        }
    }

    public override void UpdateLED()
    {
        uint color;
        for (int i = 0; i < floor; i++)
            for (int j = 0; j < round; j++)
                for (int k = 0; k < counters[j]; k++)
                {
                    unsafe
                    {
                        color = (uint)resbuf[(i+1) * 40000 + (j) * 200 + k];
                        if (color == 0)
                        {
                            // j, i, LEDZ-1-k
                            continue;
                        }
                        ledseq.setRealLed(i, j, k, color);
                    }

                }

    }

    public override void VoxStart()
    {
        constructCylinder(
            floor + 2, round + 1, cylinderledseq.step, cylinderledseq.distance,
            cylinderledseq.height, cylinderledseq.pillar);
    }

    public override Vector3 GetTranslationFromZeroCenter()
    {
        return new Vector3();
    }
}

