using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeParticleVox : ParticleVox
{
    private CubeLedSeq cubeledseq;
    private int LEDX, LEDY, LEDZ;
    private Vector3 offset = new Vector3(0.5f, 0, 0.5f);

    public CubeParticleVox(CubeLedSeq led, Bounds b, HashSet<IParticleObject> pos)
        :base(led, b, pos)
    {
        cubeledseq = led;
        LEDX = led.LEDX;
        LEDY = led.LEDY;
        LEDZ = led.LEDZ;
    }

    public override void setRealLed(Vector3 position, Color color, float size)
    {
        position = (position + offset);

        float currentSize = 1f;
        if (size < 1f)
        {
            currentSize = size;
            if (size < .25f)
            {
                return;
            }
        }
        Vector3 scale = new Vector3(currentSize, currentSize, currentSize);

        int x = (int)((position.x) * LEDX);
        int y = (int)((position.y) * LEDY);
        int z = (int)((position.z) * LEDZ);

        if (x < 0 || y < 0 || z < 0)
        {
            return;
        }

        if (x >= LEDX || y >= LEDY || z >= LEDZ)
        {
            //Debug.Log(x + " " + y + " " + z);
            return;
        }

        ledseq.setRealLed(x, y, z, color2uint(color, currentSize));
    }
}
