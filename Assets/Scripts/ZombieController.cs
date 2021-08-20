using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        target=GameObject.FindObjectOfType<FPController>().gameObject;
    }

    private void Update()
    {
        navMeshAgent.SetDestination(target.transform.position);
        bool distantCheck = navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
        animator.SetBool(Walk, distantCheck?true:false);
        animator.SetBool(Attack, !(distantCheck));



    }
}
