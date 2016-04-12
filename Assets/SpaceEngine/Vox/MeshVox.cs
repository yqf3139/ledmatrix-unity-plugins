using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

public enum MeshMode { MeshFilterMode, SkinnedMeshRendererMode, TextMode };

public abstract class MeshVox : Vox
{
    static int[,] signs = new int[8, 3]
    {
            { -1,-1,-1 },
            { -1,-1,1 },
            { -1,1,-1 },
            { -1,1,1 },
            { 1,-1,-1 },
            { 1,-1,1 },
            { 1,1,-1 },
            { 1,1,1 },
    };

    const string VOXDLLNAME = "testdll";

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern int* getResultBuffer();

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern int* getTriBuffer();

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern float* getVertexBuffer();

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern int* getColourBuffer();

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern int* getTriMatBuffer();

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern float* getBondBuffer();

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern void voxel(int numobjects, int numverts, int numtris, int matnum);

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern void construct(int ledx, int ledy, int ledz);

    [DllImport(VOXDLLNAME)]
    protected static unsafe extern void deconstruct();

    [DllImport(VOXDLLNAME)]
    public static unsafe extern void setGradientColor(
        bool _enable, int _axle, int _g0r, int _g0g, int _g0b, int _g1r, int _g1g, int _g1b);

    [DllImport(VOXDLLNAME)]
    public static unsafe extern void setSolidFill(bool _useSolidFill);

    [DllImport(VOXDLLNAME)]
    public static unsafe extern void setFillColor(int _color);

    protected unsafe int* tribuf = null;
    protected unsafe int* resbuf = null;
    protected unsafe float* vertbuf = null;
    protected unsafe int* colorbuf = null;
    protected unsafe int* trimatbuf = null;
    protected unsafe float* bondbuf = null;

    const int TRILIMIT = 32768 << 2, VERTLIMIT = 32768 << 2;

    protected int numobjects = 0, numverts = 0, numtris = 0, namplc = 0, nummats = 0, numtrimats = 0;

    protected HashSet<IMeshObject> voxObjects = new HashSet<IMeshObject>();
    protected HashSet<IVoxListener> listeners = new HashSet<IVoxListener>();

    public MeshVox(LedSeq seq, Bounds worldBounds, HashSet<IMeshObject> objs, HashSet<IVoxListener> liss)
        :base(seq, worldBounds)
    {
        voxObjects = objs == null ? voxObjects : objs;
        listeners = liss == null ? listeners : liss;
    }

    public void Start()
    {
        VoxStart();
        unsafe
        {
            tribuf = getTriBuffer();
            resbuf = getResultBuffer();
            vertbuf = getVertexBuffer();
            colorbuf = getColourBuffer();
            trimatbuf = getTriMatBuffer();
            bondbuf = getBondBuffer();
            bondbuf[0] = worldBounds.center.x;
            bondbuf[1] = worldBounds.center.y;
            bondbuf[2] = worldBounds.center.z;
            bondbuf[3] = worldBounds.extents.x + 0.5f;
            bondbuf[4] = worldBounds.extents.y + 0.5f;
            bondbuf[5] = worldBounds.extents.z + 0.5f;
        }
    }

    // Use this for initialization
    public abstract void VoxStart();

    public void OnDisable()
    {
        Debug.Log("OnDisable");
        deconstruct();
    }

