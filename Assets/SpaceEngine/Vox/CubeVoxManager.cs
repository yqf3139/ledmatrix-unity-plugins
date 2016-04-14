using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public enum CubeDisplayMode { NORMAL, LED }

public class CubeVoxManager : DefaultVoxManager
{
    public uint LEDX = 32;
    public uint LEDY = 32;
    public uint LEDZ = 32;
    public float step = 5f;
    public CubeDisplayMode mode = CubeDisplayMode.LED;

    protected override void initBounds()
    {

    }

    protected override LedMatrix initBridge(LedSeq s)
    {
        throw new NotImplementedException();
    }

    protected override void initConfig()
    {
        CubeLedMatrix.step = step;
        CubeMatrix.step = step;
    }

    protected override LedMatrix initEmulator(LedSeq s)
    {
        switch (mode)
        {
            case CubeDisplayMode.NORMAL:
                return new CubeMatrix(s as CubeLedSeq);
            case CubeDisplayMode.LED:
                return new CubeLedMatrix(s as CubeLedSeq);
            default:
                return new CubeLedMatrix(s as CubeLedSeq);
        }

    }

    protected override LedSeq initLedSeq()
    {
        return new CubeLedSeq(LEDX, LEDY, LEDZ);
    }

    protected override MeshVox initMeshVox(LedSeq s, Bounds b, HashSet<IMeshObject> ss)
    {
        return new CubeMeshVox(s as CubeLedSeq, b, ss);
    }

    protected override ParticleVox initParticleVox(LedSeq s, Bounds b, HashSet<IParticleObject> ss)
    {
        return new CubeParticleVox(s as CubeLedSeq, b, ss);
    }

    protected override void initWorkers(HashSet<Thread> s)
    {

    }
}