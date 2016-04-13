using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System;

public class BubbleDemo : MonoBehaviour {

    const string phoneip = "192.168.0.135";

    Thread sonicControlThread;
    NetworkStream sonicControlStream;
    float sonicRadius = 4f;

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
                sonicRadius = 2f + ((float)radius) * 6f;
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

    // Use this for initialization
    void Start () {
        sonicControlThread = new Thread(sonicControl);
        sonicControlThread.Start();
    }

    // Update is called once per frame
    void Update () {
        if (sonicRadius == transform.localScale.x)
        {
            return;
        }
        transform.localScale = new Vector3(sonicRadius, sonicRadius, sonicRadius);
    }

    void OnDisable()
    {
        if (sonicControlStream != null)
        {
            sonicControlStream.Close();
        }
        if (sonicControlThread != null)
        {
            sonicControlThread.Interrupt();
            sonicControlThread.Join();
        }
    }
}
