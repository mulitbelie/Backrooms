using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHand : MonoBehaviour
{
    private Zombie zombie;

    public int damage => zombie != null ? zombie.zombieDamage : 0;

    private void Start()
    {
        zombie = GetComponentInParent<Zombie>();
    }
}