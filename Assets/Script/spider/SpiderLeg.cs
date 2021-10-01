using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    public GameObject body, leg;
    public GameObject bodyLegJoint, leg1Stick, leg1Leg2Joint, leg2Stick;

    public float maxForward, maxBackwards;
    public float offsetYrotation = 0.2f;

    public Vector3 previousPosition;
    public float startYPos = 0;
    // Start is called before the first frame update
    void Start()
    {
        getPositionRoutine = StartCoroutine(getPositionTick());      
        startYPos = bodyLegJoint.transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (previousPosition.x != body.transform.position.x)
        {
            if (previousPosition.x > body.transform.position.x)
            {
                bodyLegJoint.transform.localRotation = Quaternion.Euler(bodyLegJoint.transform.localRotation.eulerAngles.x, bodyLegJoint.transform.localRotation.eulerAngles.y + offsetYrotation, bodyLegJoint.transform.localRotation.eulerAngles.z);
            }
            else if (previousPosition.x < body.transform.position.x)
            {
                bodyLegJoint.transform.localRotation = Quaternion.Euler(bodyLegJoint.transform.localRotation.eulerAngles.x, bodyLegJoint.transform.localRotation.eulerAngles.y - offsetYrotation, bodyLegJoint.transform.localRotation.eulerAngles.z);
            }
            if (bodyLegJoint.transform.localRotation.eulerAngles.y > startYPos+maxForward || bodyLegJoint.transform.localRotation.eulerAngles.y < startYPos - maxBackwards)
            {
                resetLegPosition();
            }
        }
    }

    void resetLegPosition()
    {
       bodyLegJoint.transform.localRotation = Quaternion.Euler(bodyLegJoint.transform.localRotation.eulerAngles.x, startYPos, bodyLegJoint.transform.localRotation.eulerAngles.z);
    }

    public Coroutine getPositionRoutine = null;
    public IEnumerator getPositionTick()
    {
        while (true)
        {
            
            previousPosition = body.transform.position;       
            yield return new WaitForSeconds(.33f);
        }
    }
}
