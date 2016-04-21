using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ParticleSystemEmitMode { NORMAL, GRAVITY, FIREWORK, CONTINUOUS}

public class ParticleSystemObject : MonoBehaviour, IParticleObject
{
    public Bounds bounds;
    public ParticleSystem ps;
    public Vector2 heightRange;
    public bool needTranslation;
    public Vector3 center;
    public float extend;

    public ParticleSystemEmitMode mode = ParticleSystemEmitMode.NORMAL;

    IParticleObject impl;

    void Start()
    {
        bounds = new Bounds(center, 2 * extend * DefaultVoxManager.getDefault().getRatio());
        ps = GetComponent<ParticleSystem>();

        switch (mode)
        {
            case ParticleSystemEmitMode.GRAVITY:
                impl = new ParticleSystemObjectGravityImpl(bounds, ps, heightRange, needTranslation);
                break;
            case ParticleSystemEmitMode.NORMAL:
                impl = new ParticleSystemObjectImpl(bounds, ps, heightRange, needTranslation);
                break;
            case ParticleSystemEmitMode.FIREWORK:
                impl = new ParticleSystemObjectFireworksImpl(bounds, ps, heightRange, needTranslation);
                break;
            case ParticleSystemEmitMode.CONTINUOUS:
                impl = new ParticleSystemObjectContinuousImpl(bounds, ps, heightRange, needTranslation);
                break;
            default:
                throw new NotImplementedException();
        }

    }
    public void ParticleObjectUpdate(UpdateLedHander handler)
    {
        impl.ParticleObjectUpdate(handler);
    }
	
    public void ParticleObjectPlay(float height, float time, Vector2 center, Vector2 area)
    {
        impl.ParticleObjectPlay(height, time, center, area);
    }
}