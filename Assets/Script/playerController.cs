using System.Collections;
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
    public Transform granadeHold;

    public bool isRightHandFull = false;
    public bool isAiming = false;

    private itemsMenager item;
    private gunManager gunMenager;
    private ammoMenager ammoMenager;
    private gadJet gadJet1 = null, gadJet2 = null;

    private bulletQuantity bulletQuantity = new bulletQuantity();
    private granadeManager granadeManager = null;
    public GameObject[] granadeArray;
    [Space]
    public int numberOfGranade;


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


        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (item != null)
                aim();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            gunHold.transform.localPosition = new Vector3(0.3f, -0.3f, 0.7f);

            if (item != null)
            {
                item.transform.localPosition = new Vector3(0f, 0f, 0f);

            }
            if (isRightHandFull)
                if (item.GetComponent<gunManager>())
                {
                    item.transform.localPosition = new Vector3(item.GetComponent<gunManager>().gunPositionXOffset, item.GetComponent<gunManager>().gunPositionYOffset, item.GetComponent<gunManager>().gunPositionZOffset);
                    Camera.main.fieldOfView = 60;
                    isAiming = false;
                }
            //else item.transform.position = gunHold.position;
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHaight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            pickUp();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            throwGranade();
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
                else if (hit.collider.gameObject.CompareTag("gun") && !isRightHandFull)
                {
                    item = hold.GetComponent<itemsMenager>();
                    item.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    item.transform.parent = gunHold.gameObject.transform;
                    item.transform.localPosition = new Vector3(0f, 0f, 0f);
                    item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);


                    isRightHandFull = true;
                    if (hold.GetComponent<gunManager>())
                        item.transform.localPosition = new Vector3(0f + hold.GetComponent<gunManager>().gunPositionXOffset, 0f + hold.GetComponent<gunManager>().gunPositionYOffset, 0f + hold.GetComponent<gunManager>().gunPositionZOffset);

                    if (hold.GetComponent<gunManager>())
                    {
                        item.transform.localEulerAngles = new Vector3(hold.GetComponent<gunManager>().gunRotationXOffset, hold.GetComponent<gunManager>().gunRotationYOffset, 0f);
                        //item.transform.rotation = Quaternion.Euler(item.transform.rotation.x + hold.GetComponent<gunManager>().gunRotationXOffset, hold.GetComponent<gunManager>().gunRotationYOffset, 0f);
                    }

                    if (hit.collider.gameObject.CompareTag("gun"))
                    {
                        gunMenager = item.GetComponent<gunManager>();
                        gunMenager.isHold = true;
                    }

                }
                else if (hit.collider.gameObject.CompareTag("Gadjet"))
                {
                    if (item.GetComponent<gunManager>() && gadJet1 == null)
                    {
                        gadJet1 = hit.collider.gameObject.GetComponent<gadJet>();
                        attachItem(gadJet1);
                    }
                    else if (item.GetComponent<gunManager>() && gadJet2 == null)
                    {
                        gadJet2 = hit.collider.gameObject.GetComponent<gadJet>();
                        attachItem(gadJet2);
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Granade"))
                {
                    Debug.Log("hit in granada tag " + hit.collider.gameObject);
                    if (granadeManager != null)
                    {
                        if (granadeManager.granadeType == hit.collider.gameObject.GetComponent<granadeManager>().granadeType && granadeManager.maxForType > numberOfGranade)
                        {
                            if (!hit.collider.gameObject.GetComponent<granadeManager>().inPlayerPossesion)
                            {
                                granadeManager = hit.collider.gameObject.GetComponent<granadeManager>();
                                granadeArray[numberOfGranade] = hit.collider.gameObject;
                                numberOfGranade++;
                                setGranadePositionOnBelt();
                            }
                        }
                    }
                    else
                    {
                        if (numberOfGranade > 0)
                        {
                            for (int i = 0; i < numberOfGranade; i++)
                            {
                                granadeArray[i].transform.parent = null;
                                granadeArray[i].transform.position = hit.collider.gameObject.transform.position;
                            }
                        }



                        granadeManager = hit.collider.gameObject.GetComponent<granadeManager>();
                        granadeArray[numberOfGranade] = hit.collider.gameObject;
                        setGranadePositionOnBelt();

                        numberOfGranade = 1;

                    }
                }

            }

        }

        void setGranadePositionOnBelt()
        {
            granadeManager.inPlayerPossesion = true;
            granadeManager.rigidbody.isKinematic = true;

            granadeManager.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            granadeManager.transform.parent = granadeHold.gameObject.transform;
            granadeManager.transform.localPosition = new Vector3(0f, 0f, 0f);
            granadeManager.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }


    }

    /*
     * this method "attachItem" is used to place a gadjet on the right spot on a weapon
     */
    private void attachItem(gadJet g)
    {
        if (g.gadjetType.ToString() == "flashLight")
            g.transform.parent = item.gameObject.GetComponent<gunManager>().gadJetBarrel.transform;
        else if (g.gadjetType.ToString() == "longRangeScope" || g.gadjetType.ToString() == "scope")
        {
            g.transform.parent = item.gameObject.GetComponent<gunManager>().gadJetScope.transform;
            item.gameObject.GetComponent<gunManager>().hasScope = true;
        }

        g.GetComponent<gadJet>().isHold = true;
        g.transform.localPosition = new Vector3(0f, 0f, 0f);
        g.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        g.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    private void dropItem()
    {
        if (item.gameObject != null)
        {
            isRightHandFull = false;
            item.GetComponent<gunManager>().isHold = false;
            item.transform.localPosition = new Vector3(0f, 0f, 0f);
            item.transform.parent = null;
            item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            item.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            item = null;
        }

        if (gadJet1 != null)
            gadJet1 = null;

        if (gadJet2 != null)
            gadJet2 = null;
    }
    private void aim()
    {

        if (isRightHandFull)
            if (item.GetComponent<gunManager>())
            {
                gunHold.transform.localPosition = new Vector3(0f, -0.3f, 0.7f);

                if (item.gameObject.GetComponent<gunManager>().hasScope)
                {
                    item.transform.localPosition = new Vector3(item.GetComponent<gunManager>().gunScopeAimPositionXOffset, item.GetComponent<gunManager>().gunScopeAimPositionYOffset, item.GetComponent<gunManager>().gunScopeAimPositionZOffset);
                }
                else
                {
                    item.transform.localPosition = new Vector3(item.GetComponent<gunManager>().gunAimPositionXOffset, item.GetComponent<gunManager>().gunAimPositionYOffset, item.GetComponent<gunManager>().gunAimPositionZOffset);
                }

                if (!isAiming)
                    Camera.main.fieldOfView += item.GetComponent<gunManager>().zoomOnAim;
                isAiming = true;
            }
            else item.transform.position = gunHold.position;
    }

    private void throwGranade()
    {

        if (numberOfGranade > 0)
        {
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().transform.parent = null;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().inPlayerPossesion = false;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().beenThrown = true;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().rigidbody.isKinematic = false;
            granadeArray[numberOfGranade - 1].transform.position = gunHold.position;
            granadeArray[numberOfGranade - 1].transform.rotation = gameObject.transform.rotation;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().rigidbody.AddForce((granadeArray[numberOfGranade - 1].transform.up + granadeArray[numberOfGranade - 1].transform.forward ) * granadeManager.thrust, ForceMode.Impulse);
            granadeArray[numberOfGranade - 1] = null;
            numberOfGranade--;
        }
        if (numberOfGranade <= 0)
            granadeManager = null;

    }
}
