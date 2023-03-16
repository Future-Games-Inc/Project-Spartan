using UnityEngine;

internal class Vector3Influence : ParticleSystemForceField
{
    private Vector3 forceDirection;

    public Vector3Influence(Vector3 forceDirection)
    {
        this.forceDirection = forceDirection;
    }
}