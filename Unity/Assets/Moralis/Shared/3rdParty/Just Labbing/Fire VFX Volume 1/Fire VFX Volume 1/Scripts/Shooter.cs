using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject[] vfxToShoot;
    public Transform body;
    public Transform viewpoint;
    public int selectedVFX;
    public bool canFire;
    public float currFireTime, maxFireTime;
    public Vector3 targetRotation;
    public CharacterScript cS;
    public float inBetween;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetInfo()
    {
        canFire = true;
        currFireTime = 0;
    }

    void Fire()
    {
        float r = Random.Range(0f, 2f);
        if(r > .5f)
        {
            StartCoroutine(CharacterProjectile(cS.leftHand));
        }
        else
        {
            StartCoroutine(CharacterProjectile(cS.rightHand));
        }
    }

    void AimAt()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            if(hit.transform.gameObject.CompareTag("Wall"))
            {
                targetRotation = hit.point;
                Vector3 direction = targetRotation - transform.position;
                
                transform.forward = direction;
                var lookPos = targetRotation - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                body.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 90);
            }

            // Do something with the object that was hit by the raycast.
        }
    }

    public IEnumerator CharacterProjectile(CharacterScript.HandInfo hand)
    {
        canFire = false;
        float t = 0;
        cS.CastProjectile(hand);
        while (t<inBetween)
        {

            t += Time.deltaTime;
            yield return null;
        }
        transform.position = hand.hand.transform.position;
        GameObject firedVFX = Instantiate(vfxToShoot[selectedVFX], hand.hand.transform.position, transform.rotation);
        ProjectileScript fbs = firedVFX.GetComponent<ProjectileScript>();
        fbs.hasBeenFired = true;
        currFireTime = 0;
        canFire = true;
    }
    public void SwitchProjectile(bool t)
    {
        if(t)
        {
            if(selectedVFX >= vfxToShoot.Length-1)
            {
                selectedVFX = 0;
            }
            else
            {
                selectedVFX++;
            }
        }
        else
        {
            if(selectedVFX<=0)
            {
                selectedVFX = vfxToShoot.Length - 1;
            }
            else
            {
                selectedVFX--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canFire)
        {
            if (currFireTime >= maxFireTime)
            {
                Fire();
            }
            else
            {
                currFireTime += Time.deltaTime;
            }
        }
        AimAt();
    }
}
