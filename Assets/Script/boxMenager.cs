using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxMenager : MonoBehaviour
{
    public Color mycolor;
    public int r, g, b;
    // Start is called before the first frame update
    void Start()
    {
        mycolor = new Color(r, g, b);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("bullet"))
        {
            gameObject.GetComponent<Renderer>().material.color = mycolor;

            r += 2 + b;
            g += 2 + r;
            b += 2 + g;

            if (r >= 256)
            {
                r = 0;
            }
            if (g >= 256)
            {
                g = 0;
            }
            if (b >= 256)
            {
                b = 0;
            }

            mycolor = new Color(r, g, b);
        }
        
    }
}
