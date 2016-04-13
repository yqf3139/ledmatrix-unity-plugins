using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CylinderParticleVox : ParticleVox
{
    private CylinderLeqSeq cylinderledseq;

    private int floorCounter, roundsCounter;
    private float radius;
    private float step;
    private Vector3[][] positions = null;
    private Vector3 origin;

    public CylinderParticleVox(CylinderLeqSeq led, Bounds b, HashSet<IParticleObject> pos)
        :base(led, b, pos)
    {
        cylinderledseq = led;
        origin = b.center - new Vector3(0, b.extents.y, 0);

        floorCounter = led.floorCounter;
        roundsCounter = led.roundsCounter;
        radius = led.radius;
        step = led.step;
        positions = led.positions;
    }

    // position need to be center around [0, +, 0]
    public override void setRealLed(Vector3 position, Color color, float size)
    {
        float currentSize = 1f;
        if (size < 1f)
        {
            currentSize = size;
            if (size < .25f)
            {
                return;
            }
        }

        int y = (int)((position.y) * floorCounter);

        if (y < 0 || y >= floorCounter)
        {
            return;
        }

        float fx = position.x * 2 * radius;
        float fz = position.z * 2 * radius;
        float distanceTo0 = Mathf.Sqrt(fx * fx + fz * fz);
        if (distanceTo0 < cylinderledseq.pillar || distanceTo0 > radius)
        {
            return;
        }
        int x = (int)(distanceTo0 / step - 1.5f);
        x = x < 0 ? 0 : x;
        x = x >= roundsCounter ? roundsCounter - 1 : x;

        int angleShareCount = positions[x].Length;
        float oneShare = 2f * Mathf.PI / angleShareCount;
        float angle = Mathf.Acos(fx / distanceTo0);
        angle = fz >= 0 ? angle : 2 * Mathf.PI - angle;
        int z = (int)(angle / oneShare);
        z = z < 0 || z == angleShareCount ? 0 : z;

        if (x >= roundsCounter || z > angleShareCount)
        {
            Debug.Log(x + " " + roundsCounter + " " + z + " " + angleShareCount + " " + angle + " " + oneShare);
            return;
        }

        ledseq.setRealLed(y, x, z, color2uint(color, currentSize));
    }
}

