using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class MeshObjectImpl : IMeshObject
{
    protected Transform t;

    public MeshObjectImpl(Transform t)
    {
        this.t = t;
    }

    public GameObject MeshObjectGetGameObject()
    {
        throw new NotImplementedException();
    }

    public abstract Material[] MeshObjectGetMaterials();

    public abstract Mesh MeshObjectGetMesh();

    public virtual IMeshEventListener MeshObjectListener()
    {
        return null;
    }

    public abstract Vector3 MeshObjectTransformPoint(Vector3 v);
}

public class MeshNormalObjectImpl : MeshObjectImpl
{
    MeshFilter meshFilter = null;
    MeshRenderer render = null;
    Material[] mats = null;

    public MeshNormalObjectImpl(Transform t)
        : base(t)
    {
        meshFilter = t.GetComponent<MeshFilter>();
        render = t.GetComponent<MeshRenderer>();
    }

    public override Material[] MeshObjectGetMaterials()
    {
        return render.materials;
    }

    public override Mesh MeshObjectGetMesh()
    {
        return meshFilter.mesh;
    }

    public override Vector3 MeshObjectTransformPoint(Vector3 v)
    {
        return meshFilter.transform.TransformPoint(v);
    }
}

public class MeshSkinObjectImpl : MeshObjectImpl
{
    SkinnedMeshRenderer skin;

    public MeshSkinObjectImpl(Transform t)
        : base(t)
    {
        skin = t.GetComponent<SkinnedMeshRenderer>();
    }

    public override Material[] MeshObjectGetMaterials()
    {
        return skin.materials;
    }

    public override Mesh MeshObjectGetMesh()
    {
        Mesh mesh = new Mesh();
        skin.BakeMesh(mesh);
        return mesh;
    }

    public override Vector3 MeshObjectTransformPoint(Vector3 v)
    {
        return v;
    }
}