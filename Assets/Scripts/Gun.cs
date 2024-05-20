using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
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
    public string gun_name;
    public string gun_description;
    public Sprite gun_Icon;
    public AudioClip gunshotSfx;
    [Range(0.0f,1.0f)]
    public float gunshotVolume = 0.5f;
    public AudioClip reloadSfx;
    [Range(0.0f,1.0f)]
    public float reloadVolume = 0.5f;

    // Hidden Info
    private int gun_id;
    private int current_clip = 0;
    private int current_exit = 0;
    private Transform mainBody;

    // Gun Info
    [SerializeField] private Transform[] gun_exit;
    [SerializeField] private GunType gun_type = GunType.Manual;
    [SerializeField] private float gun_weight = 0.0f;
    [SerializeField] private int gun_total_ammo = 1000;
    [SerializeField] private int gun_clip_size = 20;
    public bool gun_in_world = false;

    // The Bullet itself
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bullet_speed = 10.0f;

    // recoil info
    [SerializeField] private float gun_max_recoil_z = 20.0f;
    [SerializeField] private float gun_recoil_speed = 10.0f;
    [SerializeField] private float gun_kickback = 10.0f;
    [SerializeField] private float recoil_variance = 5;
    public Rigidbody2D arm_rb;

    // Start is called before the first frame update
    void Start()
    {
        // Generate a random value for the gun id
        Random random = new Random();

        // Generate a random number between 100000 and 999999
        this.gun_id = random.Next(100000, 1000000);

        // set up the clip to be the correct amount
        this.current_clip = this.gun_clip_size;
        this.gun_total_ammo -= this.gun_clip_size;

        // set up the kickback with the right stuff

        this.mainBody = GameObject.FindGameObjectWithTag("Gloctopus").transform;
    }

    private void Update()
    {
        if (this.mainBody != null)
        {
            // Calculate the opposite direction relative to the parent's right direction
            Vector2 direction = (transform.root.position - transform.position).normalized;
            Quaternion oppositeRotation = Quaternion.LookRotation(direction, this.mainBody.transform.right); // NATE this is yelling at me saying "view direction is zero"
            oppositeRotation.x = 0;
            oppositeRotation.y = 0;

            // Apply the opposite rotation to the child object
            transform.rotation = Quaternion.Slerp(transform.rotation, oppositeRotation, Time.deltaTime * 10f);
        }
    }

    public string GetAmmoText() {
        return this.current_clip + " in Clip\n" + this.gun_total_ammo + " Ammo Left";
    }

    public bool Fire() {
        if (this.current_clip != 0)
        {
            this.current_clip -= 1;
            Vector3 temp_position = gun_exit[this.current_exit].position;
            temp_position.z = 0;
            GameObject current_Bullet = Instantiate(this.bullet, temp_position, gun_exit[this.current_exit].rotation, GameObject.Find("ParentObject").transform);
            current_Bullet.GetComponent<Bullet>().Fire(this.bullet_speed);
            current_Bullet.GetComponent<Bullet>().shot_from = this.gun_id;
            this.current_exit = (this.current_exit + 1 < this.gun_exit.Length) ? this.current_exit + 1 : 0;
            GetComponent<AudioSource>().PlayOneShot(gunshotSfx, gunshotVolume);
            this.Recoil();
            return true;
        }
        else if (this.current_clip == 0 && this.gun_total_ammo > 0)
        {
            GetComponent<AudioSource>().PlayOneShot(reloadSfx, reloadVolume);
            this.reload();
        }
        else { 
            // display some message saying no more ammo
        }
        return false;
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

    public void Recoil()
    {
        //every time you fire a bullet, add to the recoil.. of course you can probably set a max recoil etc..
        //recoil += 0.1f;
        // TODO: NATE idk why this random isnt working lmao
        //Vector2 direction = Quaternion.AngleAxis(Random.Range(-recoil_variance, recoil_variance), Vector3.left) * (transform.root.position - this.arm_rb.transform.position).normalized;
        Vector2 direction = Quaternion.AngleAxis(0, Vector3.left) * (transform.root.position - this.arm_rb.transform.position).normalized;
        this.arm_rb.AddForce(direction * this.gun_kickback);
    }
}
