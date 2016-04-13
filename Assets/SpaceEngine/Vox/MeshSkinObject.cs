using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeshSkinObject : MonoBehaviour, IMeshObject
{
    IMeshObject impl;
    public IMeshEventListener listener = null;

    void Start()
    {
        impl = new MeshSkinObjectImpl(transform);
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

    public IMeshEventListener MeshObjectListener()
    {
        if (listener != null)
        {
            return listener;
        }
        return impl.MeshObjectListener();
    }
}
