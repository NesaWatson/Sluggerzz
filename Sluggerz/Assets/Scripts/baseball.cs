using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseball : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] int destroyTime;

    private GameObject shooter;
    public AudioClip baseballAudio;
    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }
    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        Destroy(gameObject, destroyTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        iDamage damageable = other.GetComponent<iDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }

        Destroy(gameObject);
    }

}

