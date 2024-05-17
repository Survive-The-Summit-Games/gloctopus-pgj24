using UnityEngine;
using System.Collections;
using System.Linq;
using Input = UnityEngine.Input;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.U2D;

public enum pick_up_mode
{
    pick_up, replace, nothing
}

public class GunQueue : MonoBehaviour
{
    public GameObject gun_wheel;
    public float rotationDuration = 0.25f; // Duration of the rotation
    public float transformDuration = 0.5f; // Duration of the scale and translation

    public Text ammo_text;
    public Text pickupText;

    private GameObject[] gun_wheel_images;
    private bool isRotating = false; // Flag to check if the wheel is rotating
    private GameObject[] gun_holders;
    private GameObject[] gun_targets;
    private GameObject[] guns = { null, null, null, null, null, null, null, null }; // our collection of guns
    private int current_idx = 0; // What Gun we are currently on

    private bool updated = false; 
    private pick_up_mode pick_up_mode = pick_up_mode.nothing;

    // Start is called before the first frame update
    void Start()
    {
        this.gun_wheel_images = GameObject.FindGameObjectsWithTag("Gun_Image").OrderBy(go => go.name).ToArray();
        this.gun_holders = GameObject.FindGameObjectsWithTag("Gun_Holder").OrderBy(go => go.name).ToArray();
        this.gun_targets = GameObject.FindGameObjectsWithTag("Gun_Target").OrderBy(go => go.name).ToArray();

        // Iterate through the gun_holders array
        for (int i = 0; i < gun_holders.Length; i++)
        {
            // Find the child object with the tag "Gun"
            foreach (Transform child in gun_holders[i].transform)
            {
                if (child.CompareTag("Gun"))
                {
                    guns[i] = child.gameObject;
                    break; // Break the inner loop once the gun is found
                }
            }
        }

        this.UpdateGunWheel();
        this.UpdateAmmoText();

        // Initialize the first gun wheel image to be scaled and translated
        StartCoroutine(GrowAndMoveUp(gun_wheel_images[current_idx]));
    }

    // Update is called once per frame
    void Update()
    {
        GameObject closestGun = GetClosestGun();
        if (closestGun != null)
        {
            ShowPickupText(closestGun);
        }
        else
        {
            HidePickupText();
        }

        if (isRotating)
        {
            return; // Block input during rotation
        }

        if (this.pick_up_mode == pick_up_mode.pick_up && Input.GetKeyDown(KeyCode.P) && closestGun != null)
        {
            PickUp(closestGun, false);
            closestGun = null;
            this.UpdateAmmoText();
            this.UpdateGunWheel();
            // Do we also want it to flip to it?
        }

        if (this.pick_up_mode == pick_up_mode.replace && Input.GetKeyDown(KeyCode.L) && closestGun != null)
        {
            GameObject temp_throw_gun = this.guns[current_idx];
            this.guns[current_idx] = null;
            temp_throw_gun.GetComponent<Gun>().gun_in_world = true;
            temp_throw_gun.GetComponent<Gun>().arm_rb = null;
            temp_throw_gun.transform.parent = closestGun.transform.parent;
            temp_throw_gun.transform.SetPositionAndRotation(closestGun.transform.position, closestGun.transform.rotation);
            temp_throw_gun.transform.localScale = closestGun.transform.localScale;
            PickUp(closestGun, true);
            closestGun = null;
            this.UpdateAmmoText();
            this.UpdateGunWheel();
        }

        // Fire the current gun when Space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.guns[this.current_idx].GetComponent<Gun>().Fire();
            this.UpdateAmmoText();
        }

