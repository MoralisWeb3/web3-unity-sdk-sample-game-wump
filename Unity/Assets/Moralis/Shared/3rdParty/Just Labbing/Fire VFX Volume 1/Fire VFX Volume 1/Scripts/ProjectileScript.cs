using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public bool madeContact;
    public bool hasBeenFired;
    public float speed;
    public Rigidbody projectileBody;
    public GameObject impactVFX;
    public float currLife;
    public float maxLife = 7.5f;
    // Start is called before the first frame update
    void Start()
    {
        projectileBody = GetComponent<Rigidbody>();
    }

    void Fly()
    {
        projectileBody.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasBeenFired)
        {
            Fly();
        }
        if(currLife < maxLife)
        {
            currLife += Time.deltaTime;
        }
        else
        {
            Instantiate(impactVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            madeContact = true;
            Debug.Log("Contact Made");
            Instantiate(impactVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
