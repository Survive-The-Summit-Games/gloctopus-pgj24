using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    public Sprite backgroundSprite;
    public int width;
    public int height;
    public float scale = 1f;
    public float parallaxSpeed;
    public float thresholdX;
    public float thresholdY;
    private List<Transform> childTransforms;

    private Transform cameraTransform;
    private float spriteWidth;
    private float spriteHeight;
    private float lastCameraX;
    private float lastCameraY;

    // Use this for initialization
    void Start()
    {
        childTransforms = new List<Transform>();
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;
        spriteWidth = backgroundSprite.bounds.size.x * scale ;
        spriteHeight = backgroundSprite.bounds.size.y * scale;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                GameObject child = new GameObject();
                child.transform.parent = transform;
                child.transform.localScale = Vector3.one * scale;
                SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
                renderer.sprite = backgroundSprite;
                child.transform.position = new Vector3(w * spriteWidth - (spriteHeight * width / 2), h * spriteHeight - (spriteHeight * height / 2), transform.position.z);
                childTransforms.Add(child.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        parallaxHor();
        parallaxVer();

        foreach (Transform child in childTransforms)
        {
            float deltaX = cameraTransform.position.x - child.transform.position.x;
            float deltaY = cameraTransform.position.y - child.transform.position.y;

            if (deltaX > thresholdX * scale) child.transform.position = new Vector3(child.position.x + spriteWidth * width, child.position.y, child.position.z);
            if (deltaX <= -thresholdX * scale) child.transform.position = new Vector3(child.position.x - spriteWidth * width, child.position.y, child.position.z);
            if (deltaY > thresholdY * scale) child.transform.position = new Vector3(child.position.x, child.position.y + spriteHeight * height, child.position.z);
            if (deltaY <= -thresholdY * scale) child.transform.position = new Vector3(child.position.x, child.position.y - spriteHeight * height, child.position.z);
        }
    }


    private void parallaxHor()
    {
        float deltaX = cameraTransform.position.x - lastCameraX;
        transform.position += Vector3.right * (deltaX * parallaxSpeed);
        lastCameraX = cameraTransform.position.x;
    }

    private void parallaxVer()
    {
        float deltaY = cameraTransform.position.y - lastCameraY;
        transform.position += Vector3.up * (deltaY * parallaxSpeed);
        lastCameraY = cameraTransform.position.y;
    }
}
