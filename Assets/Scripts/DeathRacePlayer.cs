using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "New Death Race Player")]
public class DeathRacePlayer : ScriptableObject
{
    public string playerName;
    public Sprite playerSprite;

    [Header("Weapon Properties")]
    public string weaponName;
    public float damage;
    public float fireRate;
    public float bulletSpeed;

}
