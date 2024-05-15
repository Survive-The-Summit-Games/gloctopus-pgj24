using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using System.Linq;
using Input = UnityEngine.Input;

public class GunQueue : MonoBehaviour
{
    private GameObject[] guns;
    private int current_idx = 0;
    public GameObject gun_wheel;
    public float duration = 0.25f; // Duration of the rotation

    private GameObject[] gun_wheel_images;
    private bool isRotating = false; // Flag to check if the wheel is rotating

    bool updated = false;

    // Start is called before the first frame update
    void Start()
    {
        this.guns = GameObject.FindGameObjectsWithTag("Gun");
        this.gun_wheel_images = GameObject.FindGameObjectsWithTag("Gun_Image").OrderBy(go => go.name).ToArray();
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
            this.current_idx = (this.current_idx - 1 < 0) ? guns.Length - 1 : this.current_idx - 1;
            updated = true;
        }

        // Rotate to the next gun when Mouse1 or E is pressed
        if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E)) && !updated)
        {
            this.current_idx = (this.current_idx + 1 >= guns.Length) ? 0 : this.current_idx + 1;
            updated = true;
        }

        // If updated, start rotating the gun wheel
        if (updated)
        {
            StartCoroutine(RotateGunWheel());
            updated = false;
        }
    }

    // Coroutine to smoothly rotate the gun wheel
    private IEnumerator RotateGunWheel()
    {
        isRotating = true;
        float targetAngle = 45f * current_idx;
        float currentAngle = gun_wheel.transform.rotation.eulerAngles.z;
        float elapsed = 0f;

        // Ensure the shortest rotation direction
        if (targetAngle - currentAngle > 180)
            targetAngle -= 360;
        if (targetAngle - currentAngle < -180)
            targetAngle += 360;

        while (elapsed < this.duration)
        {
            float angle = Mathf.Lerp(currentAngle, targetAngle, elapsed / this.duration);
            this.gun_wheel.transform.rotation = Quaternion.Euler(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is exact
        this.gun_wheel.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        isRotating = false;
    }
}
