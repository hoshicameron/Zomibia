using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEventTrigger : MonoBehaviour
{
    public void TriggerAttack()
    {
        GetComponentInParent<ZombieController>().HitPlayer();
    }
}
