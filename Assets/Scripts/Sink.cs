using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    [SerializeField] private float sinkingDelay=10f;
    private float destroyHeight;

    private void Start()
    {
        if (gameObject.CompareTag("Ragdoll"))
        {
            Invoke(nameof (StartSink),5f);
        }
    }

    public void StartSink()
    {
        destroyHeight = transform.position.y - 5f;
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        InvokeRepeating(nameof(SinkIntoGround),sinkingDelay,0.2f);

    }

    private void SinkIntoGround()
    {
        transform.Translate(0,-0.001f,0);
        if (this.transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }
}
