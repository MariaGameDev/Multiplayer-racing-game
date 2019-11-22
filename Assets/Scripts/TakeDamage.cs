using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TakeDamage : MonoBehaviourPun
{
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    Rigidbody rb;

    public GameObject PlayerGraphics;
    public GameObject PlayerUI;
    public GameObject PlayerWeaponHolder;
    public GameObject DeathPanelUIPrefab;
    private GameObject DeathPanelGameObject;


    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void DoDamage(float _damage)
    {
        health -= _damage;
        Debug.Log("health" + health);

        healthBar.fillAmount = health / startHealth;

        if (health<=0f)
        {
            //Die
            Die();
        }

    }

    private void Die()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        PlayerGraphics.SetActive(false);
        PlayerUI.SetActive(false);
        PlayerWeaponHolder.SetActive(false);

        if (photonView.IsMine)
        {
            //respawn
            StartCoroutine(ReSpawn());
        }

    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");

        if (DeathPanelGameObject == null)
        {
            DeathPanelGameObject = Instantiate(DeathPanelUIPrefab, canvasGameObject.transform);
        }
        else
        {
            DeathPanelGameObject.SetActive(true);
        }
        Text respawnTimeText = DeathPanelGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;
        respawnTimeText.text = respawnTime.ToString(" .00");

        while (respawnTime>0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(" .00");


            GetComponent<CarMovement>().enabled = false;
            GetComponent<Shooting>().enabled = false;

        }

        DeathPanelGameObject.SetActive(false);

        //reEnable
        GetComponent<CarMovement>().enabled = true;
        GetComponent<Shooting>().enabled = true;

        int RandomPoint = UnityEngine.Random.Range(-20,20);
        transform.position = new Vector3(RandomPoint,0,RandomPoint);

        photonView.RPC("Reborn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Reborn()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        PlayerGraphics.SetActive(true);
        PlayerUI.SetActive(true);
        PlayerWeaponHolder.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   


}
