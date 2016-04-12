using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Threading;

public class VoxManager : MonoBehaviour
{

    static VoxManager manager;

    VoxManager()
    {
        manager = this;
    }

    public static VoxManager getDefault()
    {
        return manager;
    }

    static System.Random rand = new System.Random();

    const string phoneip = "192.168.0.107";

    public enum Gesture
    {
        STILL,
        PUSH,
        PULL,
        PUSH_PULL_PUSH,
        PULL_PUSH_PULL,
        PULL_PUSH_PULL_PUSH,
        PUSH_PULL,
        PULL_PUSH,
        PUSH_PULL_PUSH_PULL,
        SWIPE_LEFT_L,
        SWIPE_RIGHT_L,
        SWIPE_LEFT_P,
        SWIPE_RIGHT_P,
        SWING_LEFT_L,
        SWING_RIGHT_L,
        SWING_LEFT_P,
        SWING_RIGHT_P,
        SWIPE_BACK_LEFT_L,
        SWIPE_BACK_RIGHT_L,
        SWIPE_BACK_LEFT_P,
        SWIPE_BACK_RIGHT_P,
        CROSSOVER,
        CROSSOVER_CLOCKWISE,
        CROSSOVER_ANTICLOCK
    };

    public GameObject giraffe;
    public GameObject dolphin;
    public GameObject fish;
    public GameObject fishanimator;
    public GameObject text;
    public GameObject sth;
    public GameObject glow;
    public ParticleSystem glowps;
    public GameObject ripple;
    public GameObject bubble;
    public ParticleSystem rippleps;

    public GameObject buddy;
    public GameObject face;

    public GameObject stag;

    public HashSet<IMeshObject> objs;

    MeshVox vox = null;
    LedSeq ledseq = null;
    LedMatrix matrix = null;
    LedMatrix bridge = null;
    ParticleVox pvox = null;

    Bounds objectsWorld, particlesWorld, ledWorld;

    Thread touchControlThread;
    NetworkStream touchControlStream;

    Thread sonicControlThread;
    NetworkStream sonicControlStream;

    Thread gestureControlThread;
    NetworkStream gestureControlStream;

    char[] touchControlSplit = new char[] { ' ' };

    float sonicRadius = 4f;