        // Rotate to the previous gun when Mouse0 or Q is pressed
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Q)) && !updated)
        {
            bool found = false;

            if (!found)
            {
                StartCoroutine(SwitchGun(-1));
                updated = true;
            }
        }

        // Rotate to the next gun when Mouse1 or E is pressed
        if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E)) && !updated)
        {
            bool found = false;

            if (!found)
            {
                StartCoroutine(SwitchGun(1));
                updated = true;
            }
        }
    }

    private void PickUp(GameObject closestGun, bool current_spot) {
        int temp_spot;
        if (current_spot)
        {
            temp_spot = this.current_idx;
        }
        else {
            temp_spot = this.SpotAvailable();
        }
        this.guns[temp_spot] = closestGun;
        closestGun.GetComponent<Gun>().gun_in_world = false;
        closestGun.GetComponent<Gun>().arm_rb = gun_targets[temp_spot].GetComponent<Rigidbody2D>();
        closestGun.transform.parent = this.gun_holders[temp_spot].transform;
        closestGun.transform.localPosition = Vector3.zero;
        closestGun.transform.localRotation = Quaternion.identity;
        closestGun.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }

    private void UpdateAmmoText()
    {
        this.ammo_text.text = this.guns[this.current_idx].GetComponent<Gun>().GetAmmoText();
    }

    private void UpdateGunWheel()
    {
        for (int i = 0; i < this.guns.Length; i++)
        {
            Image temp_old_image = this.gun_wheel_images[i].transform.GetComponentInChildren<Image>(true);
            if (this.guns[i] != null)
            {
                Sprite temp_new_sprite = this.guns[i].GetComponent<Gun>().gun_Icon;

                if (temp_old_image != null && temp_new_sprite != null)
                {
                    // Set the sprite of the Image
                    temp_old_image.sprite = temp_new_sprite;

                    // Get the sprite dimensions
                    float spriteWidth = temp_new_sprite.rect.width;
                    float spriteHeight = temp_new_sprite.rect.height;
                    float pixelsPerUnit = temp_new_sprite.pixelsPerUnit;

                    // Calculate the size in units
                    float widthInUnits = spriteWidth / pixelsPerUnit;
                    float heightInUnits = spriteHeight / pixelsPerUnit;

                    // Adjust the RectTransform size
                    RectTransform rectTransform = temp_old_image.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(widthInUnits, heightInUnits);
                }

                temp_old_image.enabled = true;
            }
            else {
                temp_old_image.enabled = false;
            }
        }
    }

    // Coroutine to handle the complete switching process
    private IEnumerator SwitchGun(int direction)
    {
        int new_pos = (current_idx + direction + guns.Length) % guns.Length;

        // search for the next gun that exists
        while (this.guns[new_pos] == null)
        {
            //Debug.Log("Looking for next gun");
            new_pos = (new_pos + direction + guns.Length) % guns.Length;
        }
        //Debug.Log("Found next gun " + new_pos);

        if (new_pos != current_idx)
        {
            isRotating = true;

            // Shrink and move down the current gun wheel image
            yield return StartCoroutine(ShrinkAndMoveDown(gun_wheel_images[current_idx]));

            // Update the current index
            current_idx = new_pos;

            // Rotate the gun wheel
            yield return StartCoroutine(RotateGunWheel());

            this.UpdateAmmoText();

            // Grow and move up the new gun wheel image
            yield return StartCoroutine(GrowAndMoveUp(gun_wheel_images[current_idx]));

            isRotating = false;
        }
        updated = false;
    }

    // Coroutine to smoothly rotate the gun wheel
    private IEnumerator RotateGunWheel()
    {
        float targetAngle = 45f * current_idx;
        float currentAngle = gun_wheel.transform.rotation.eulerAngles.z;
        float elapsed = 0f;

        // Ensure the shortest rotation direction
        if (targetAngle - currentAngle > 180)
            targetAngle -= 360;
        if (targetAngle - currentAngle < -180)
            targetAngle += 360;

        while (elapsed < this.rotationDuration)
        {
            float angle = Mathf.Lerp(currentAngle, targetAngle, elapsed / this.rotationDuration);
            this.gun_wheel.transform.rotation = Quaternion.Euler(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is exact
        this.gun_wheel.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    // Coroutine to grow and move up the selected gun wheel image
    private IEnumerator GrowAndMoveUp(GameObject gunImage)
    {
        Vector3 initialScale = gunImage.transform.localScale;
        Vector3 targetScale = initialScale * 1.2f; // Grow the gun image by 20%
        Vector3 initialPosition = gunImage.transform.position;
        Vector3 targetPosition = initialPosition + Vector3.up; // Move the gun image up by 1 unit
        float elapsed = 0f;

        while (elapsed < this.transformDuration)
        {
            float t = elapsed / this.transformDuration;
            gunImage.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            gunImage.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final transformations are exact
        gunImage.transform.localScale = targetScale;
        gunImage.transform.position = targetPosition;
    }

    // Coroutine to shrink and move down the deselected gun wheel image
    private IEnumerator ShrinkAndMoveDown(GameObject gunImage)
    {
        Vector3 initialScale = gunImage.transform.localScale;
        Vector3 targetScale = initialScale / 1.2f; // Shrink the gun image by the inverse of 20%
        Vector3 initialPosition = gunImage.transform.position;
        Vector3 targetPosition = initialPosition - Vector3.up; // Move the gun image down by 1 unit
        float elapsed = 0f;

        while (elapsed < this.transformDuration)
        {
            float t = elapsed / this.transformDuration;
            gunImage.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            gunImage.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final transformations are exact
        gunImage.transform.localScale = targetScale;
        gunImage.transform.position = targetPosition;
    }
    private GameObject GetClosestGun()
    {
        GameObject[] found_guns = GameObject.FindGameObjectsWithTag("Gun");
        GameObject closestGun = null;
        float closestDistance = 5.0f;  // Distance threshold

        foreach (GameObject gun in found_guns)
        {
            if (gun.GetComponent<Gun>().gun_in_world)
            {
                float distance = Vector3.Distance(this.transform.position, gun.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGun = gun;
                }
            }
        }

        return closestGun;
    }

    private void ShowPickupText(GameObject gun)
    {
        if (pickupText != null)
        {
            pickupText.enabled = true;
            Vector3 screenPosition = (gun.transform.position + new Vector3(0.0f, 2.0f, -5.0f));
            pickupText.transform.position = screenPosition;
            if (SpotAvailable() != -1)
            {
                pickupText.text = "Press P to pick up";
                this.pick_up_mode = pick_up_mode.pick_up;
            }
            else
            {
                pickupText.text = "Press L to replace";
                this.pick_up_mode = pick_up_mode.replace;
            }
        }
    }

    private void HidePickupText()
    {
        if (pickupText != null)
        {
            pickupText.enabled = false;
            this.pick_up_mode = pick_up_mode.nothing;
        }
    }

    private int SpotAvailable() {
        for (int i = 0; i < this.guns.Length; i++) {
            if (this.guns[i] == null) {
                return i;
            }
        }
        return -1;
    }
}

