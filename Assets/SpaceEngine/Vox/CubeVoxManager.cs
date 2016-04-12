using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class CubeVoxManager : DefaultVoxManager
{
    public uint LEDX = 32;
    public uint LEDY = 32;
    public uint LEDZ = 32;

    protected override void initBounds()
    {

    }

    protected override LedMatrix initBridge(LedSeq s)
    {
        throw new NotImplementedException();
    }

    protected override void initConfig()
    {

    }

    protected override LedMatrix initEmulator(LedSeq s)
    {
        return new CubeLedMatrix(s as CubeLedSeq);
    }

    protected override LedSeq initLedSeq()
    {
        return new CubeLedSeq(LEDX, LEDY, LEDZ);
    }

    protected override MeshVox initMeshVox(LedSeq s, Bounds b, HashSet<IMeshObject> ss, HashSet<IVoxListener> liss)
    {
        return new CubeMeshVox(s as CubeLedSeq, b, ss, liss);
    }

    protected override ParticleVox initParticleVox(LedSeq s, Bounds b, HashSet<IParticleObject> ss)
    {
        return new CubeParticleVox(s as CubeLedSeq, b, ss);
    }

    protected override void initWorkers(HashSet<Thread> s)
    {

    }
}