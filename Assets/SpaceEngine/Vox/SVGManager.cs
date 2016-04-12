using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class SVGManager
{
    static float pscale = .1f;

    public static unsafe float* pointbuf = getPointbuf();

    public static unsafe int* colorbuf = getColorbuf();

    public static unsafe int* pathbuf = getPathbuf();

    [DllImport("TestSVG")]
    public static unsafe extern float* getPointbuf();

    [DllImport("TestSVG")]
    public static unsafe extern int* getPathbuf();

    [DllImport("TestSVG")]
    public static unsafe extern int* getColorbuf();

    [DllImport("TestSVG")]
    public static unsafe extern int parse(string name);

    [DllImport("TestSVG")]
    public static unsafe extern void construct();

    [DllImport("TestSVG")]
    public static unsafe extern void deconstruct();

    public static GameObject create(string filename, bool fillWithPainter = false, Color painter = new Color())
    {
        int pathnum = parse(filename);
        int pointnum = 0;
        int idx = 0, lastidx = 0, lenindices = 0;

        unsafe
        {
            for (int i = 0; i < pathnum; i++)
            {
                pointnum += pathbuf[i];
            }
        }

        GameObject parent = new GameObject();
        Vector2[] ppt = new Vector2[pointnum];
        Material[] mats = new Material[pathnum];
        Vector3[] vertices = new Vector3[pointnum];
        int[][] indicesarr = new int[pathnum][];
        Mesh msh = new Mesh();
        msh.subMeshCount = pathnum;
        for (int i = 0; i < pathnum; i++)
        {
            Vector2[] singleppt = null;
            Color color = new Color(1, 1, 1, 1);
            unsafe
            {
                int len = pathbuf[i];
                int intcolor = colorbuf[i];
                if (fillWithPainter)
                {
                    color = painter;
                }
                else if (intcolor != 0)
                {
                    color.r = (intcolor & 0xff) / 255f;
                    color.g = ((intcolor >> 8) & 0xff) / 255f;
                    color.b = ((intcolor >> 16) & 0xff) / 255f;
                }
                else
                {
                    color.a = 0;
                }

                singleppt = new Vector2[len];
                for (int j = 0; j < len; j++)
                {
                    ppt[j + idx] = singleppt[j] = new Vector2(pointbuf[(idx + j) * 2], pointbuf[(idx + j) * 2 + 1]);
                }
                lastidx = idx;
                idx += len;
            }

            Triangulator tr = new Triangulator(singleppt);
            int[] indices = tr.Triangulate("revert");
            int indicelen = indices.Length;
            lenindices += indicelen;
            for (int j = 0; j < indicelen; j++)
            {
                indices[j] += lastidx;
            }
            indicesarr[i] = indices;

            Material pmat = new Material(Shader.Find("Transparent/VertexLitDouble"));
            pmat.color = color;
            mats[i] = pmat;
        }

        int pptlen = ppt.Length;
        for (int i1 = 0; i1 < pptlen; i1++)
        {
            vertices[i1] = new Vector3(pscale * ppt[i1].x, -pscale * ppt[i1].y, 0);
        }
        msh.vertices = vertices;

        for (int i = 0; i < pathnum; i++)
        {
            msh.SetIndices(indicesarr[i], MeshTopology.Triangles, i);
        }
        msh.RecalculateNormals();
        msh.RecalculateBounds();
        msh.uv = ppt;
        msh.Optimize();
        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();
        parent.GetComponent<MeshFilter>().mesh = msh;
        parent.GetComponent<MeshRenderer>().materials = mats;
        return parent;
    }

}