    void touchControl()
    {
        Debug.Log("touchControl TCP thread start");

        TcpClient client = new TcpClient();

        try
        {
            client.Connect("127.0.0.1", 12345);
        }
        catch (Exception e)
        {
            Debug.LogError("touchControl server is not available");
            return;
        }
        touchControlStream = client.GetStream();

        Debug.Log("touchControl connected");

        byte[] buf = new byte[64];

        int num;
        try
        {
            num = touchControlStream.Read(buf, 0, 64);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }

        while (num != 0)
        {
            string result = System.Text.Encoding.UTF8.GetString(buf);
            Debug.Log(result);
            string[] data = result.Split(touchControlSplit);

            float fx, fy;
            fx = float.Parse(data[0]);
            fy = float.Parse(data[1]);
            Debug.Log("x " + fx + " y " + fy);

            Loom.QueueOnMainThread(() =>
            {
                onTouch(new Vector3(Screen.width * fx, Screen.height * fy));
            });

            try
            {
                num = touchControlStream.Read(buf, 0, 64);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }
        Debug.Log("touchControl TCP thread end");
    }

    void sonicControl()
    {
        Debug.Log("sonicControl TCP thread start");

        TcpClient client = new TcpClient();

        try
        {
            client.Connect(phoneip, 13139);
        }
        catch (Exception e)
        {
            Debug.LogError("sonicControl server is not available");
            return;
        }
        sonicControlStream = client.GetStream();

        Debug.Log("sonicControl connected");

        byte[] buf = new byte[9];

        int num;
        try
        {
            num = sonicControlStream.Read(buf, 0, buf.Length);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }

        while (num != 0)
        {
            string result = System.Text.Encoding.UTF8.GetString(buf);
            //Debug.Log(result);

            double radius = 0;

            try
            {
                radius = double.Parse(result);
            }
            catch (Exception e)
            {
                //Debug.LogError(e);
            }
            if (radius > -0.1f && radius < 2.1f)
            {
                sonicRadius = 2f + ((float)radius) * 10f;
                Debug.Log(sonicRadius);
            }

            try
            {
                num = sonicControlStream.Read(buf, 0, buf.Length);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }
        Debug.Log("sonicControl TCP thread end");
    }

    void gestureControl()
    {
        Debug.Log("gestureControl TCP thread start");

        TcpClient client = new TcpClient();

        try
        {
            client.Connect(phoneip, 13138);
        }
        catch (Exception e)
        {
            Debug.LogError("gestureControl server is not available");
            return;
        }
        gestureControlStream = client.GetStream();

        Debug.Log("gestureControl connected");

        byte[] buf = new byte[4];

        int num;
        try
        {
            num = gestureControlStream.Read(buf, 0, buf.Length);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }

        while (num != 0)
        {
            string result = System.Text.Encoding.UTF8.GetString(buf);
            Debug.Log(DateTime.UtcNow + " " + result);

            Gesture g = (Gesture)Enum.Parse(typeof(Gesture), result);
            Debug.Log(DateTime.UtcNow + " " + g);

            switch (g)
            {
                case Gesture.STILL:
                    break;
                case Gesture.PUSH:
                    break;
                case Gesture.PULL:
                    break;
                case Gesture.PUSH_PULL_PUSH:
                    Loom.QueueOnMainThread(() =>
                    {
                        onTouch(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
                    });
                    break;
                case Gesture.PULL_PUSH_PULL:
                    break;
                case Gesture.PULL_PUSH_PULL_PUSH:
                    break;
                case Gesture.PUSH_PULL:
                    break;
                case Gesture.PULL_PUSH:
                    break;
                case Gesture.PUSH_PULL_PUSH_PULL:
                    break;
                case Gesture.SWIPE_LEFT_L:
                    Loom.QueueOnMainThread(() =>
                    {
                        onTouch(new Vector3(Screen.width * 0.25f, Screen.height * 0.5f));
                    });
                    break;
                case Gesture.SWIPE_RIGHT_L:
                    Loom.QueueOnMainThread(() =>
                    {
                        onTouch(new Vector3(Screen.width * 0.75f, Screen.height * 0.5f));
                    });
                    break;
                case Gesture.SWIPE_LEFT_P:
                    Loom.QueueOnMainThread(() =>
                    {
                        onTouch(new Vector3(Screen.width * 0.25f, Screen.height * 0.5f));
                    });
                    break;
                case Gesture.SWIPE_RIGHT_P:
                    Loom.QueueOnMainThread(() =>
                    {
                        onTouch(new Vector3(Screen.width * 0.75f, Screen.height * 0.5f));
                    });
                    break;
                case Gesture.SWING_LEFT_L:
                    break;
                case Gesture.SWING_RIGHT_L:
                    break;
                case Gesture.SWING_LEFT_P:
                    break;
                case Gesture.SWING_RIGHT_P:
                    break;
                case Gesture.SWIPE_BACK_LEFT_L:
                    break;
                case Gesture.SWIPE_BACK_RIGHT_L:
                    break;
                case Gesture.SWIPE_BACK_LEFT_P:
                    break;
                case Gesture.SWIPE_BACK_RIGHT_P:
                    break;
                case Gesture.CROSSOVER:
                    break;
                case Gesture.CROSSOVER_CLOCKWISE:
                    break;
                case Gesture.CROSSOVER_ANTICLOCK:
                    break;
                default:
                    break;
            }

            try
            {
                num = gestureControlStream.Read(buf, 0, buf.Length);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }
        Debug.Log("gestureControl TCP thread end");
    }

    void Awake()
    {
        Debug.Log("Awake");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 20;
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");

        Loom.Initialize();

        //touchControlThread = new Thread(touchControl);
        //touchControlThread.Start();

        //sonicControlThread = new Thread(sonicControl);
        //sonicControlThread.Start();

        //gestureControlThread = new Thread(gestureControl);
        //gestureControlThread.Start();

        giraffe = GameObject.Find("/Giraffe");
        dolphin = GameObject.Find("/dolphin");
        fish = GameObject.Find("transformFizzBait");
        fishanimator = GameObject.Find("/fish");
        sth = GameObject.Find("/sth");
        glow = GameObject.Find("/Glow");
        glowps = glow.GetComponent<ParticleSystem>();
        ripple = GameObject.Find("/Ripple");
        rippleps = ripple.GetComponent<ParticleSystem>();
        bubble = GameObject.Find("/bubble");
        buddy = GameObject.Find("U_Char");
        face = GameObject.Find("Bruce_Willis_Face");
        stag = GameObject.Find("STAG/STAG");

        GameObject test = GameObject.Find("/test");
        GameObject mermaid = GameObject.Find("/Mermaid/Mermaid");

        Animator aa = null;
        if (fishanimator != null)
        {
            aa = fishanimator.GetComponent<Animator>();
            aa.speed = 0.4f;
        }

        //objectsWorld = new Bounds(new Vector3(0, 11.5f, 0), new Vector3(7, 11.5f, 7) * 2);
        //particlesWorld = new Bounds(new Vector3(0, 36, 0), new Vector3(18, 36, 18) * 2);

        objectsWorld = new Bounds(new Vector3(0, 9f, 0), new Vector3(18, 9f, 18) * 2);
        //particlesWorld = new Bounds(new Vector3(0, 18, 0), new Vector3(27, 18, 27) * 2);
        particlesWorld = new Bounds(new Vector3(0, 18, 0), new Vector3(27, 18, 27) * 2);

        //ParticleSystem p = GameObject.Find("/FireworksGood").GetComponent<ParticleSystem>();
        HashSet<IParticleObject> pos = new HashSet<IParticleObject>
        {
           //new ParticleSystemObject(particlesWorld, rippleps, new Vector2())
        };


        Vector3 fishvelocity;
        if (GameObject.Find("/sth") != null)
        {
            fishvelocity = 5 * RandomWalkVelocity(objectsWorld, GameObject.Find("/sth").transform.position);
        }

        objs = new HashSet<IMeshObject> {
            //new IMeshObject {
            //    gameObject = mermaid,
            //    mode = MeshMode.SkinnedMeshRendererMode,
            //    transform = mermaid.transform,
            //},
            //new VoxObject {
            //    gameObject = GameObject.Find("/Campfire_Animated"),
            //    mode = MeshMode.MeshFilterMode,
            //    transform = GameObject.Find("/Campfire_Animated").transform,
            //},
            //new VoxObject {
            //    gameObject = GameObject.Find("WTF"),
            //    mode = MeshMode.SkinnedMeshRendererMode,
            //    transform = GameObject.Find("WTF").transform,
            //},
            //new VoxObject {
            //    gameObject = stag,
            //    mode = MeshMode.SkinnedMeshRendererMode,
            //    transform = stag.transform,
            //},
            //new VoxObject {
            //    gameObject = GameObject.Find("11_Hand_Right"),
            //    mode = MeshMode.MeshFilterMode,
            //    transform = GameObject.Find("11_Hand_Right").transform,
            //},
            //new VoxObject {
            //    gameObject = GameObject.Find("07_Hand_Left"),
            //    mode = MeshMode.MeshFilterMode,
            //    transform = GameObject.Find("07_Hand_Left").transform,
            //},
            //new VoxObject {
            //    gameObject = face,
            //    mode = MeshMode.SkinnedMeshRendererMode,
            //    transform = face.transform,
            //},
            //new VoxObject {
            //    gameObject = buddy,
            //    mode = MeshMode.SkinnedMeshRendererMode,
            //    transform = buddy.transform,
            //},
            //new VoxObject {
            //    gameObject = sth,
            //    mode = MeshMode.MeshFilterMode,
            //    transform = sth.transform,
            //    Update = (VoxObject.MeshObjectUpdateStatus status) =>
            //    {
            //        if (status != VoxObject.MeshObjectUpdateStatus.IN)
            //        {
            //            fishvelocity = 5 * RandomWalkVelocity(objectsWorld, sth.transform.position);
            //                                Quaternion q = sth.transform.rotation;
            //            Vector3 v = fishvelocity;
            //            v.y = 0;
            //            v = Vector3.Normalize(v);
            //            float angle = v.z > 0 ? Mathf.Acos(v.x) : 2 * Mathf.PI - Mathf.Acos(v.x);
            //            //q.y = 270f - 360f * angle;
            //            q.eulerAngles = new Vector3(90, 360f - 360f * angle / (2 * Mathf.PI), 0);
            //            sth.transform.rotation = q;
            //            Debug.Log("new dir");
            //        }
            //        sth.transform.position += fishvelocity * Time.deltaTime;
            //        //aa.Play("Swim");
            //    },
            //    OnEvent = (WorldEvent e) =>
            //    {
            //        fishvelocity = 5 * WalkVelocity(e.position, sth.transform.position);
            //        Quaternion q = sth.transform.rotation;
            //        Vector3 v = e.position - objectsWorld.center;
            //        v.y = 0;
            //        v = Vector3.Normalize(v);
            //        float angle = v.z > 0 ? Mathf.Acos(v.x) : 2 * Mathf.PI - Mathf.Acos(v.x);
            //        q.eulerAngles = new Vector3(90, 360f - 360f * angle / (2 * Mathf.PI), 0);
            //        sth.transform.rotation = q;
            //    }
            //},
            //new VoxObject {
            //    gameObject = fish,
            //    mode = MeshMode.SkinnedMeshRendererMode,
            //    transform = fish.transform,
            //    Update = (VoxObject.MeshObjectUpdateStatus status) =>
            //    {
            //        //if (status != VoxObject.MeshObjectUpdateStatus.IN)
            //        //{
            //        //    fishvelocity = RandomVelocity(world, fish.transform.position);
            //        //    Debug.Log("new dir");
            //        //}
            //        //fish.transform.position += fishvelocity * 2 * Time.deltaTime;
            //        aa.Play("Swim");
            //    }
            //},
            //new VoxObject{
            //    gameObject = dd,
            //    mode = MeshMode.MeshFilterMode,
            //    transform = dd.transform
            //},
        };

        //extract(new Transform[]
        //{
        //    //GameObject.Find("/flower01").transform,
        //    //GameObject.Find("/flower02").transform,
        //    //GameObject.Find("/flower03").transform,
        //    //GameObject.Find("/flower05").transform,
        //    //GameObject.Find("/rose").transform,
        //    //GameObject.Find("/flower06").transform,
        //}, objs);

        //CubeLedSeq s = new CubeLedSeq(21, 33, 21);
        //CylinderLeqSeq s = new CylinderLeqSeq(56, 12, 7, 7, 5, 7);
        CylinderLeqSeq s = new CylinderLeqSeq(25, 8, 15f, 15f, 6f, 8.5f);

        //CylinderLeqSeq s = new CylinderLeqSeq(40, 12, 15f, 15f, 6f, 8.5f);

        //CylinderLeqSeq s = new CylinderLeqSeq(32, 2, 15, 15, 6, 0);

        //CylinderLeqSeq s = new CylinderLeqSeq(80, 18, 15, 15, 6, 0);

        ledseq = s;
        //s.open("C:\\Users\\yqf\\Desktop\\test.ledseq", 30 * 20);
        //vox = new CubeMeshVox(s, objs, objectsWorld);

        //vox = new CylinderMeshVox(s, objectsWorld, objs);
        vox.Start();
        MeshVox.setGradientColor(false, 1, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff);
        MeshVox.setSolidFill(true);
        //MeshVox.setFillColor(0x7f7f7f00);

        pvox = new CylinderParticleVox(s, particlesWorld, pos);

        //matrix = new CubeLedMatrix(s);
        matrix = new CylinderLedMatrix(s);
        //bridge = new CylinderBigLedBridge(s);
        ledWorld = matrix.getLedBound();
    }

    public void onTouch(Vector3 e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e);
        WorldEvent eventForObj = null, eventForParticle = null;

        float dis;
        if (ledWorld.IntersectRay(ray, out dis))
        {
            Vector3 hit = ray.origin + dis * Vector3.Normalize(ray.direction);
            Debug.Log("ray " + hit + " " + Input.mousePosition);

            glow.transform.position = hit;

            // translate to the obj and particle world position and OnEvent()
            eventForObj = new WorldEvent() { position = TranslatePosition(ledWorld, objectsWorld, hit) };
            eventForParticle = new WorldEvent() { position = TranslatePosition(ledWorld, particlesWorld, hit) };

            glowps.Play();

            vox.OnEvent(eventForObj);
            ripple.transform.position = eventForParticle.position;
            rippleps.Play();
            pvox.OnEvent(eventForParticle);
        }
    }

    public void onBubble(Vector3 hit)
    {
        Loom.QueueOnMainThread(() =>
        {
            ripple.transform.position = TranslatePosition(objectsWorld, particlesWorld, hit);
            rippleps.Play();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (bubble != null)
        {
            bubble.transform.localScale = new Vector3(sonicRadius, sonicRadius, sonicRadius);
        }

        // use cam-mouse ray to cal the led world position
        if (Input.GetButtonDown("Fire1"))
        {
            onTouch(Input.mousePosition);
        }

        // update real ledseq data and serialize data
        ledseq.clear();
        vox.Update();
        pvox.Update();
        //ledseq.save();
        matrix.showLed();
        if (bridge != null)
        {
            bridge.showLed();
        }
    }

    void OnDisable()
    {
        Debug.Log("on disable");

        if (touchControlStream != null)
        {
            touchControlStream.Close();
        }
        if (sonicControlStream != null)
        {
            sonicControlStream.Close();
        }
        if (bridge != null)
        {
            bridge.dispose();
        }
        vox.OnDisable();
        ////ledseq.close();
        if (touchControlThread != null)
        {
            touchControlThread.Interrupt();
            touchControlThread.Join();
        }
        if (sonicControlThread != null)
        {
            sonicControlThread.Interrupt();
            sonicControlThread.Join();
        }
    }

    public static Vector3 WalkVelocity(Vector3 des, Vector3 p)
    {
        return Vector3.Normalize(des - p);
    }

    public static Vector3 RandomWalkVelocity(Bounds b, Vector3 p)
    {
        Vector3 tmp = b.max - b.min;
        Vector3 des = b.min + new Vector3(tmp.x * (float)rand.NextDouble(), tmp.y * (float)rand.NextDouble(), tmp.z * (float)rand.NextDouble());
        return Vector3.Normalize(des - p);
    }

    public static Vector3 TranslatePosition(Bounds b1, Bounds b2, Vector3 p)
    {
        Vector3 extend1 = p - b1.center;
        Vector3 extend2 = new Vector3(
            extend1.x / b1.extents.x * b2.extents.x,
            extend1.y / b1.extents.y * b2.extents.y,
            extend1.z / b1.extents.z * b2.extents.z);
        return b2.center + extend2;
    }

    //void extract(Transform[] objs, HashSet<IMeshObject> vobjs)
    //{
    //    foreach (Transform tr in objs)
    //    {
    //        // search self
    //        MeshFilter mf = tr.GetComponent<MeshFilter>();
    //        if (mf != null)
    //        {
    //            vobjs.Add(new IMeshObject
    //            {
    //                gameObject = tr.gameObject,
    //                mode = MeshMode.MeshFilterMode,
    //                transform = tr
    //            });
    //        }
    //        if (tr.childCount == 0)
    //        {
    //            continue;
    //        }
    //        MeshRenderer[] children = tr.GetComponentsInChildren<MeshRenderer>();
    //        Transform[] trs = new Transform[children.Length];
    //        for (int i = 0; i < trs.Length; i++)
    //        {
    //            trs[i] = children[i].transform;
    //        }
    //        extract(trs, vobjs);
    //    }
    //}
}
