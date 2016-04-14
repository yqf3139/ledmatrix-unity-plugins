using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeMatrix : LedMatrix
{
    GameObject thecube = null;
    CubeLedSeq cubeledseq = null;
    Bounds bounds;
    public static float step = 10f;
    public static Vector3 origin;

    int LEDX, LEDY, LEDZ;

    const bool reflect = false;

    public CubeMatrix(CubeLedSeq ledseq)
        : base(ledseq)
    {
        origin = new Vector3(100f, 0, 500 - ledseq.LEDZ / 2 * step);
        cubeledseq = ledseq;

        LEDX = ledseq.LEDX;
        LEDY = ledseq.LEDY;
        LEDZ = ledseq.LEDZ;

        ledcubes = new GameObject[TOTAL];
        ledmats = new Material[TOTAL];

        GameObject go = null;

        bounds = new Bounds(origin, new Vector3());

        parent = new GameObject("CubeMatrix");
        for (int i = 0; i < LEDX; i++)
        {
            for (int j = 0; j < LEDY; j++)
            {
                for (int k = 0; k < LEDZ; k++)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = new Vector3(i * step, j * step, k * step) + origin;
                    go.transform.localScale = new Vector3(step, step, step);
                    go.SetActive(false);
                    go.transform.parent = parent.transform;
                    bounds.Encapsulate(new Vector3(i * step, j * step, k * step) + origin);
                    ledmats[ledseq.getLedIdx(i, j, k)] = go.GetComponent<MeshRenderer>().material;
                    ledcubes[ledseq.getLedIdx(i, j, k)] = go;
                }
            }
        }
    }

    public override Bounds getLedBound()
    {
        return bounds;
    }

    public override void setColor(int idx, float r, float g, float b, float w)
    {
        ledmats[idx].color = new Color(r / 255f, g / 255f, b / 255f);
    }
}


