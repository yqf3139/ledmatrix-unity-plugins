using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Vox : IInteractionInput
{
    protected LedSeq ledseq;
    protected Bounds worldBounds;

    public Vox(LedSeq s, Bounds b)
    {
        ledseq = s;
        worldBounds = b;
    }

    public abstract void OnCrowdInfo(CrowdInfo[] infos);
    public abstract void OnCrowdInfoSummry(CrowdInfoSummry summary);
    public abstract void OnInteractionInput(WorldEvent e);
    public abstract void Update();
}
