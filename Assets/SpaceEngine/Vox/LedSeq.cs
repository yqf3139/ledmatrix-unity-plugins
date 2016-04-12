using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class LedSeq
{
    protected const uint version = 0x02;
    protected enum Shape : int { Cube, Cylinder };
    protected enum HeaderItem : int { Version, FrameTotolCount, Type, Fps, Shape };

    protected const int HEADERNUM = 10;
    protected const int PARAM1NUM = 10;
    protected const int PARAM2NUM = 10;

    protected Stream sw;
    protected uint[] header = new uint[HEADERNUM];
    protected uint[] param1 = new uint[PARAM1NUM];
    protected float[] param2 = new float[PARAM2NUM];
    public uint[] leddata = null;
    public uint[][] framedata;

    public int ledsPerFrame;
    public int bytesPerFrame;
    protected int playIdx = 0, saveIdx = 0;
    protected bool dirty;
    protected int frameCounter, fps;

    public abstract void open(string path, uint frames);

    public virtual void open(string path, uint frames, uint shape)
    {
        header[(int)HeaderItem.Version] = version;
        header[(int)HeaderItem.FrameTotolCount] = frames;
        header[(int)HeaderItem.Type] = 1;
        header[(int)HeaderItem.Fps] = 30;
        header[(int)HeaderItem.Shape] = shape;

        frameCounter = (int)frames;

        // write the headers and params
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        sw = File.Create(path);

        byte[] res = null;
        res = new byte[header.Length * sizeof(uint)];
        Buffer.BlockCopy(header, 0, res, 0, res.Length);
        sw.Write(res, 0, res.Length);

        res = new byte[param1.Length * sizeof(uint)];
        Buffer.BlockCopy(param1, 0, res, 0, res.Length);
        sw.Write(res, 0, res.Length);

        res = new byte[param2.Length * sizeof(float)];
        Buffer.BlockCopy(param2, 0, res, 0, res.Length);
        sw.Write(res, 0, res.Length);
    }

    public void clear()
    {
        dirty = false;
        int len = leddata.Length;
        for (int i = 0; i < len; i++)
        {
            leddata[i] = 0;
        }
    }

    public void save()
    {
        if (!dirty || saveIdx == frameCounter)
        {
            return;
        }
        Debug.Log(saveIdx+ " len "+ leddata.Length);

        byte[] raw = new byte[leddata.Length * sizeof(uint)];
        Buffer.BlockCopy(leddata, 0, raw, 0, raw.Length);
        save(raw);
        saveIdx++;
    }

    public void save(byte[] raw)
    {
        sw.Write(raw, 0, raw.Length);
    }

    public void close()
    {
        sw.Close();
        sw.Dispose();
    }

    public static LedSeq load(string path)
    {
        // read header and params
        // open the archive file
        if (!File.Exists(path))
        {
            Debug.LogError("the file do not exist");
            return null;
        }

        // get the headers and valid header
        // create the corresponding ledseq
        Stream sw = File.OpenRead(path);
        LedSeq ledseq = null;
        if ((ledseq = validHeader(sw)) != null)
        {
            // input all the data and fill ledseq
            ledseq.restoreParams();
            ledseq.restoreFrameBuffer(sw);
        }

        sw.Close();
        sw.Dispose();
        return ledseq;
    }

    void restoreFrameBuffer(Stream sw)
    {
        byte[] framebuf = new byte[bytesPerFrame];

        framedata = new uint[frameCounter][];

        int len = 0;
        for (int i = 0; i < frameCounter; i++)
        {
            len = sw.Read(framebuf, 0, framebuf.Length);
            if (len != framebuf.Length)
            {
                Debug.Log(i + " len not match " + len + " " + framebuf.Length);
            }
            framedata[i] = new uint[ledsPerFrame];
            Buffer.BlockCopy(framebuf, 0, framedata[i], 0, framebuf.Length);
        }
    }

    static LedSeq validHeader(Stream sw)
    {
        uint[] header = new uint[HEADERNUM];
        byte[] buf = new byte[header.Length * sizeof(uint)];
        sw.Read(buf, 0, buf.Length);
        Buffer.BlockCopy(buf, 0, header, 0, buf.Length);

        if (header[(int)HeaderItem.Version] != version)
        {
            Debug.LogError("the ledseq version mismatch " + header[(int)HeaderItem.Version]);
            return null;
        }

        LedSeq ledseq = null;
        switch (header[(int)HeaderItem.Shape])
        {
            case (uint)Shape.Cube:
                ledseq = new CubeLedSeq();
                break;
            case (uint)Shape.Cylinder:
                ledseq = new CylinderLeqSeq();
                break;
            default:
                return null;
        }
        Buffer.BlockCopy(header, 0, ledseq.header, 0, header.Length * sizeof(uint));

        buf = new byte[ledseq.param1.Length * sizeof(uint)];
        sw.Read(buf, 0, buf.Length);
        Buffer.BlockCopy(buf, 0, ledseq.param1, 0, buf.Length);

        buf = new byte[ledseq.param2.Length * sizeof(float)];
        sw.Read(buf, 0, buf.Length);
        Buffer.BlockCopy(buf, 0, ledseq.param2, 0, buf.Length);

        return ledseq;
    }

    public abstract void restoreParams();

    public static void startServer()
    {

    }

    public static void stopServer()
    {

    }

    public abstract int getLedIdx(int i, int j, int k);

    public void setRealLed(int i, int j, int k, uint d)
    {
        dirty = true;
        int idx = getLedIdx(i, j, k);
        if (idx < 0)
        {
            return;
        }

        leddata[idx] |= d;
    }

    public void play()
    {
        if (framedata == null)
        {
            return;
        }
        playIdx++;
        if (playIdx == frameCounter)
        {
            playIdx = 0;
        }
        leddata = framedata[playIdx];
    }
}

