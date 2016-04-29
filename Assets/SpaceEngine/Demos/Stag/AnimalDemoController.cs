using UnityEngine;
using System.Collections;
using System;

public class AnimalDemoController : MonoBehaviour, IMeshEventListener
{
    public static AnimalDemoController[] controllers = new AnimalDemoController[3];
    static int counter = 0;

    public MeshSkinObject self;
    protected Animation a;
    protected Vector3 target;
    protected Vector3 groundDir;
    protected Vector3 eulerAngles;

    protected GameObject ground;

    float feedWaitTime = 0f;
    float feedWaitThreshold = 6f;

    bool isInit = true;
    protected bool isFeeding = false;
    protected bool isEatting = false;
    int id;

    GameObject food;

    // Use this for initialization
    public virtual void Start () {

        ground = GameObject.Find("Ground");

        a = GetComponent<Animation>();
        self = GetComponentInChildren<MeshSkinObject>();
        self.listener = this;

        foreach (AnimationState aa in a)
        {
            a[aa.name].wrapMode = WrapMode.Once;
        }

        id = counter++;
        Debug.Log(" " + id);
        if (id > controllers.Length - 1)
        {
            Debug.Log("skip " + id);
        }
        else controllers[id] = this;

    }

    void init()
    {
        if (id != 0)
        {
            Debug.Log(gameObject + " to sleep");
            gameObject.SetActive(false);
            self.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log(gameObject + " selected");
        }

        eulerAngles = self.transform.eulerAngles;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isInit)
        {
            isInit = false;
            init();
        }

        eulerAngles = Vector3.Lerp(eulerAngles, target, Time.deltaTime);
        self.transform.eulerAngles = eulerAngles;

        if (isFeeding)
        {
            // if wait feeding time is too long, animal turn to normal
            feedWaitTime += Time.deltaTime;
            if (feedWaitTime > feedWaitThreshold)
            {
                isFeeding = false;
            }
        }
        else if (isEatting)
        {
            if (isActionDone())
            {
                isEatting = false;
                isFeeding = false;
                food.SetActive(false);
            }
        }
        else
        {
            if (a.IsPlaying("walk"))
            {
                ground.transform.position += Time.deltaTime * groundDir * 6f;
            }

            if (a.IsPlaying("run"))
            {
                ground.transform.position += Time.deltaTime * groundDir * 12f;
            }

            if (ground.transform.position.magnitude > 50)
            {
                ground.transform.position = -ground.transform.position;
                ground.transform.position -= Vector3.Normalize(ground.transform.position);
            }
        }
    }

    public virtual void MeshObjectUpdate(MeshObjectUpdateStatus s)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
    }

    public virtual void OnCrowdInfo(CrowdInfo[] infos)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
    }

    public virtual void OnCrowdInfoSummry(CrowdInfoSummry summary)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (isFeeding)
        {
            return;
        }
        turnToDir(summary.crowdDirection);
    }

    public virtual void OnInteractionInput(WorldEvent e)
    {

    }

    public virtual void PlayDefault()
    {

    }

    public virtual void FeedAttention(Vector3 dir)
    {
        //Debug.LogError("isFeeding" + isFeeding);

        if (isFeeding)
        {
            return;
        }
        turnToDir(dir);
        isFeeding = true;

        feedWaitTime = 0;
    }

    public virtual void Eat(GameObject food, Vector3 dir)
    {
        if (isEatting)
        {
            return;
        }
        this.food = food;

        food.SetActive(true);
        Vector3 center = transform.position;
        food.transform.position = center + Vector3.Normalize(dir - center) * 10 + new Vector3(0,40,0);

        isEatting = true;
    }

    public virtual bool isActionDone()
    {
        return false;
    }

    void turnToDir(Vector3 dir)
    {
        groundDir = dir;
        groundDir.y = 0;
        groundDir = -Vector3.Normalize(groundDir);
        float sin = Mathf.Asin(dir.z);
        float cos = Mathf.Acos(dir.x);

        float angle = cos / Mathf.PI * 180;
        if (sin < 0)
        {
            angle = 360 - angle;
        }

        target = new Vector3(0, (angle + 90f) % 360, 0);

        if (Mathf.Abs(target.y - eulerAngles.y) > 180)
        {
            if (target.y > eulerAngles.y)
                target.y -= 360;
            else
                target.y += 360;
        }

        //Debug.Log("select target : "+target);
    }
}