    public override void OnEvent(WorldEvent e)
    {
        foreach (IVoxListener l in listeners)
        {
            l.MeshObjectOnEvent(e);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        numobjects = numverts = numtris = nummats = numtrimats = 0;
        foreach (IMeshObject o in voxObjects)
        {
            GameObject go = o.MeshObjectGetGameObject();
            // if o is not active, skip it
            if (!go.activeSelf)
            {
                continue;
            }
            MeshObjectUpdateStatus status = MeshObjectUpdateStatus.IN;
            // if o is completely out of the box, skip it
            Renderer r = go.GetComponent<Renderer>();
            if (!worldBounds.Intersects(r.bounds))
            {
                status = MeshObjectUpdateStatus.OUT;
            }
            else
            {
                status = worldBounds.Contains(go.transform.position) ? 
                    MeshObjectUpdateStatus.IN : MeshObjectUpdateStatus.INTERSECT;
            }

            if (o.MeshObjectListener() != null)
            {
                o.MeshObjectListener().MeshObjectUpdate(status);
            }

            if (status != MeshObjectUpdateStatus.OUT)
            {
                numobjects++;
                vox(o);
            }
        }
        voxel(numobjects, numverts, numtris, nummats);
        UpdateLED();
    }

    public abstract void UpdateLED();

    void vox(IMeshObject obj)
    {
        Mesh mesh = null;
        Material[] mats = null;

        mesh = obj.MeshObjectGetMesh();
        mats = obj.MeshObjectGetMaterials();

        // tris
        int[] tris = mesh.triangles;
        int lentris = mesh.triangles.Length;
        for (int i = 0; i < lentris; i++)
        {
            unsafe
            {
                tribuf[numtris + i] = numverts + tris[i];
            }
        }
        numtris += lentris;

        // vertices
        var vertices = mesh.vertices;
        int vertidx = 0;
        int vertslen = vertices.Length;
        for (int i = 0; i < vertslen; i++)
        {
            Vector3 vert1 = obj.MeshObjectTransformPoint(vertices[i]);

            vertidx = 3 * (numverts + i);
            unsafe
            {
                vertbuf[vertidx + 0] = vert1.x;
                vertbuf[vertidx + 1] = vert1.y;
                vertbuf[vertidx + 2] = vert1.z;
            }
        }
        numverts += vertices.Length;

        // trimats or color from texture

        if (mats[0].mainTexture == null)
        {
            int lensubmats = mesh.subMeshCount;
            for (int i = 0; i < lensubmats; i++)
            {
                int[] indices = mesh.GetIndices(i);
                int indicesLen = indices.Length / 3;
                for (int j = 0; j < indicesLen; j++)
                {
                    unsafe
                    {
                        trimatbuf[j + numtrimats] = nummats + i;
                    }
                }
                numtrimats += indicesLen;
            }
        }
        else
        {
            int lensubmats = mesh.subMeshCount;
            for (int i = 0; i < lensubmats; i++)
            {
                Texture2D t2d = mats[i].mainTexture as Texture2D;
                Vector2[] uv = mesh.uv;
                Vector2 target = new Vector2();
                lentris /= 3;
                Color targetColor;
                int c;

                int[] indices = mesh.GetIndices(i);
                int indicesLen = indices.Length / 3;
                for (int j = 0; j < indicesLen; j++)
                {
                    target = uv[indices[3 * j + 0]] + uv[indices[3 * j + 1]] + uv[indices[3 * j + 2]];
                    target /= 3;
                    targetColor = t2d.GetPixelBilinear(target.x, target.y);
                    c = ((int)(255f * targetColor.r)) << 16;
                    c |= ((int)(255f * targetColor.g)) << 8;
                    c |= ((int)(255f * targetColor.b));

                    unsafe
                    {
                        trimatbuf[j + numtrimats] = 0x7f000000 | c;
                    }
                }
                numtrimats += indicesLen;
            }
        }

        // mats
        int intcolor, lenmats = mats.Length;
        Color color;
        for (int i = 0; i < lenmats; i++)
        {
            color = mats[i].color;
            intcolor = 0;
            intcolor |= ((int)(color.r * 255)) << 16;
            intcolor |= ((int)(color.g * 255)) << 8;
            intcolor |= ((int)(color.b * 255));
            unsafe
            {
                colorbuf[nummats + i + 1] = intcolor;
            }
        }
        nummats += mats.Length;

    }
}
