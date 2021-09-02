using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ammoMenager : MonoBehaviour
{

    public float min, max;
    public int quantity;

    // Start is called before the first frame update
    void Start()
    {
        quantity = (int) Random.Range(min, max);
    }
 
}
