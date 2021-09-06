using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class gunManager : MonoBehaviour
{
    public Transform bulletShell, bullet, magazine;
    public GameObject bulletShellGO, bulletGO, magazineGO, gadJetBarrel, gadJetScope;

    public Transform setRotation;


    [Header("is the player holding the gun")]
    public bool isHold;
    [Header("is the gun automatic")]
    public bool isAutomatic;
    [Header("is the gun reloading ")]
    public bool isReloading = false;

    [Header("the audio for firing")]
    public AudioClip bulletAudio;
    public AudioSource bulletShotAudio;
    public float volume = 1f;

    [Header("Magazine variable and reload")]
    public int magazineSpace;
    public int ammoInMagazine;
    public float reloadTime;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public float rangeFall;
    public float bulletSpeed;
    public Transform lastBulletPosition;
    public float rangeOffset;

    [Space]
    public float fireRate;
    private float nextTimeToFire = 0f;

    private int contromisura;
    private Coroutine reloadCoroutine = null;
    public enum ResourceTypes { handgun, shotgun, rifle, sniperRifle, machineGun};
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
    public float gunAimPositionXOffset = 0f;
    public float gunAimPositionZOffset = 0f;
    [Space]
    public bool hasScope = false;
    public float gunScopeAimPositionYOffset = 0f;
    public float gunScopeAimPositionXOffset = 0f;
    public float gunScopeAimPositionZOffset = 0f;
    [Space]
    public float zoomOnAim = 0f;

    
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
            if(Input.GetKeyDown(KeyCode.Mouse0) && !isReloading && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                fire();                
            }
            if (Input.GetKey(KeyCode.Mouse0) && isAutomatic && Time.time >= nextTimeToFire && !isReloading)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                fire();              
            }
            if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                if (ammoInMagazine != magazineSpace)
                    reloadCoroutine = StartCoroutine( productionTimeTick(reloadTime));               
            }
        }
    
    }

    /*
     * the method fire plays the sound for the bullet, create the shall of the bullet that get expelled from the side of gun
     * then the method create a raycast to the range before the bullet fall, if doesn't hit anything the method checkHitAtRange
     * is called to start the calculation for the bullet fall 
     */
    public void fire()
    {
        if (ammoInMagazine < 1)
        {
            reloadCoroutine = StartCoroutine(productionTimeTick(reloadTime));
        }
        else
        {
            bulletShotAudio.PlayOneShot(bulletAudio, volume);
            ammoInMagazine -= 1;

            muzzleFlash.Play();

            lastBulletPosition.position = new Vector3(0, 0, 0);

            /*
             * the next three line are used to drop a bullet shell from the side of the gun
             */
            GameObject actualShell = (GameObject)Instantiate(bulletShellGO, bulletShell.position, bulletShell.rotation);
            actualShell.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -10 * Time.deltaTime, ForceMode.Impulse);
            actualShell.gameObject.GetComponent<Rigidbody>().velocity = (transform.up * 2);

            if (resourceTypes != ResourceTypes.shotgun)
                normalFire();
            else
                shotgunFire();

        }
              
    }

    private void normalFire()
    {
        RaycastHit hit;

        if (Physics.Raycast(bullet.transform.position, bullet.transform.TransformDirection(Vector3.forward), out hit, rangeFall))
        {
            GameObject actualBullet = (GameObject)Instantiate(bulletGO, hit.point, bullet.rotation);
            actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y, bullet.rotation.z);

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
            }

            Debug.DrawRay(bullet.transform.position, bullet.transform.TransformDirection(Vector3.forward) * rangeFall, Color.green, 1f, false);

            GameObject impactEffectGO = (GameObject)Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactEffectGO, 2f);
        }
        else
        {
            Debug.DrawRay(bullet.transform.position, bullet.transform.TransformDirection(Vector3.forward) * rangeFall, Color.blue, 10f, false);

            lastBulletPosition.position = bullet.transform.position + bullet.transform.TransformDirection(Vector3.forward) * rangeFall;

            GameObject actualBullet = (GameObject)Instantiate(bulletGO, lastBulletPosition.position, bullet.rotation);
            actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y, bullet.rotation.z);

            checkHitAtRange(lastBulletPosition);
        }
    }
    private void shotgunFire()
    {
        for (int pallets = 0; pallets < 7; pallets++)
        {
            Vector3 offsetBullet = new Vector3(Random.Range(-.2f,.2f), Random.Range(-.3f, .3f) + 0f) ;
            RaycastHit hit;

            if (Physics.Raycast(bullet.transform.position + offsetBullet, bullet.transform.TransformDirection(Vector3.forward), out hit, rangeFall))
            {
                GameObject actualBullet = (GameObject)Instantiate(bulletGO, hit.point, bullet.rotation);
                actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y, bullet.rotation.z);

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
                }

                Debug.DrawRay(bullet.transform.position + offsetBullet, bullet.transform.TransformDirection(Vector3.forward) * rangeFall, Color.green, 1f, false);

                GameObject impactEffectGO = (GameObject)Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactEffectGO, 2f);
            }
            else
            {
                Debug.DrawRay(bullet.transform.position + offsetBullet, bullet.transform.TransformDirection(Vector3.forward) * rangeFall, Color.blue, 10f, false);

                lastBulletPosition.position = bullet.transform.position + bullet.transform.TransformDirection(Vector3.forward) * rangeFall;

                GameObject actualBullet = (GameObject)Instantiate(bulletGO, lastBulletPosition.position, bullet.rotation);
                actualBullet.transform.rotation = Quaternion.Euler(bullet.rotation.x, setRotation.transform.eulerAngles.y, bullet.rotation.z);

                checkHitAtRange(lastBulletPosition);
            }
        }
    }

    IEnumerator productionTimeTick(float second)
    {
        while (true)
        {
            isReloading = true;

            this.gameObject.transform.localRotation = Quaternion.Euler(gunRotationXOffset, gameObject.transform.localRotation.y, gameObject.transform.localRotation.z - 30f);        
            yield return new WaitForSeconds(second);
            this.gameObject.transform.localRotation = Quaternion.Euler(gunRotationXOffset, gameObject.transform.localRotation.y, 0);

            ammoInMagazine = magazineSpace;
            isReloading = false;
          
            GameObject magazzineToDrop = (GameObject)Instantiate(magazineGO, magazine.position, magazine.rotation);
            magazzineToDrop.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -10 * Time.deltaTime, ForceMode.Impulse);

            StopCoroutine(reloadCoroutine);
            yield return null;
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
    
}
