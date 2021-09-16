using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectBuilding : MonoBehaviour
{
    public Material invisibleMaterial, toBuildMaterial;
    public GameObject building;
   
    public bool isBuilt = false;
    public bool inRange = false;

    public float buildTimeNeeded = 5f;
    public float buildingTime = 0f;

    Coroutine buildCoroutine = null;

    [Space]
    public bool hasParentObject = false;
    public GameObject[] parentNedded;

    private void Start()
    {
        GetComponent<MeshRenderer>().material = invisibleMaterial;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && inRange)
        {
            if(!hasParentObject)
            build();
            else if (areAllParentBuild())
            {
                build();
            }
        }
        if (Input.GetKeyUp(KeyCode.B) && inRange)
        {
            if (!isBuilt)
            {
                StopCoroutine(buildTick());
                buildingTime = 0;
            }

        }
    }

    public bool areAllParentBuild()
    {
        foreach(GameObject g in parentNedded)
        {
            if (!g.GetComponent<objectBuilding>().isBuilt)
                return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<playerController>().hasHammer && !isBuilt)
        {
            if (areAllParentBuild())
            {
                GetComponent<MeshRenderer>().material = toBuildMaterial;
                inRange = true;
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<playerController>().hasHammer && !isBuilt)
        {
            if (areAllParentBuild())
            {
                GetComponent<MeshRenderer>().material = toBuildMaterial;
                inRange = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<playerController>().hasHammer)
        {
            GetComponent<MeshRenderer>().material = invisibleMaterial;
            inRange = false;
        }
    }

    public void build()
    {
        buildCoroutine = StartCoroutine(buildTick());
    }

    public IEnumerator buildTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            buildingTime++;
            if(buildingTime <= buildTimeNeeded && !isBuilt)
            {
                isBuilt = true;
                Instantiate(building, transform.position, transform.rotation);
                GetComponent<MeshRenderer>().material = invisibleMaterial;
                StopCoroutine(buildCoroutine);
            }
        }
    }

}
