using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GunType { 
    Manual,
    Semi_Automatic,
    Automatic,
    Burst
}

public class Gun : MonoBehaviour
{
    // Gun Flavor
    [SerializeField] private string gun_name;
    [SerializeField] private string gun_description;

    // Hidden Info
    private int gun_id;
    private int current_clip;
    private int current_exit = 0;

    // Gun Info
    [SerializeField] private Transform[] gun_exit;
    [SerializeField] private GunType gun_type = GunType.Manual;
    [SerializeField] private float gun_weight = 0.0f;
    [SerializeField] private int gun_total_ammo = 1000;
    [SerializeField] private int gun_clip_size = 20;

    // The Bullet itself
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bullet_speed = 10.0f;

    // recoil info
    [SerializeField] private float gun_max_recoil_z = 20.0f;
    [SerializeField] private float gun_recoil_speed = 10.0f;
    [SerializeField] private float gun_kickback = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        // set up the clip to be the correct amount
        this.current_clip = this.gun_clip_size;
        this.gun_total_ammo -= this.current_clip;

        // set up the kickback with the right stuff
        var parent_script = this.GetComponentInParent<GunShoot>();
        parent_script.max_recoil_z = this.gun_max_recoil_z;
        parent_script.recoil_speed = this.gun_recoil_speed;
        parent_script.kickback = this.gun_kickback;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.Fire();
        }
    }

    public void Fire() {
        if (this.gun_total_ammo + this.current_clip != 0)
        {
            GameObject current_Bullet = Instantiate(this.bullet, gun_exit[this.current_exit].position, gun_exit[this.current_exit].rotation, GameObject.Find("ParentObject").transform);
            current_Bullet.GetComponent<Bullet>().Fire(this.bullet_speed);
            this.current_clip -= 1;
            this.GetComponentInParent<GunShoot>().Fire();
        }
        else { 
            // display some message saying out of ammo entirely
        }
    }

    public void reload() {
        if (this.gun_total_ammo != 0)
        {
            // dump current clip back into general ammo pool
            this.gun_total_ammo += current_clip;
            this.current_clip = 0; // set to 0
            // fill the clip. If we do not have enough ammo to fill the clip, fill it as much as we can
            this.current_clip = Math.Min(this.gun_total_ammo, this.gun_clip_size);
            this.gun_total_ammo -= this.current_clip;
        }
        else { 
            // idk display some message saying out of ammo
        }
    }
}
