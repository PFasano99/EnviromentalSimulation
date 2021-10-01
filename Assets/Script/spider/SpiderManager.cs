using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderManager : MonoBehaviour
{
    public GameObject target;
    public GameObject secondTarget;

    public bool hasReachedTarget = false, hasReachedsecondTarget = true;

    public float speed;

    public float offsetRotationX, offsetRotationY, offsetRotationZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveBetween(target, secondTarget);
    }

    void moveBetween(GameObject target1, GameObject target2)
    {
        if (secondTarget != null)
        {
            if (transform.position == target2.transform.position)
            {
                hasReachedsecondTarget = true;
                hasReachedTarget = false;
                //transform.LookAt(target1.transform, Vector3.left);
                transform.rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x - offsetRotationX, transform.localRotation.eulerAngles.y - offsetRotationY, transform.localRotation.eulerAngles.z - offsetRotationZ);
            }
            else if (transform.position == target1.transform.position)
            {
                hasReachedTarget = true;
                hasReachedsecondTarget = false;
                //transform.LookAt(target2.transform, Vector3.zero);
                transform.rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x + offsetRotationX, transform.localRotation.eulerAngles.y + offsetRotationY, transform.localRotation.eulerAngles.z + offsetRotationZ);
            }

            if (hasReachedsecondTarget)
            {
                transform.position = Vector3.MoveTowards(transform.position, target1.transform.position, speed * Time.deltaTime);
            }
            else if (hasReachedTarget)
            {
                transform.position = Vector3.MoveTowards(transform.position, target2.transform.position, speed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target1.transform.position, speed * Time.deltaTime);
        }
    }

}
