using UnityEngine;
using System.Collections;
using MKGlowSystem;

public class GlowConfig : MonoBehaviour {

    private MKGlow mkGlow;

    void Awake()
    {
        mkGlow = this.GetComponent<MKGlow>();
        InitGlowSystem();
        Debug.Log(mkGlow);
    }

    private void InitGlowSystem()
    {
        mkGlow.BlurIterations = 5;
        mkGlow.BlurOffset = 0.1f;
        mkGlow.Samples = 2;
        mkGlow.GlowIntensity = 0.7f;
        mkGlow.BlurSpread = 0.2f;

        mkGlow.GlowType = MKGlowType.Selective;
        mkGlow.GlowQuality = MKGlowQuality.High;
    }
}
