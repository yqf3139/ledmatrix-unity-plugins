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
    public Vector3 center;
    public float extend;

    IParticleObject impl;

    void Start()
    {
        bounds = new Bounds(center, 2 * extend * DefaultVoxManager.getDefault().getRatio());
        Debug.Log(DefaultVoxManager.getDefault());
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