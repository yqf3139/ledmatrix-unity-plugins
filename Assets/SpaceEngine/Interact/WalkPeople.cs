using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WalkPeople : MonoBehaviour
{
    public static int NextID = 0;

    public int id = 0;
    public int gender = 0;
    public int age = 20;
    public int children = 0;

    protected System.Random rand = new System.Random(NextID);

    protected GameObject text;
    protected TextMesh tmesh;
    protected IInteractionInput input;

    protected Vector3 center;
    protected float far;
    protected float near;
    protected float mid;
    protected float far2;
    protected float near2;
    protected float mid2;

    public virtual void init(Vector3 center, float r0, float r1, float r2, IInteractionInput input)
    {
        rand = new System.Random(NextID);
        id = NextID;
        NextID++;

        this.center = center;
        near = r0;
        mid = r1;
        far = r2;
        near2 = near * near;
        mid2 = mid * mid;
        far2 = far * far;

        this.input = input;

        text = new GameObject("text");
        tmesh = text.AddComponent<TextMesh>();
        tmesh.text = "test";
        tmesh.fontSize = 500;
        text.transform.parent = transform;
        text.transform.position = transform.position + new Vector3(0f, 200f, 0f);
        text.SetActive(false);
    }

    public virtual void attr(int gender, int age, int children)
    {
        this.gender = gender;
        this.age = age;
        this.children = children;
    }

    public bool visible()
    {
        return dis2() < mid2;
    }

    public float dis2()
    {
        Vector3 pos = transform.position - center;
        return pos.x * pos.x + pos.z * pos.z;
    }
}