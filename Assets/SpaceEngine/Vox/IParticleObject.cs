using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface UpdateLedHander
{
    void setRealLed(Vector3 position, Color color, float size);
}

public interface IParticleObject
{
    void ParticleObjectPlay(float height, float time, Vector2 center, Vector2 area);
    void ParticleObjectUpdate(UpdateLedHander handler);
}

