using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPun
{
    public GameObject BulletPrefab;
    public Transform firePosition;
    public Camera PlayerCamera;

    public DeathRacePlayer DeathRacePlayerProperties;

    private float fireRate;
    private float fireTimer = 0.0f;
    private bool useLaser;
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        fireRate = DeathRacePlayerProperties.fireRate;

        if (DeathRacePlayerProperties.weaponName == "Laser Gun")
        {
            useLaser = true;
        }
        else
        {
            useLaser = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //fire
        if (Input.GetKey("space"))
        {
            if (fireTimer>fireRate)
            {
                // Fire
                photonView.RPC("Fire", RpcTarget.All,firePosition.position);
                fireTimer = 0.0f;
            }

            
        }

        if (fireTimer<fireRate)
        {
            fireTimer += Time.deltaTime;
        }


    }

    [PunRPC]
    public void Fire(Vector3 _firePosition)
    {
        if (useLaser)
        {
            //laser codes
            RaycastHit _Hit;
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out _Hit, 200))
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                lineRenderer.startWidth = 0.3f;
                lineRenderer.endWidth = 0.1f;

                lineRenderer.SetPosition(0, _firePosition);
                lineRenderer.SetPosition(1, _Hit.point);

                if (_Hit.collider.gameObject.CompareTag("Player"))
                {
                    if (_Hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        _Hit.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage",
                                               RpcTarget.AllBuffered, DeathRacePlayerProperties.damage);
                    }

                   
                }





                StopAllCoroutines();
                StartCoroutine(DisableLaserAfterSecs(0.3f));
            }

        }
        else
        {
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            GameObject bulletGameObject = Instantiate(BulletPrefab, _firePosition, Quaternion.identity);
            bulletGameObject.GetComponent<BulletScript>().Initialize(ray.direction, DeathRacePlayerProperties.bulletSpeed,
                DeathRacePlayerProperties.damage);
        }

        IEnumerator DisableLaserAfterSecs(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            lineRenderer.enabled = false;
        }

        
    }
}
