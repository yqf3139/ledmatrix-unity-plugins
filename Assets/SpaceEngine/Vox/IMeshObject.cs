using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public interface IMeshObject
{
    Mesh MeshObjectGetMesh();
    Material[] MeshObjectGetMaterials();
    GameObject MeshObjectGetGameObject();
    Vector3 MeshObjectTransformPoint(Vector3 v);
    IMeshEventListener MeshObjectListener();
}

