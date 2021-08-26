using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ZombieController : MonoBehaviour
{

    enum State
    {
        IDLE,
        WANDER,
        ATTACK,
        CHASE,
        DEAD
    };

    private State state=State.IDLE;
    [Header("Properties")]
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject ragDoll;
    [SerializeField] private float attackThreshold = 1.0f;
    [SerializeField] private int hitPower=5;

    [Header("Audio")]
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip zombieSound;
    [SerializeField] private AudioClip moanClip;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private AudioSource attackAudioSource;
    private AudioSource soundAudioSource;
    private Health health;

    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Death = Animator.StringToHash("death");


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        health = GetComponent<Health>();
        health.OnDied+= Health_OnDied;

        soundAudioSource=gameObject.AddComponent<AudioSource>() as AudioSource;
        attackAudioSource=gameObject.AddComponent<AudioSource>() as AudioSource;

        soundAudioSource.clip = zombieSound;
        soundAudioSource.loop = true;
        soundAudioSource.spatialBlend = 1f;
        soundAudioSource.maxDistance = 20f;
        soundAudioSource.rolloffMode = AudioRolloffMode.Linear;
        soundAudioSource.Play();
    }

    private void Health_OnDied(object sender, EventArgs e)
    {
        if (Random.Range(0, 10) < 5)
        {
            RagdollDeath();
        } else
        {
            AnimatedDeath();
        }
    }

    private void Start()
    {
        if(!GameStats.gameOver)
        target=GameObject.FindObjectOfType<FPController>().gameObject;
    }

    private void Update()
    {
        FiniteStateMachine();
    }

    public void AnimatedDeath()
    {
        TurnOffTriggers();
        animator.SetTrigger(Death);
        soundAudioSource.Stop();

        soundAudioSource.clip = moanClip;
        soundAudioSource.loop = false;
        soundAudioSource.Play();
        state = State.DEAD;
    }

    public void RagdollDeath()
    {
        GameObject rag = Instantiate(ragDoll, transform.position, transform.rotation) as GameObject;
        GameObject hips = FindInChildren(rag, "Hips");
        if (hips != null)
        {
            hips.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
            Destroy(gameObject);
        }
    }

    public static Transform FindInChildren(Transform transform, string name) {
        if(transform == null) return null;
        int count = transform.childCount;
        for(int i = 0; i < count; i++) {
            Transform child = transform.GetChild(i);
            if(child.name == name) return child;
            Transform subChild = FindInChildren(child, name);
            if(subChild != null) return subChild;
        }
        return null;
    }

    public static GameObject FindInChildren(GameObject gameObject, string name) {
        if(gameObject == null) return null;
        Transform transform = gameObject.transform;
        Transform child = FindInChildren(transform, name);
        return child != null ? child.gameObject : null;
    }

    private void TurnOffTriggers()
    {
        animator.SetBool(Walk, false);
        animator.SetBool(Attack, false);
    }


    private void FiniteStateMachine()
    {
        switch (state)
        {
            case State.IDLE:
                if (CanSeePlayer())
                    state = State.CHASE;
                else if(Random.Range(0,5000)<500)
                    state = State.WANDER;
                break;
            case State.WANDER:

                if (!navMeshAgent.hasPath)
                {
                    float newX = transform.position.x + Random.Range(-5, 5);
                    float newZ = transform.position.z + Random.Range(-5, 5);
                    float newY = transform.position.y;
                    Vector3 dest=new Vector3(newX,newY,newZ);
                    navMeshAgent.SetDestination(dest);
                    navMeshAgent.stoppingDistance = 0;
                    TurnOffTriggers();
                    animator.SetBool(Walk,true);

                }
                if (CanSeePlayer())
                    state = State.CHASE;
                else if (Random.Range(0, 5000) < 5)
                {
                    TurnOffTriggers();
                    navMeshAgent.ResetPath();
                    state = State.IDLE;
                }
                break;
            case State.ATTACK:
                if (GameStats.gameOver)
                {
                    TurnOffTriggers();
                    state = State.WANDER;
                }
                TurnOffTriggers();
                animator.SetBool(Attack,true);
                this.transform.LookAt(target.transform.position);
                if (DistanceToPlayer() > navMeshAgent.stoppingDistance + attackThreshold)
                {
                    state = State.CHASE;
                }
                break;
            case State.CHASE:
                if (GameStats.gameOver)
                {
                    TurnOffTriggers();
                    state = State.WANDER;
                }
                if(!navMeshAgent.isOnNavMesh)    return;
                navMeshAgent.SetDestination(target.transform.position);
                navMeshAgent.stoppingDistance = 2f;
                TurnOffTriggers();
                animator.SetBool(Walk,true);

                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
                {
                    state = State.ATTACK;
                }

                if (ForgetPlayer())
                {
                    state = State.WANDER;
                    navMeshAgent.ResetPath();

                }
                break;
            case State.DEAD:
                Destroy(navMeshAgent);
                GetComponent<Sink>().StartSink();


                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool CanSeePlayer()
    {
        if (DistanceToPlayer() < 10)
            return true;
        return false;
    }

    private float DistanceToPlayer()
    {
        if (GameStats.gameOver)
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(target.transform.position, transform.position);
    }

    private bool ForgetPlayer()
    {
        if (DistanceToPlayer() >20)
            return true;
        return false;
    }

    public void HitPlayer()
    {
        if(GameStats.gameOver)    return;

        target.GetComponent<Health>().GetDamage(hitPower);
        attackAudioSource.clip = hitClips[Random.Range(0, hitClips.Length)];
        attackAudioSource.Play();
    }


}
