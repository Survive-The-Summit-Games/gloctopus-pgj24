using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunQueue : MonoBehaviour
{
    private GameObject[] guns;
    private int current_idx = 0;

    // Start is called before the first frame update
    void Start()
    {
        guns = GameObject.FindGameObjectsWithTag("Gun");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            guns[current_idx].GetComponent<Gun>().Fire();
            this.current_idx = (this.current_idx + 1 < this.guns.Length) ? this.current_idx + 1 : 0;
        }
    }
}
