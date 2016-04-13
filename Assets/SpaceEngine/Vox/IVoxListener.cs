using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum MeshObjectUpdateStatus { IN, INTERSECT, OUT };

public interface IMeshEventListener
{
    void MeshObjectUpdate(MeshObjectUpdateStatus s);
    void MeshObjectOnEvent(WorldEvent e);
}

public interface IParticleEventListener
{
    void ParticleObjectOnEvent(WorldEvent e);
}
