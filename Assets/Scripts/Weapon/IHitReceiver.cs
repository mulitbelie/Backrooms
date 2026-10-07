using UnityEngine;

public interface IHitReceiver
{
    void HandleHit(Collision collision, int damage, GameObject bullet);
}