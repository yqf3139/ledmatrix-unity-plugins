using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeLedMatrix : LedMatrix
{
    GameObject thecube = null;
    CubeLedSeq cubeledseq = null;
    Bounds bounds;
    public static float step = 10f;
    public static Vector3 origin;

    int LEDX, LEDY, LEDZ;

    const bool reflect = false;

    public CubeLedMatrix(CubeLedSeq ledseq)
        :base(ledseq)
    {
        origin = new Vector3(100f, 0, 500 - ledseq.LEDZ / 2 * step);
        cubeledseq = ledseq;

        LEDX = ledseq.LEDX;
        LEDY = ledseq.LEDY;
        LEDZ = ledseq.LEDZ;

        UnityEngine.Object o = Resources.Load("sampleled");
        thecube = (GameObject)GameObject.Instantiate(o, Vector3.zero, Quaternion.identity);
        Material themat = thecube.GetComponent<MeshRenderer>().material;

        ledcubes = new GameObject[TOTAL];
        ledmats = new Material[TOTAL];

        parent = new GameObject("CubeLedPillar");

        Material transmat = (Material)Resources.Load("transmat", typeof(Material));
        transmat.color = new Color(143f / 255f, 143f / 255f, 143f / 255f, 50 / 255f);

        GameObject go = null;
        for (int i = 0; i < LEDX; i++)
        {
            for (int k = 0; k < LEDZ; k++)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                go.transform.position = new Vector3(i * step, 0, k * step) + new Vector3(0f, step * LEDY / 2, 0f) + origin;
                go.transform.parent = parent.transform;
                go.transform.localScale = new Vector3(1f, step * LEDY / 2, 0.2f);
                go.GetComponent<MeshRenderer>().material = new Material(transmat);
            }
        }

        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "cubetop";
        top.transform.localScale = new Vector3(LEDX * step * 1.1f, 2f, LEDZ * step * 1.1f);
        top.transform.position = new Vector3(LEDX / 2 * step, step * LEDY * 1f, LEDZ / 2 * step) + origin;
        top.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

        bounds = new Bounds(origin, new Vector3());

        parent = new GameObject("CubeLedMatrix");
        GameObject l = GameObject.Find("plight");

        for (int i = 0; i < LEDX; i++)
        {
            for (int j = 0; j < LEDY; j++)
            {
                for (int k = 0; k < LEDZ; k++)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = new Vector3(i * step, j * step, k * step) + origin;
                    go.SetActive(false);
                    go.transform.parent = parent.transform;
                    bounds.Encapsulate(new Vector3(i * step, j * step, k * step) + origin);
                    ledmats[ledseq.getLedIdx(i, j, k)] = go.GetComponent<MeshRenderer>().material = new Material(themat);
                    ledmats[ledseq.getLedIdx(i, j, k)].SetVector("_MKGlowColor", new Vector4(1f, 1f, 1f, 1f));
                    ledcubes[ledseq.getLedIdx(i, j, k)] = go;

                    if (reflect)
                    {
                        GameObject ll = GameObject.Instantiate(l);
                        ll.transform.position = go.transform.position;
                        ll.transform.parent = go.transform;
                    }
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
        ledmats[idx].SetVector("_MKGlowColor", new Vector4(r / 255f, g / 255f, b / 255f, 1f));
    }
}

