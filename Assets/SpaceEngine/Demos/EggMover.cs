using UnityEngine;
using System.Collections;

public class EggMover : MonoBehaviour
{
    IMeshObject o;

    static int counter = 0;

    int id;
    // Use this for initialization
    void Start()
    {
        id = counter++;
        //o = new IMeshObject
        //{
        //    gameObject = gameObject,
        //    mode = MeshMode.MeshFilterMode,
        //    transform = transform,
        //};
        DefaultVoxManager.getDefault().objs.Add(o);
        Debug.Log("mover");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Contains("Hand"))
        {
            //DefaultVoxManager.getDefault().onBubble(col.gameObject.transform.position);
            suicide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            suicide();
        }
    }

    void suicide()
    {
        DefaultVoxManager.getDefault().objs.Remove(o);
        if (id == 0)
        {
            return;
        }
        Destroy(gameObject);
    }
}
