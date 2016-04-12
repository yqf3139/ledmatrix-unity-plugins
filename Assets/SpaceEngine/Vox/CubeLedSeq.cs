using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public class CubeLedSeq : LedSeq
{
    protected enum CubeParam1 : int { X, Y, Z };

    public int LEDX, LEDY, LEDZ;

    public CubeLedSeq(uint x = 0, uint y = 0, uint z = 0)
    {
        param1[(int)CubeParam1.X] = x;
        param1[(int)CubeParam1.Y] = y;
        param1[(int)CubeParam1.Z] = z;

        LEDX = (int)x;
        LEDY = (int)y;
        LEDZ = (int)z;

        init();
    }

    public void init()
    {
        ledsPerFrame = LEDX * LEDY *LEDZ;
        bytesPerFrame = ledsPerFrame * sizeof(uint);
        leddata = new uint[ledsPerFrame];
    }

    public override void open(string path, uint frames)
    {
        open(path, frames, (uint)Shape.Cube);
    }

    public override void restoreParams()
    {
        LEDX = (int)param1[(int)CubeParam1.X];
        LEDY = (int)param1[(int)CubeParam1.Y];
        LEDZ = (int)param1[(int)CubeParam1.Z];
        frameCounter = (int)header[(int)HeaderItem.FrameTotolCount];
        fps = (int)header[(int)HeaderItem.Fps];

        init();
    }

    public override int getLedIdx(int i, int j, int k)
    {
        return i * LEDY * LEDZ + j * LEDZ + k;
    }

}