using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    // thanks dani
    private float startPos;
    public float length;
    public new Transform camera;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }

    void Update()
    {
        float temp = (camera.position.x * (1 - parallaxEffect));
        float dist = (camera.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length - 1) startPos += length;
    }
}
