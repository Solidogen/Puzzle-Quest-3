using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var asd = "ASD:";
        var random = new Random().Next();
        var iss = random.Equals(asd);
        var sd = this.name.ToString();
    }
}
