using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

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
    private Transform mainBody;

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
        // Generate a random value for the gun id
        Random random = new Random();

        // Generate a random number between 100000 and 999999
        this.gun_id = random.Next(100000, 1000000);

        // set up the clip to be the correct amount
        this.current_clip = this.gun_clip_size;
        this.gun_total_ammo -= this.current_clip;

        // set up the kickback with the right stuff
        var parent_script = this.GetComponentInParent<GunShoot>();
        parent_script.max_recoil_z = this.gun_max_recoil_z;
        parent_script.recoil_speed = this.gun_recoil_speed;
        parent_script.kickback = this.gun_kickback;

        this.mainBody = GameObject.FindGameObjectWithTag("Gloctopus").transform;
    }

    private void Update()
    {
        if (this.mainBody != null)
        {
            // Calculate the opposite direction relative to the parent's right direction
            Vector2 direction = (transform.root.position - transform.position).normalized;
            Quaternion oppositeRotation = Quaternion.LookRotation(direction, this.mainBody.transform.right);
            oppositeRotation.x = 0;
            oppositeRotation.y = 0;

            // Apply the opposite rotation to the child object
            transform.rotation = Quaternion.Slerp(transform.rotation, oppositeRotation, Time.deltaTime * 10f);
        }
    }

    public void Fire() {
        if (this.gun_total_ammo + this.current_clip != 0)
        {
            Vector3 temp_position = gun_exit[this.current_exit].position;
            temp_position.z = 0;
            GameObject current_Bullet = Instantiate(this.bullet, temp_position, gun_exit[this.current_exit].rotation, GameObject.Find("ParentObject").transform);
            current_Bullet.GetComponent<Bullet>().Fire(this.bullet_speed);
            current_Bullet.GetComponent<Bullet>().shot_from = this.gun_id;
            this.current_clip -= 1;
            this.current_exit = (this.current_exit + 1 < this.gun_exit.Length) ? this.current_exit + 1 : 0;
            this.GetComponentInParent<GunShoot>().Fire();
        }
        else if (this.current_clip == 0 && this.gun_total_ammo > 0)
        {
            this.reload();
        }
        else { 
            // display some message saying no more ammo
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
