using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeLedMatrix : LedMatrix
{
    GameObject thecube = null;
    CubeLedSeq cubeledseq = null;
    Bounds bounds = new Bounds();
    const float step = 5f;

    int LEDX, LEDY, LEDZ;

    public CubeLedMatrix(CubeLedSeq ledseq)
        :base(ledseq)
    {
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
                go.transform.position = new Vector3(i * step - 70, 0, k * step) + new Vector3(0f, step * LEDY / 2, 0f);
                go.transform.parent = parent.transform;
                go.transform.localScale = new Vector3(1f, step * LEDY / 2, 0.2f);
                go.GetComponent<MeshRenderer>().material = new Material(transmat);
            }
        }

        parent = new GameObject("CubeLedMatrix");

        for (int i = 0; i < LEDX; i++)
        {
            for (int j = 0; j < LEDY; j++)
            {
                for (int k = 0; k < LEDZ; k++)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = new Vector3(i * step - 70, j * step, k * step);
                    go.SetActive(false);
                    go.transform.parent = parent.transform;
                    bounds.Encapsulate(go.transform.position);
                    go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    ledmats[ledseq.getLedIdx(i, j, k)] = go.GetComponent<MeshRenderer>().material = new Material(themat);
                    ledmats[ledseq.getLedIdx(i, j, k)].SetVector("_MKGlowColor", new Vector4(1f, 1f, 1f, 1f));
                    ledcubes[ledseq.getLedIdx(i, j, k)] = go;
                }
            }
        }
    }

    public override Bounds getLedBound()
    {
        return bounds;
    }
}

