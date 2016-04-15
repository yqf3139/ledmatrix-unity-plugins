using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;

public abstract class DefaultVoxManager : MonoBehaviour {

    static System.Random rand = new System.Random();

    static DefaultVoxManager manager;

    public bool NeedObjectVox = true;
    public bool NeedParticleVox = true;

    public bool NeedEmulator = true;
    public bool Need2DClick = true;
    public bool NeedClickRipple = true;
    public bool NeedBridge = false;
    public bool NeedSaveSeq = false;
    public bool NeedSolidFill = true;
    public bool NeedAutoSearch = true;
    public bool NeedGradientColor = false;
    public bool NeedRegister = true;

    public int fillColor = 0x7f7f7f00;

    public string SeqName = "C:\\Users\\yqf\\Desktop\\test.ledseq";
    public uint SeqFrames = 30 * 20;

    public float objectWorldExtend;
    public float particlesWorldExtend;
    public Vector3 objectWorldCenter;
    public Vector3 particlesWorldCenter;

    public Bounds objectsWorld;
    public Bounds particlesWorld;
    public Bounds ledWorld;

    public static HashSet<IMeshObject> objs = new HashSet<IMeshObject>();
    public static HashSet<IMeshEventListener> liss = new HashSet<IMeshEventListener>();
    public static HashSet<IParticleObject> pos = new HashSet<IParticleObject>();
    public static HashSet<IParticleEventListener> pliss = new HashSet<IParticleEventListener>();

    public HashSet<Thread> workers = new HashSet<Thread>();

    public static DefaultVoxManager getDefault()
    {
        return manager;
    }

    GameObject glow;
    ParticleSystem glowps;
    GameObject ripple;
    ParticleSystem rippleps;

    LedSeq ledseq = null;
    LedMatrix emulator = null;
    LedMatrix bridge = null;
    ParticleVox pvox = null;
    MeshVox vox = null;

    protected abstract void initConfig();
    protected abstract void initBounds();

    protected abstract void initWorkers(HashSet<Thread> s);
    protected abstract LedSeq initLedSeq();
    protected abstract MeshVox initMeshVox(LedSeq s, Bounds b, HashSet<IMeshObject> ss);
    protected abstract ParticleVox initParticleVox(LedSeq s, Bounds b, HashSet<IParticleObject> ss);
    protected abstract LedMatrix initEmulator(LedSeq s);
    protected abstract LedMatrix initBridge(LedSeq s);

