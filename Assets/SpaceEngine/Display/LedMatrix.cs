﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class LedMatrix
{
    protected LedSeq ledseq;
    protected GameObject[] ledcubes = null;
    protected Material[] ledmats = null;
    protected GameObject parent = null;
    protected int TOTAL = 0;

    public LedMatrix(LedSeq led)
    {
        ledseq = led;
        TOTAL = led.ledsPerFrame;
    }

    public abstract Bounds getLedBound();

    public virtual void clearLed()
    {
        for (int i = 0; i < TOTAL; i++)
            ledcubes[i].SetActive(false);
    }

    public virtual void showLed()
    {
        uint color;
        uint r, g, b;
        for (int i = 0; i < TOTAL; i++)
        {
            color = ledseq.leddata[i];
            if (color == 0)
            {
                ledcubes[i].SetActive(false);
                continue;
            }
            ledcubes[i].SetActive(true);
            color >>= 8;
            b = 0xff & color;
            color >>= 8;
            g = 0xff & color;
            color >>= 8;
            r = 0xff & color;
            ledmats[i].SetVector("_MKGlowColor", new Vector4(r / 255f, g / 255f, b / 255f, 1f));
        }
    }

    public virtual void dispose()
    {
        UnityEngine.Object.Destroy(parent);
    }

}

