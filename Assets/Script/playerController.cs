﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 12.0f;
    public float gravity = -9.81f;
    
    public float jumpHaight = 3.0f;

    public CharacterController controller;

    Vector3 velocity;

    public bool isGround;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;


    public Transform gunHold;
    public Transform gun;

    public bool isRightHandFull = false;
    public bool isAiming = false;

    private itemsMenager item;
    private gunManager gunMenager;
    private ammoMenager ammoMenager;

    private bulletQuantity bulletQuantity = new bulletQuantity();
   
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    

        if(isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(Input.GetKey(KeyCode.Mouse1))
        {
           
            aim();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            gunHold.transform.localPosition = new Vector3(0.061f, -0.09f, 0.9f);
            if (isRightHandFull)
                if (item.GetComponent<gunManager>())
                {
                    item.transform.position = new Vector3(gunHold.position.x + item.GetComponent<gunManager>().gunPositionXOffset, gunHold.position.y + item.GetComponent<gunManager>().gunPositionYOffset, gunHold.position.z + item.GetComponent<gunManager>().gunPositionZOffset);
                    Camera.main.fieldOfView = 60;
                    isAiming = false;
                }
                else item.transform.position = gunHold.position;
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHaight * -2f * gravity);
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(!isRightHandFull)
            pickUp();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            pickGunGadjet();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            dropItem();
        }
    }

    /*
     * the method pickUp creates a raycast thati if hits a GO in the items layer and the hand is not full will place the GO in the holding position
     */
    private void pickUp()
    {    
            Debug.DrawRay(Camera.main.transform.position, transform.TransformDirection(Vector3.forward) * 30f, Color.magenta, 10f, false);
            RaycastHit hit;
            
            if (Physics.Raycast(Camera.main.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, LayerMask.GetMask("items")))
            {
                Debug.DrawRay(Camera.main.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow, 10f, false);

                if (hit.collider != null)
                {            
                      //  Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                        GameObject hold = hit.collider.gameObject;

                    if (hit.collider.gameObject.CompareTag("Ammo"))
                    {
                        ammoMenager = hold.GetComponent<ammoMenager>();
                        bulletQuantity.setGlockAmmo(bulletQuantity.getGlockAmmo() + ammoMenager.quantity);
                        Destroy(ammoMenager.gameObject);
                    }
                    else
                    {
                        item = hold.GetComponent<itemsMenager>();

                        isRightHandFull = true;
                        if (hold.GetComponent<gunManager>())
                            item.transform.position = new Vector3 (gunHold.position.x + hold.GetComponent<gunManager>().gunPositionXOffset, gunHold.position.y + hold.GetComponent<gunManager>().gunPositionYOffset,  gunHold.position.z + hold.GetComponent<gunManager>().gunPositionZOffset);
                        item.transform.parent = gameObject.transform;
                        if(hold.GetComponent<gunManager>())
                            item.transform.localEulerAngles = new Vector3(hold.GetComponent<gunManager>().gunRotationXOffset, hold.GetComponent<gunManager>().gunRotationYOffset, 0f);
                       
                        if (hit.collider.gameObject.CompareTag("gun") )
                        {
                            gunMenager = item.GetComponent<gunManager>();
                            gunMenager.isHold = true;
                        }

                    }

                }
            
            }

             
    }

    private void dropItem()
    {
        if(item.gameObject != null)
        {
            item.transform.parent = null;
            isRightHandFull = false;
            item.GetComponent<gunManager>().isHold = false;
        }
    }
    private void aim()
    {
        gunHold.transform.localPosition = new Vector3(0f, -0.08f, 0.8f);
        if(isRightHandFull)
            if (item.GetComponent<gunManager>())
            {
                item.transform.position = new Vector3(gunHold.position.x, gunHold.position.y + item.GetComponent<gunManager>().gunAimPositionYOffset, gunHold.position.z);
                if(!isAiming)
                Camera.main.fieldOfView += item.GetComponent<gunManager>().zoomOnAim;
                isAiming = true;
            }
            else item.transform.position = gunHold.position;
    }

    private void pickGunGadjet()
    {
        if (gunMenager.isHold)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, LayerMask.GetMask("items")))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                Debug.Log("here 2");

                if (hit.collider != null)
                {
                    Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                    GameObject hold = hit.collider.gameObject;

                    if (hit.collider.gameObject.CompareTag("scope"))
                    {
                        
                        hold.transform.position = gunMenager.gadjet1.position;
                        hold.transform.parent = gunMenager.gameObject.transform;
                        hold.transform.localEulerAngles = new Vector3(-90, -90, 180);
                        

                        hold.GetComponent<Rigidbody>().isKinematic = true;
                        hold.GetComponent<Collider>().enabled = false;

                        gunMenager.gadjet1GO = hold;
                        gunMenager.hasGadjet1 = true;

                    }

                }
            }
        }
    }
   
}
