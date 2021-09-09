using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class granadeManager : MonoBehaviour
{
    public enum GranadeType { frag, smoke, fire };
    public GranadeType granadeType;

    public Rigidbody rigidbody;
    public float thrust = 10f;

    public int maxForType = 2;

    public bool inPlayerPossesion = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
