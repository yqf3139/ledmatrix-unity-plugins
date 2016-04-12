using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Vox
{
    protected LedSeq ledseq;
    protected Bounds worldBounds;

    public Vox(LedSeq s, Bounds b)
    {
        ledseq = s;
        worldBounds = b;
    }

    public abstract void Update();
    public abstract void OnEvent(WorldEvent e);
}
