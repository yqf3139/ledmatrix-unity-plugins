using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum MeshObjectUpdateStatus { IN, INTERSECT, OUT };

public interface IMeshEventListener : IInteractionInput
{
    void MeshObjectUpdate(MeshObjectUpdateStatus s);
}

public interface IParticleEventListener : IInteractionInput
{

}
