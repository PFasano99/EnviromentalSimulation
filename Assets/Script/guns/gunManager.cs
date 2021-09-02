using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class gunManager : MonoBehaviour
{
    public Transform nozle, gadjet1, gadjet2, gadjet3, gadjet4, bulletShell, bullet;
    public GameObject nozleGO, gadjet1GO, gadjet2GO, gadjet3GO, gadjet4GO, bulletShellGO, bulletGO;

    public Transform setRotation;

    public bool hasNozle = false;
    public bool hasGadjet1 = false;
    public bool hasGadjet2 = false;
    public bool hasGadjet3 = false;
    public bool hasGadjet4 = false;

    [Header("is the player holding the gun")]
    public bool isHold;
    [Header("is the gun automatic")]
    public bool isAutomatic;
    [Header("is the gun firing ")]
    public bool isFiring = false;

    public AudioClip bulletAudio;
    public AudioSource bulletShotAudio;
    public float volume = 1f;

    public int magazineSpace;
    public int ammoInMagazine;

    public Coroutine reloadCoroutine = null;

    public float rangeFall;
    public float bulletSpeed;
    public Transform lastBulletPosition;
    public float rangeOffset;
    public int contromisura;



    public float fireRate;
    public Coroutine fireRateCoroutine = null;

    public enum ResourceTypes { handgun, shotgun, rifle};
    public ResourceTypes resourceTypes;

    [Space]
    [Header("the offset the gun has when picked up on the XYZ Rotation")]
    public float gunRotationYOffset = 0f;
    public float gunRotationXOffset = 0f;
    public float gunRotationZOffset = 0f;
    [Space]
    [Header("the offset the gun has when picked up on the XYZ Position")]
    public float gunPositionYOffset = 0f;
    public float gunPositionXOffset = 0f;
    public float gunPositionZOffset = 0f;
    [Space]
    [Header("the offset the gun has when picked up and Aiming on the XYZ Rotation")]
    public float gunAimPositionYOffset = 0f;
    [Space]
    public float zoomOnAim = 0f;

    public float reloadTime;
    // Start is called before the first frame update
    void Start()
    {
        //lastBulletPosition.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isHold)
        {
            
            if(Input.GetKeyDown(KeyCode.Mouse0) && ammoInMagazine > 0)
            {                
                fire();                
            }
            if (Input.GetKey(KeyCode.Mouse0) && ammoInMagazine > 0 && !isFiring)
            { 
                if (!isAutomatic)
                    fire();
               // else automaticFire();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                reloadCoroutine = StartCoroutine( productionTimeTick(reloadTime));               
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isFiring = false;
                if (fireRateCoroutine != null)
                    StopCoroutine(fireRateCoroutine);
            }
        }


        if(hasGadjet1)
        {
           // useGadjet(gadjet1GO, gadjet1);
        }

       
    }

    /*
     * the method fire plays the sound for the bullet, create the shall of the bullet that get expelled from the side of gun
     * then the method create a raycast to the range before the bullet fall, if doesn't hit anything the method checkHitAtRange
     * is called to start the calculation for the bullet fall 
     */
    public void fire()
    {
        bulletShotAudio.PlayOneShot(bulletAudio, volume);
        ammoInMagazine -= 1;

        lastBulletPosition.position = new Vector3(0,0,0);
        

        GameObject actualShell = (GameObject)Instantiate(bulletShellGO, bulletShell.position, bulletShell.rotation);
        actualShell.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -10 * Time.deltaTime, ForceMode.Impulse);
        actualShell.gameObject.GetComponent<Rigidbody>().velocity = (transform.up * 2);

        RaycastHit hit;
       
        if (Physics.Raycast(bullet.transform.position, bullet.transform.TransformDirection(Vector3.forward), out hit, rangeFall))
        {                                         
            GameObject actualBullet = (GameObject)Instantiate(bulletGO, hit.point, bullet.rotation);
            actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y, bullet.rotation.z);

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
            }          

            Debug.DrawRay(bullet.transform.position, bullet.transform.TransformDirection(Vector3.forward) * rangeFall, Color.green, 1f, false);
        }
        else
        {
            Debug.DrawRay(bullet.transform.position, bullet.transform.TransformDirection(Vector3.forward) * rangeFall, Color.blue, 10f, false);       
                             
            lastBulletPosition.position = bullet.transform.position + bullet.transform.TransformDirection(Vector3.forward) * rangeFall;

            GameObject actualBullet = (GameObject)Instantiate(bulletGO, lastBulletPosition.position, bullet.rotation);
            actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y, bullet.rotation.z);

            //lastBulletPosition.transform.rotation = Quaternion.Euler(gunRotationXOffset, gunRotationYOffset, gunRotationZOffset);
            checkHitAtRange(lastBulletPosition);
        }

        if(ammoInMagazine <= 0)
            reloadCoroutine = StartCoroutine(productionTimeTick(reloadTime));
    }

    private void useGadjet(GameObject gadjet, Transform gadjetPosition)
    {
        gadjet.transform.position = gadjetPosition.position;
       // gadjet.transform.rotation = Quaternion.Euler(-90, -90, 180);
        gadjet.transform.parent = gameObject.transform;
        
        
    }


    IEnumerator productionTimeTick(float second)
    {
        while (true)
        {
           isFiring = false;
           yield return new WaitForSeconds(second);
           ammoInMagazine = magazineSpace; 
           StopCoroutine(reloadCoroutine);
        }
    }

    IEnumerator fireRateTimeTick(float second)
    {
        while (true)
        {
            isFiring = true;
            if(ammoInMagazine > 0)
                fire();  
            else
                reloadCoroutine = StartCoroutine(productionTimeTick(reloadTime));
            yield return new WaitForSeconds(second);           
        }
    }

    /*
     * the method checkHitAtRange create raycasts from the point the bullet starts to fall down, lowering the transform position for which the raycast is created
     * for a max of 500 times, an arbitrary number to stop if from looping
     * 
     */
    private void checkHitAtRange(Transform positionCheck)
    {
        
        RaycastHit hit;
        do
        {
            positionCheck.position += (positionCheck.transform.TransformDirection(Vector3.down * 0.05f + Vector3.forward * rangeOffset));
            if (Physics.Raycast(positionCheck.position, positionCheck.transform.TransformDirection(Vector3.forward), out hit, rangeOffset))
            {
                Debug.DrawRay(positionCheck.position, positionCheck.transform.TransformDirection(Vector3.forward) * rangeOffset, Color.magenta, 10f, false);
                GameObject actualBullet = (GameObject)Instantiate(bulletGO, hit.point, bullet.rotation);
                actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y , bullet.rotation.z);
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
                }

                contromisura = 500;
            }
            else
            {
                
                Debug.DrawRay(positionCheck.transform.position, positionCheck.transform.TransformDirection(Vector3.forward) * rangeOffset, Color.yellow, 10f, false);

            }
            contromisura++;
        } while (contromisura <= 500);

        lastBulletPosition.position = new Vector3(0, 0, 0);
        contromisura = 0;
             
    }
    
    private void automaticFire()
    {
        fireRateCoroutine = StartCoroutine(fireRateTimeTick(fireRate));
    }
}
