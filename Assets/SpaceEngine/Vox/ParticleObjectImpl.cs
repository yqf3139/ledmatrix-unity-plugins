using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public abstract class ParticleObjectImpl : IParticleObject
{
    public Bounds bounds;
    protected Vector3 origin;

    protected ParticleObjectImpl(Bounds b)
    {
        bounds = b;
        origin = b.center - new Vector3(0, b.extents.y, 0);
    }

    public abstract void ParticleObjectPlay(float height);
    public abstract void ParticleObjectUpdate(UpdateLedHander handler);

}

public class ParticleEmitObjectImpl : ParticleObjectImpl
{
    ParticleEmitter emitter;
    public Vector2 heightRange;

    public ParticleEmitObjectImpl(Bounds b, ParticleEmitter e, Vector2 heightRange)
        : base(b)
    {
        emitter = e;
        this.heightRange = heightRange;
    }

    public virtual void setHeight(float height)
    {
        emitter.localVelocity = new Vector3(0, heightRange.x + (heightRange.y - heightRange.x) * height, 0);
    }

    public override void ParticleObjectPlay(float height)
    {
        if (emitter.emit)
        {
            return;
        }
        setHeight(height);
        emitter.emit = true;
        new Thread(() =>
        {
            Thread.Sleep(5 * 1000);
            Loom.QueueOnMainThread(() =>
            {
                emitter.emit = false;
            });
        }).Start();
    }
    public void setParticle(Particle p)
    {

    }
    public override void ParticleObjectUpdate(UpdateLedHander handler)
    {
        int len = emitter.particleCount;
        Particle[] parr = emitter.particles;
        for (int i = 0; i < len; i++)
        {
            Particle particle = parr[i];
            Vector3 p = particle.position - origin;

            handler.setRealLed(
                new Vector3(p.x / bounds.size.x, p.y / bounds.size.y, p.z / bounds.size.z),
                particle.color, particle.size);
        }
    }

}

public class ParticleSystemObjectImpl : ParticleObjectImpl
{
    ParticleSystem.Particle[] ps_arr = new ParticleSystem.Particle[5000];

    public HashSet<ParticleSystem> pss = new HashSet<ParticleSystem>();
    public ParticleSystem root;
    public bool needTranslate = false;
    public Vector2 heightRange;

    public ParticleSystemObjectImpl(Bounds b, ParticleSystem ps, Vector2 heightRange, bool needTranslate = false)
        : base(b)
    {
        root = ps;
        ExtractParticleSystems(ps);
        this.needTranslate = needTranslate;
        this.heightRange = heightRange;
    }

    public override void ParticleObjectPlay(float height)
    {
        root.gravityModifier = heightRange.x + (1 - height) * (heightRange.y - heightRange.x);
        root.Play();
    }

    public void ExtractParticleSystems(ParticleSystem theps)
    {
        if (theps == null) return;
        pss.Add(theps);
        ExtractParticleSystems(theps.subEmitters.birth0);
        ExtractParticleSystems(theps.subEmitters.birth1);
        ExtractParticleSystems(theps.subEmitters.death0);
        ExtractParticleSystems(theps.subEmitters.death1);
        ExtractParticleSystems(theps.subEmitters.collision0);
        ExtractParticleSystems(theps.subEmitters.collision1);
    }

    public override void ParticleObjectUpdate(UpdateLedHander handler)
    {
        foreach (ParticleSystem ps in pss)
        {
            int len = ps_arr.Length;
            while (len < ps.particleCount)
            {
                len <<= 1;
            }
            if (len != ps_arr.Length)
            {
                ps_arr = new ParticleSystem.Particle[len];
            }
            int arrlen = ps.GetParticles(ps_arr);
            for (int i = 0; i < arrlen; i++)
            {
                Color32 c32 = ps_arr[i].GetCurrentColor(ps);
                Color c = new Color(c32.r / 255f, c32.g / 255f, c32.b / 255f, 1f);
                Vector3 p;
                if (needTranslate)
                    p = ps.transform.TransformPoint(ps_arr[i].position) - origin;
                else
                    p = ps_arr[i].position - origin;

                handler.setRealLed(
                    new Vector3(p.x / bounds.size.x, p.y / bounds.size.y, p.z / bounds.size.z),
                    c,
                    ps_arr[i].GetCurrentSize(ps));
            }
        }
    }
}