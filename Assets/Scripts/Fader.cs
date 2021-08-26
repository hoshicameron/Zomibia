using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] private float reductionAmount  = 0.03f;
    private RawImage image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        var imageColor = image.color;

        imageColor.a -= reductionAmount;
        imageColor.a = Mathf.Clamp01(imageColor.a);
        image.color = imageColor;

        if (Math.Abs(image.color.a) < Mathf.Epsilon)
        {
            Destroy(gameObject);
        }
    }
}
