using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParticleSystemObject : MonoBehaviour, IParticleObject
{
    public Bounds bounds;
    public ParticleSystem ps;
    public Vector2 heightRange;
    public bool needTranslation;

    IParticleObject impl;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        impl = new ParticleSystemObjectImpl(bounds, ps, heightRange, needTranslation);
    }

    public void ParticleObjectPlay(float height)
    {
        Debug.Log("ParticleObjectPlay");
        impl.ParticleObjectPlay(height);
    }

    public void ParticleObjectUpdate(UpdateLedHander handler)
    {
        impl.ParticleObjectUpdate(handler);
    }
}