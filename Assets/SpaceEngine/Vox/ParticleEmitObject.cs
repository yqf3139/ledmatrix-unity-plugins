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
    public Vector3 center;
    public float extend;

    IParticleObject impl;

    void Start()
    {
        bounds = new Bounds(center, 2 * extend * DefaultVoxManager.getDefault().getRatio());
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

