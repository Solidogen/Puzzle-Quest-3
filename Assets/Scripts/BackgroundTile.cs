using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public GameObject[] dots;

    void Start()
    {
        Setup();
    }

    void Update()
    {
        
    }

    void Setup()
    {
        int dotToUse = Random.Range(0, dots.Length);
        GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity);
        dot.transform.parent = transform;
        dot.name = gameObject.name;
    }
}
