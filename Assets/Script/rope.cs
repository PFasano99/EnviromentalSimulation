using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope : MonoBehaviour
{
    public Point[] points;
    public Stick[] sticks;

    public float gravity;

    public int numIterations;

    public int ropeLenght;
  
    // Update is called once per frame
    void FixedUpdate()
    {
        Simulate();
    }

    void Simulate()
    {
        foreach (Point p in points)
        {
            if (!p.locked)
            {
                Vector2 positionBeforeUpdate = p.position;
                p.position += p.position - p.prevPosition;
                p.position += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
                p.prevPosition = positionBeforeUpdate;
            }
        }

        foreach (Stick stick in sticks)
        {
            for (int i = 0; i < numIterations; i++)
            {
                Vector2 stickCentre = (stick.pointA.position + stick.pointB.position) / 2;
                Vector2 stickDir = (stick.pointA.position - stick.pointB.position).normalized;
                if (!stick.pointA.locked)
                    stick.pointA.position = stickCentre + stickDir * stick.lenght / 2;
                if (!stick.pointB.locked)
                    stick.pointB.position = stickCentre - stickDir * stick.lenght / 2;
            }
            
        }
    }
    
}

[System.Serializable]
public class Point
{
    public Vector2 position, prevPosition;
    public bool locked;
    
}
[System.Serializable]
public class Stick
{
    public Point pointA, pointB;
    public float lenght;    
}
