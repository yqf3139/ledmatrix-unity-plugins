using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ParticleVox : Vox, UpdateLedHander
{
    HashSet<IParticleObject> pos;

    public ParticleVox(LedSeq led, Bounds b, HashSet<IParticleObject> pos)
        :base(led, b)
    {
        this.pos = pos;
    }

    public uint color2uint(Color color, float size)
    {
        uint d = 0;
        d |= ((uint)(color.r * 255 * size)) << 24;
        d |= ((uint)(color.g * 255 * size)) << 16;
        d |= ((uint)(color.b * 255 * size)) << 8;
        return d;
    }

    public override void OnInteractionInput(WorldEvent e)
    {

    }

    public override void OnCrowdInfo(CrowdInfo[] infos)
    {

    }

    public override void OnCrowdInfoSummry(CrowdInfoSummry summary)
    {

    }

    public override void Update()
    {
        if (pos != null)
            foreach (IParticleObject po in pos)
            {
                po.ParticleObjectUpdate(this);
            }
    }

    public abstract void setRealLed(Vector3 position, Color color, float size);
}

