using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CylinderLedMatrix : LedMatrix
{
    public static Vector3 origin = new Vector3(-100, 0, 500);
    //public static Vector3 origin = new Vector3(0, 0, 0);

    GameObject thecube = null;
    Bounds bounds;

    GameObject[][] pillars = null;
    GameObject pillar = null;
    GameObject top = null;

    const bool reflect = false;

    public CylinderLedMatrix(CylinderLeqSeq ledseq)
        : base(ledseq)
    {
        float height = ledseq.height;
        int floorCounter = ledseq.floorCounter;
        int roundsCounter = ledseq.roundsCounter;

        GameObject l = GameObject.Find("plight");

        UnityEngine.Object o = null;
        o = Resources.Load("sampleled");
        thecube = (GameObject)GameObject.Instantiate(o, Vector3.zero, Quaternion.identity);
        Material themat = new Material(thecube.GetComponent<MeshRenderer>().material);
        themat.color = new Color(1, 1, 1, 1);

        Material transmat = (Material)Resources.Load("transmat", typeof(Material));
        transmat.color = new Color(143f / 255f, 143f / 255f, 143f / 255f, 50 / 255f);

        Vector3 pillarScale = new Vector3(1f, height * floorCounter * 1f / 2, 1f);
        Vector3 pillarCenter = new Vector3(0f , height * floorCounter * 1f / 2, 0f) + origin;

        pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.name = "pillar";
        pillar.transform.localScale = pillarScale + new Vector3(4f, 0, 4f);
        pillar.transform.position = pillarCenter;
        pillar.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

        top = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        top.name = "top";
        top.transform.localScale = new Vector3(ledseq.radius * 2.2f, 2f, ledseq.radius * 2.2f);
        top.transform.position = new Vector3(0, height * floorCounter * 1f, 0f) + origin;
        top.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

        Vector3[][] positions = ledseq.positions;
        pillars = new GameObject[roundsCounter][];
        parent = new GameObject("CubeLedPillar");

        for (int i = 0; i < roundsCounter; i++)
        {
            int counter = positions[i].Length;

            pillars[i] = new GameObject[counter];
            for (int j = 0; j < counter; j++)
            {
                pillars[i][j] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pillars[i][j].transform.localScale = pillarScale;
                pillars[i][j].transform.position = positions[i][j] + pillarCenter;
                pillars[i][j].GetComponent<MeshRenderer>().material = new Material(transmat);
                pillars[i][j].transform.parent = parent.transform;
            }
        }

        bounds = new Bounds(positions[0][0] + origin, new Vector3());

        parent = new GameObject("CylinderLedMatrix");
        ledcubes = new GameObject[TOTAL];
        ledmats = new Material[TOTAL];
        GameObject go = null;
        int len = 0;
        int ledidx = 0;
        for (int i = 0; i < floorCounter; i++)
        {
            for (int j = 0; j < roundsCounter; j++)
            {
                len = positions[j].Length;
                for (int k = 0; k < len; k++)
                {
                    ledidx = ledseq.getLedIdx(i, j, k);
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    
                    go.transform.parent = parent.transform;
                    go.transform.position = positions[j][k] + new Vector3(0, height * i, 0f) + origin;
                    ledcubes[ledidx] = go;
                    ledmats[ledidx] = go.GetComponent<MeshRenderer>().material = new Material(themat);
                    ledmats[ledidx].SetVector("_MKGlowColor", new Vector4(1f, 1f, 1f, 1f));

                    bounds.Encapsulate(go.transform.position);

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