    protected virtual void initParticleListeners(HashSet<IParticleEventListener> s)
    {
        if (!NeedAutoSearch) return;
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if (typeof(IParticleEventListener).IsAssignableFrom(monoScript.GetType()))
            {
                s.Add(monoScript as IParticleEventListener);
            }
        }
    }

    protected virtual void initMeshListeners(HashSet<IMeshEventListener> s)
    {
        //Auto select the objects in the bound
        if (!NeedAutoSearch) return;
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if (typeof(IMeshEventListener).IsAssignableFrom(monoScript.GetType()))
            {
                s.Add(monoScript as IMeshEventListener);
            }
        }
    }

    protected virtual void initVoxObjects(HashSet<IMeshObject> s)
    {
        //Auto select the objects in the bound
        if (!NeedAutoSearch) return;
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if (typeof(IMeshObject).IsAssignableFrom(monoScript.GetType()))
            {
                s.Add(monoScript as IMeshObject);
            }
        }
    }
    protected virtual void initParticleObjects(HashSet<IParticleObject> s)
    {
        //Auto select the particle systems
        if (!NeedAutoSearch) return;
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if (typeof(IParticleObject).IsAssignableFrom(monoScript.GetType()))
            {
                s.Add(monoScript as IParticleObject);
            }
        }
    }

    void Awake()
    {
        UnityEngine.Debug.Log("Awake");
        if (NeedRegister)
        {
            manager = this;
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 20;

        initConfig();
        initBounds();
        ledseq = initLedSeq();
    }

    // Use this for initialization
    void Start()
    {
        UnityEngine.Debug.Log("Start");

        Loom.Initialize();

        initWorkers(workers);

        foreach (Thread w in workers)
        {
            w.Start();
        }

        Vector3 ratio = ledseq.getRatio();
        objectsWorld = new Bounds(objectWorldCenter, 2 * objectWorldExtend * ratio);
        particlesWorld = new Bounds(particlesWorldCenter, 2 * particlesWorldExtend * ratio);

        if (NeedSaveSeq)
        {
            ledseq.open(SeqName, SeqFrames);
        }

        if (NeedObjectVox)
        {
            initVoxObjects(objs);
            initMeshListeners(liss);
            vox = initMeshVox(ledseq, objectsWorld, objs);
            vox.Start();
            MeshVox.setGradientColor(NeedGradientColor, 1, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff);
            MeshVox.setSolidFill(NeedSolidFill);
            MeshVox.setFillColor(fillColor);
        }

        if (NeedParticleVox)
        {
            initParticleObjects(pos);
            initParticleListeners(pliss);
            pvox = initParticleVox(ledseq, particlesWorld, pos);
        }

        if (NeedClickRipple)
        {
            glow = GameObject.Find("/Glow");
            glowps = glow.GetComponent<ParticleSystem>();
            ripple = GameObject.Find("/Ripple");
            rippleps = ripple.GetComponent<ParticleSystem>();
            if (ripple.GetComponent<IParticleObject>() != null)
            {
                pos.Add(ripple.GetComponent<IParticleObject>());
            }
        }

        if (NeedEmulator)
        {
            emulator = initEmulator(ledseq);
            ledWorld = emulator.getLedBound();
            GameObject marker = new GameObject("Marker");
            marker.transform.position = ledWorld.center;
        }

        if (NeedBridge)
        {
            bridge = initBridge(ledseq);
        }
    }

    public virtual void onTouch(Vector3 e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e);
        WorldEvent eventForObj = null, eventForParticle = null;

        float dis;
        if (ledWorld.IntersectRay(ray, out dis))
        {
            Vector3 hit = ray.origin + dis * Vector3.Normalize(ray.direction);

            // translate to the obj and particle world position and OnEvent()
            eventForObj = new WorldEvent() { position = TranslatePosition(ledWorld, objectsWorld, hit) };
            eventForParticle = new WorldEvent() { position = TranslatePosition(ledWorld, particlesWorld, hit) };

            if (NeedClickRipple)
            {
                glow.transform.position = hit;
                glowps.Play();
                ripple.transform.position = eventForParticle.position;
                rippleps.Play();
            }

            if (NeedObjectVox)
            {
                vox.OnEvent(eventForObj);
                foreach (IMeshEventListener l in liss)
                    l.MeshObjectOnEvent(eventForObj);
            }
            if (NeedParticleVox)
            {
                pvox.OnEvent(eventForParticle);
                foreach (IParticleEventListener l in pliss)
                    l.ParticleObjectOnEvent(eventForParticle);
            }
        }
    }

    public Vector3 getRatio()
    {
        return ledseq.getRatio();
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
    void Update ()
    {
        // use cam-mouse ray to cal the led world position
        if (NeedEmulator && Need2DClick && Input.GetButtonDown("Fire1"))
        {
            onTouch(Input.mousePosition);
        } 

        // update real ledseq data and serialize data
        ledseq.clear();
        if (NeedObjectVox)
        {
            vox.Update();
        }
        if (NeedParticleVox)
        {
            pvox.Update();
        }
        if (NeedSaveSeq)
        {
            ledseq.save();
        }
        if (NeedEmulator)
        {
            emulator.showLed();
        }
        if (bridge != null)
        {
            bridge.showLed();
        }
    }

    void OnDisable()
    {
        UnityEngine.Debug.Log("on disable");

        if (bridge != null)
        {
            bridge.dispose();
        }
        if (NeedObjectVox)
        {
            vox.OnDisable();
        }
        if (NeedSaveSeq)
        {
            ledseq.close();
        }
        foreach (Thread w in workers)
        {
            w.Interrupt();
            w.Join();
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

    // SVG examples 
    //SVGManager.construct();
    //GameObject twitter = SVGManager.create("C:\\Users\\yqf\\Desktop\\tree.svg");

    //twitter.transform.position = new Vector3(0, 10, 0);
    //twitter.transform.localScale = new Vector3(3, 3, 3);

    //GameObject dd = SVGManager.create("C:\\Users\\yqf\\Desktop\\23.svg");

    //dd.transform.position = new Vector3(0, 0, 0);
    //dd.transform.localScale = new Vector3(1, 1, 1);
}
