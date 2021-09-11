using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class granadeManager : MonoBehaviour
{
    public enum GranadeType { frag, smoke, fire };
    public GranadeType granadeType;

    public Rigidbody rigidbody;
    public float thrust = 10f;

    public int maxForType = 2;

    public bool inPlayerPossesion = false;
    public bool beenThrown = false;

    public GameObject effectGO;
    public float effectDuration, startOffset;
    private Coroutine effectCoroutine = null;

    public void playEffect()
    {       
        effectCoroutine = StartCoroutine(effectTimeTick(startOffset, effectDuration));
    }

    bool doOnce = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            if (beenThrown && !doOnce)
            {
                doOnce = true;
                playEffect();
            }
        }
    }

    IEnumerator effectTimeTick(float startAfter, float duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(startAfter);
            GameObject toSpawn = (GameObject)Instantiate(effectGO, transform.position, transform.rotation);
            toSpawn.transform.parent = transform.parent;
            yield return new WaitForSeconds(duration);
            toSpawn.GetComponent<VisualEffect>().Stop();
            yield return new WaitForSeconds(60f);
            StopCoroutine(effectCoroutine);
            Destroy(toSpawn);
            Destroy(this);
            yield return null;
        }
    }

}
