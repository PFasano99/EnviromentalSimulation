using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gadJet : MonoBehaviour
{
    public enum GadjetType { scope, longRangeScope, flashLight, magazine};
    public GadjetType gadjetType;

    public bool isHold;
   
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<gunManager>().isHold)
        {
            useGadjet();
        }
    }

    void useGadjet()
    {
        if (GadjetType.flashLight == gadjetType)
        {
            GameObject light = gameObject.transform.GetChild(0).gameObject;
            light.SetActive(!light.activeSelf);
        }
    }
}
