using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeshNormalObject : MonoBehaviour, IMeshObject
{
    IMeshObject impl;

    void Start()
    {
        impl = new MeshNormalObjectImpl(transform);
    }

    public GameObject MeshObjectGetGameObject()
    {
        return gameObject;
    }

    public Material[] MeshObjectGetMaterials()
    {
        return impl.MeshObjectGetMaterials();
    }

    public Mesh MeshObjectGetMesh()
    {
        return impl.MeshObjectGetMesh();
    }

    public Vector3 MeshObjectTransformPoint(Vector3 v)
    {
        return impl.MeshObjectTransformPoint(v);
    }

    public IVoxListener MeshObjectListener()
    {
        return impl.MeshObjectListener();
    }
}
