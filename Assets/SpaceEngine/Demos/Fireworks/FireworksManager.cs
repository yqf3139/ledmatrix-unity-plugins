using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Net;

[Serializable]
public class FireworkMessage
{
    public string handler;
    public int type;
    public float height;
    public float x;
    public float y;
    public float time;
}

class Region
{
    public Vector2 center;
    public Vector2 area;
}

public class FireworksManager : MonoBehaviour, IParticleEventListener {

    System.Random rand = new System.Random();

    public float centerstep = 15;
    public float rangestep = 10;
    public long playRandomEffectsThreshold = 10; // seconds

    Thread touchControlThread;
    Thread randomEffectsControlThread;

    IParticleObject[] po = new IParticleObject[9];
    IParticleObject[] randomEffects = new IParticleObject[8];
    IParticleObject fishSplash;

    UpSwimFish fish;

    int idx = 0;
    bool workerThreadStopped = false;

    long lastEffectTimeStamp = UnixTimeNow(); // seconds

    Region[] regions;

    void touchControl()
    {
        Debug.Log("touchControl UDP thread start");

        UdpClient udpClient = new UdpClient(12345);
        udpClient.Client.SendTimeout = 5000;
        udpClient.Client.ReceiveTimeout = 5000;

        Debug.Log("touchControl server started");

        byte[] buf = null;
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (!workerThreadStopped)
        {
            try
            {
                buf = udpClient.Receive(ref RemoteIpEndPoint);
            }
            catch (SocketException e)
            {
                continue;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }

            string result = System.Text.Encoding.UTF8.GetString(buf);

            FireworkMessage m = null;
            try
            {
                m = JsonUtility.FromJson<FireworkMessage>(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e+result);
                continue;
            }

            handleMessage(m);
        }
        Debug.Log("touchControl TCP thread end");
    }

    void randomEffectsControl()
    {
        Debug.Log("playRandomEffects thread start");

        while (!workerThreadStopped)
        {
            if (UnixTimeNow() - lastEffectTimeStamp > playRandomEffectsThreshold)
            {
                Loom.QueueOnMainThread(() =>
                {
                    emitRandomEffects();
                });
            }
            Thread.Sleep(1000);
        }
        Debug.Log("playRandomEffects thread end");
    }

    void handleMessage(FireworkMessage m)
    {
        Debug.Log(JsonUtility.ToJson(m));

        switch (m.handler)
        {
            case "fireworks":
                if (m.type >= 0 && m.type < 9 && m.time >= 1 && m.time <= 10)
                {
                    float height = m.height > 1 ? 1 : m.height < 0 ? 0 : m.height;
                    if (po[m.type] != null)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            //onTouch(new Vector3(Screen.width * fx, Screen.height * fy));
                            po[m.type].ParticleObjectPlay(height, m.time, regions[idx % 4].center, regions[idx % 4].area);
                            idx++; // po.Length;
                            lastEffectTimeStamp = UnixTimeNow();
                        });
                    }
                }
                break;
            case "fish":

                break;
            default:
                break;
        }

    }

    void Start()
    {
        regions = new Region[]
        {
            new Region() { center = new Vector2(centerstep, centerstep), area = new Vector2(rangestep,rangestep) },
            new Region() { center = new Vector2(-centerstep, centerstep), area = new Vector2(rangestep,rangestep) },
            new Region() { center = new Vector2(centerstep, -centerstep), area = new Vector2(rangestep,rangestep) },
            new Region() { center = new Vector2(-centerstep, -centerstep), area = new Vector2(rangestep,rangestep) },
        };

        for (int i = 0; i < po.Length; i++)
        {
            GameObject go = GameObject.Find("/PS" + i);
            if (go == null)
            {
                continue;
            }
            po[i] = go.GetComponent<IParticleObject>();
        }
        for (int i = 0; i < randomEffects.Length; i++)
        {
            GameObject go = GameObject.Find("/Random" + i);
            if (go == null)
            {
                continue;
            }
            randomEffects[i] = go.GetComponent<IParticleObject>();
        }
        GameObject fishSplashObj = GameObject.Find("/FishSplash");
        if (fishSplashObj != null)
        {
            fishSplash = fishSplashObj.GetComponent<IParticleObject>();
        }

        fish = GameObject.Find("transformFizzBait").GetComponent<UpSwimFish>();

        touchControlThread = new Thread(touchControl);
        //touchControlThread.Start();
        randomEffectsControlThread = new Thread(randomEffectsControl);
        randomEffectsControlThread.Start();
    }

    void OnDisable()
    {
        workerThreadStopped = true;
        randomEffectsControlThread.Interrupt();
        randomEffectsControlThread.Join();
        touchControlThread.Interrupt();
        touchControlThread.Join();
    }

    void Update()
    {
        // use cam-mouse ray to cal the led world position
        if (Input.GetButtonDown("Fire2"))
        {
            emitFish();
        }
    }

    void emitRandomEffects()
    {
        Debug.Log("play random " + idx % randomEffects.Length);
        int r = rand.Next(0, randomEffects.Length);
        if (randomEffects[idx % randomEffects.Length] != null)
        {
            randomEffects[idx % randomEffects.Length].ParticleObjectPlay(0.5f, 2f, regions[idx % 4].center, regions[idx % 4].area);
        }
        idx++; // po.Length;
        lastEffectTimeStamp = UnixTimeNow();
    }

    void emitFish()
    {
        if (fishSplash != null)
        {
            fish.swim();
            fishSplash.ParticleObjectPlay(0.5f, 2f, regions[idx % 4].center, regions[idx % 4].area);
        }
        idx++;
        lastEffectTimeStamp = UnixTimeNow();
    }

    static long UnixTimeNow()
    {
        TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalSeconds;
    }

    public void OnInteractionInput(WorldEvent e)
    {
        Debug.Log("play " + idx % 3);
        if (po[idx % 5] != null)
        {
            po[idx % 5].ParticleObjectPlay(0.5f, 2f, regions[idx % 4].center, regions[idx % 4].area);
        }
        idx++; // po.Length;
        lastEffectTimeStamp = UnixTimeNow();
    }

    public void OnCrowdInfo(CrowdInfo[] infos)
    {

    }

    public void OnCrowdInfoSummry(CrowdInfoSummry summary)
    {

    }
}
