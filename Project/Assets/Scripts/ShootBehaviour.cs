using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *Probe con rigidBody y fuerzas pero clipean las paredes, voy a probar con raycast
 *
 * 
 */
public class ShootBehaviour : MonoBehaviour {
    public ParticleSystem muzzleFlash;
    public Transform bulletSpawnTransform;
    //public GameObject bulletPrefab;
    //public float bulletImpulse;
    public GameObject impactEffect;
    public float delayBetweenShots;
    public bool canShoot;

    
    /*this variable is only for raycast*/
    public float range;

    private void Awake() {
        canShoot = true;
    }


    // Update is called once per frame
    private void Update() {
        if (!Input.GetButton("Fire1") || !CanShoot()) return;
        muzzleFlash.Play();
        //ShootWithRigidBody();
        ShootWithRayCast();

        /*
        RaycastHit hitInfo;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10; // select distance = 10 units from the camera
        if (Physics.Raycast (yourObject.position, mousePos, out hitInfo))
        {
            GetComponent<Rigidbody>().AddForce(rayOrigin.direction * shotSpeed);
        }*/
    }

    private void ShootWithRayCast() {
        RaycastHit raycastHitInfo;
        /* el range es opcional, se puede sacar el parametro y anda igual*/
        if (Physics.Raycast(bulletSpawnTransform.position, bulletSpawnTransform.forward, out raycastHitInfo, range)) {
            Debug.Log(raycastHitInfo.transform.name); //debug code, sacarlo despues
            
            // aca puedo tomar al objeto que le pegue con raycastHitInfo.transform.getComponent o algo asi, y puedo directamente acceder al script de vida y restarsela.
            // osea creo un auxiliar para atrapar stats, Stats stats = raycastHitInfo.transform.getComponent<Stats>();
            // stats.perderVida();
            GameObject impactGameObject = Instantiate(impactEffect, raycastHitInfo.point, Quaternion.LookRotation(raycastHitInfo.normal));
            var impactMainModule = impactGameObject.GetComponent<ParticleSystem>().main;
            Destroy(impactGameObject, impactMainModule.duration);
        }
        canShoot = false;
        StartCoroutine(ShootDelay());
    }

    // private void ShootWithRigidBody() {
    //     Vector3 mousePos = Input.mousePosition;
    //     Ray ray = Camera.main.ScreenPointToRay(mousePos);
    //
    //     GameObject bullet =
    //         Instantiate(bulletPrefab, bulletSpawnTransform.transform.position, bulletSpawnTransform.rotation) as
    //             GameObject;
    //     Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
    //
    //     Vector3 direction = (ray.GetPoint(100000.0f) - bullet.transform.position).normalized;
    //
    //     bulletRigidbody.AddForce(direction * bulletImpulse, ForceMode.Impulse);
    //     
    //     canShoot = false;
    //     StartCoroutine(ShootDelay());
    // }

    private bool CanShoot() {
        return canShoot;
    }
    
    private IEnumerator ShootDelay() {
        yield return new WaitForSeconds(delayBetweenShots);
        canShoot = true;
    }
}
