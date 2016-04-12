using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeMeshVox : MeshVox
{
    CubeLedSeq cubeledseq = null;
    int LEDX, LEDY, LEDZ;

    public CubeMeshVox(CubeLedSeq seq, Bounds b, HashSet<IMeshObject> objs, HashSet<IVoxListener> liss)
        :base(seq, b, objs, liss)
    {
        cubeledseq = seq;
        LEDX = cubeledseq.LEDX;
        LEDY = cubeledseq.LEDY;
        LEDZ = cubeledseq.LEDZ;
    }

    public override void VoxStart()
    {
        // rip out the surface leds
        construct(LEDX + 2, LEDY + 2, LEDZ + 2);
    }

    public override void UpdateLED()
    {
        // draw the vol obj
        uint color;
        for (int i = 0; i < LEDX; i++)
            for (int j = 0; j < LEDY; j++)
                for (int k = 0; k < LEDZ; k++)
                {
                    unsafe
                    {
                        color = (uint)resbuf[(i + 1) * 40000 + (j + 1) * 200 + k + 1];
                        if (color == 0)
                        {
                            // j, i, LEDZ-1-k
                            continue;
                        }
                        ledseq.setRealLed(i, j, k, color);
                    }
                }
    }
}

