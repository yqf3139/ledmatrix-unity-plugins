using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParticleEmitObject : MonoBehaviour, IParticleObject
{
    public Bounds bounds;
    public ParticleEmitter pe;
    public Vector2 heightRange;
    public bool needTranslation;

    IParticleObject impl;

    void Start()
    {
        pe = GetComponent<EllipsoidParticleEmitter>();
        impl = new ParticleEmitObjectImpl(bounds, pe, heightRange);
    }

    public void ParticleObjectPlay(float height)
    {
        impl.ParticleObjectPlay(height);
    }

    public void ParticleObjectUpdate(UpdateLedHander handler)
    {
        impl.ParticleObjectUpdate(handler);
    }
}

