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
}

public class FireworksManager : CylinderVoxManager {

    Thread touchControlThread;

    IParticleObject[] po = new IParticleObject[9];

    int idx = 0;

    bool touchControlStopped = false;

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
                if (m.type >= 0 && m.type < 9)
                {
                    float height = m.height > 1 ? 1 : m.height < 0 ? 0 : m.height;
                    if (po[m.type] != null)
                    {
                        po[m.type].ParticleObjectPlay(height);
                    }
                }
            });
        }
        Debug.Log("touchControl TCP thread end");
    }

    protected override void initWorkers(HashSet<Thread> s)
    {
        base.initWorkers(s);
        //touchControlThread = new Thread(touchControl);
        //s.Add(touchControlThread);
    }

    protected override void initConfig()
    {
        base.initConfig();
        NeedObjectVox = false;
    }

    protected override void initParticleObjects(HashSet<IParticleObject> s)
    {
        base.initParticleObjects(s);
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
    }

    public override void onTouch(Vector3 e)
    {
        base.onTouch(e);
        Debug.Log("play " + idx);
        if (po[idx] != null)
        {
            po[idx].ParticleObjectPlay(1f);
        }
        idx = (idx + 1) % po.Length;
    }

}
