using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeshObject : MonoBehaviour, IMeshObject
{
    IMeshObject impl;

    public GameObject MeshObjectGetGameObject()
    {
        throw new NotImplementedException();
    }

    public Material[] MeshObjectGetMaterials()
    {
        throw new NotImplementedException();
    }

    public Mesh MeshObjectGetMesh()
    {
        throw new NotImplementedException();
    }

    public void MeshObjectOnEvent(WorldEvent e)
    {
        throw new NotImplementedException();
    }

    public Vector3 MeshObjectTransformPoint(Vector3 v)
    {
        throw new NotImplementedException();
    }

    public void MeshObjectUpdate(MeshObjectUpdateStatus s)
    {
        throw new NotImplementedException();
    }

    public IMeshEventListener MeshObjectListener()
    {
        return impl.MeshObjectListener();
    }
}