using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int count;
    [SerializeField] private float spawnRadius;

    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
            {
                GameObject zombie=Instantiate(zombiePrefab,hit.position,Quaternion.identity);
                //zombie.transform.rotation=Quaternion.identity;
                zombie.GetComponent<NavMeshAgent>().Warp(hit.position);
            } else
            {
                i--;
            }


        }
    }


}
