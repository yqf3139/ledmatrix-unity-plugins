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

    Thread touchControlThread;

    IParticleObject[] po = new IParticleObject[9];

    int idx = 0;

    bool touchControlStopped = false;

    const float centerstep = 15;
    const float rangestep = 10;

    Region[] regions = new Region[]
    {
        new Region() { center = new Vector2(centerstep, centerstep), area = new Vector2(rangestep,rangestep) },
        new Region() { center = new Vector2(-centerstep, centerstep), area = new Vector2(rangestep,rangestep) },
        new Region() { center = new Vector2(centerstep, -centerstep), area = new Vector2(rangestep,rangestep) },
        new Region() { center = new Vector2(-centerstep, -centerstep), area = new Vector2(rangestep,rangestep) },
    };

    void touchControl()
    {
        Debug.Log("touchControl UDP thread start");

        UdpClient udpClient = new UdpClient(12345);
        udpClient.Client.SendTimeout = 5000;
        udpClient.Client.ReceiveTimeout = 5000;

        Debug.Log("touchControl server started");

        byte[] buf = null;
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (!touchControlStopped)
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

            Loom.QueueOnMainThread(() =>
            {
                //onTouch(new Vector3(Screen.width * fx, Screen.height * fy));
                Debug.Log(JsonUtility.ToJson(m));
                if (m.type >= 0 && m.type < 9 && m.time >= 1 && m.time <= 10)
                {
                    float height = m.height > 1 ? 1 : m.height < 0 ? 0 : m.height;
                    if (po[m.type] != null)
                    {
                        po[m.type].ParticleObjectPlay(height, m.time, regions[idx%4].center, regions[idx%4].area);
                        idx++; // po.Length;
                    }
                }
            });
        }
        Debug.Log("touchControl TCP thread end");
    }

    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject go = GameObject.Find("/PS" + i);
            if (go == null)
            {
                continue;
            }
            po[i] = go.GetComponent<IParticleObject>();
        }
        GameObject fountain = GameObject.Find("/Fountain");
        if (fountain != null)
        {
            po[8] = fountain.GetComponent<IParticleObject>();
        }

        touchControlThread = new Thread(touchControl);
        touchControlThread.Start();
    }

    void OnDisable()
    {
        touchControlStopped = true;
        touchControlThread.Interrupt();
        touchControlThread.Join();
    }

    public void ParticleObjectOnEvent(WorldEvent e)
    {
        Debug.Log("play " + idx%3);
        if (po[idx%3] != null)
        {
            po[idx % 3].ParticleObjectPlay(0.5f, 2f, regions[idx%4].center, regions[idx%4].area);
        }
        idx++; // po.Length;
    }
}
