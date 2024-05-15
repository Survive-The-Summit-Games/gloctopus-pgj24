using UnityEngine;
using System.Collections;
using System.Linq;
using Input = UnityEngine.Input;
using System;
using System.Collections.Generic;

public class GunQueue : MonoBehaviour
{
    public GameObject gun_wheel;
    public float rotationDuration = 0.25f; // Duration of the rotation
    public float transformDuration = 0.5f; // Duration of the scale and translation

    private GameObject[] gun_wheel_images;
    private bool isRotating = false; // Flag to check if the wheel is rotating
    private GameObject[] gun_holders;
    private GameObject[] guns = { null, null, null, null, null, null, null, null }; // our collection of guns
    private int current_idx = 0; // What Gun we are currently on

    bool updated = false;

    // Start is called before the first frame update
    void Start()
    {
        this.gun_wheel_images = GameObject.FindGameObjectsWithTag("Gun_Image").OrderBy(go => go.name).ToArray();
        this.gun_holders = GameObject.FindGameObjectsWithTag("Gun_Holder").OrderBy(go => go.name).ToArray();

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

        // Initialize the first gun wheel image to be scaled and translated
        StartCoroutine(GrowAndMoveUp(gun_wheel_images[current_idx]));
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            return; // Block input during rotation
        }

        // Fire the current gun when Space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.guns[this.current_idx].GetComponent<Gun>().Fire();
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

    // Coroutine to handle the complete switching process
    private IEnumerator SwitchGun(int direction)
    {
        isRotating = true;

        // Shrink and move down the current gun wheel image
        yield return StartCoroutine(ShrinkAndMoveDown(gun_wheel_images[current_idx]));

        // Update the current index
        current_idx = (current_idx + direction + guns.Length) % guns.Length;

        // Rotate the gun wheel
        yield return StartCoroutine(RotateGunWheel());

        // Grow and move up the new gun wheel image
        yield return StartCoroutine(GrowAndMoveUp(gun_wheel_images[current_idx]));

        isRotating = false;
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
}
