﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class CylinderVoxManager : DefaultVoxManager
{
    public uint floorCounter = 25;
    public uint roundsCounter = 8;
    public float step = 15f;
    public float distance = 15f;
    public float height = 6f;
    public float pillar = 6f;

    protected override void initBounds()
    {

    }

    protected override LedMatrix initBridge(LedSeq s)
    {
        return new CylinderBigLedBridge(s as CylinderLeqSeq);
    }

    protected override void initConfig()
    {

    }

    protected override LedMatrix initEmulator(LedSeq s)
    {
        return new CylinderLedMatrix(s as CylinderLeqSeq);
    }

    protected override LedSeq initLedSeq()
    {
        return new CylinderLeqSeq(floorCounter, roundsCounter, step, distance, height, pillar);
    }

    protected override MeshVox initMeshVox(LedSeq s, Bounds b, HashSet<IMeshObject> ss)
    {
        return new CylinderMeshVox(s as CylinderLeqSeq, b, ss);
    }

    protected override ParticleVox initParticleVox(LedSeq s, Bounds b, HashSet<IParticleObject> ss)
    {
        return new CylinderParticleVox(s as CylinderLeqSeq, b, ss);
    }

    protected override void initWorkers(HashSet<Thread> s)
    {

    }
}