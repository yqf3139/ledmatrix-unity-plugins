using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

class CylinderBigLedBridge : LedMatrix
{
    const string BRIDGEDLLNAME = "bridgedll";

    [DllImport(BRIDGEDLLNAME)]
    protected static unsafe extern uint* BridgeGetLedBuffer();

    [DllImport(BRIDGEDLLNAME)]
    protected static unsafe extern void BridgeSend();

    [DllImport(BRIDGEDLLNAME)]
    protected static unsafe extern void BridgeHandshake();

    [DllImport(BRIDGEDLLNAME)]
    protected static unsafe extern void BridgeConstruct(
        int _floorCounter, int _roundsCounter, float _step, 
        float _distance, float _height, float _pillar, int _len);

    [DllImport(BRIDGEDLLNAME)]
    protected static unsafe extern void BridgeDeConstruct();

    bool init = true;
    unsafe uint* buffer = BridgeGetLedBuffer();

    public CylinderBigLedBridge(CylinderLeqSeq ledseq)
        : base(ledseq)
    {
        BridgeConstruct(ledseq.floorCounter, ledseq.roundsCounter, ledseq.step, 
            ledseq.distance, ledseq.height, ledseq.pillar, ledseq.ledsPerFrame);
    }

    public override void clearLed()
    {

    }

    public override void showLed()
    {
        // fill in the buffer
        unsafe
        {
            for (int i = 0; i < TOTAL; i++)
            {
                buffer[i] = ledseq.leddata[i];
            }
        }

        if (init)
        {
            BridgeHandshake();
            init = false;
        }
        BridgeSend();
    }

    public override void dispose()
    {
        BridgeDeConstruct();
    }

    public override Bounds getLedBound()
    {
        throw new NotImplementedException();
    }
}
