using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    public Sprite backgroundSprite;
    public int width;
    public int height;
    public float parallaxSpeed;
    public float thresholdX;
    public float thresholdY;
    private List<Transform> childTransforms;

    private Transform cameraTransform;
    private int spriteWidth;
    private int spriteHeight;

    // Use this for initialization
    void Start()
    {
        childTransforms = new List<Transform>();
        cameraTransform = Camera.main.transform;
        spriteWidth = (int)backgroundSprite.bounds.size.x;
        spriteHeight = (int)backgroundSprite.bounds.size.y;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                GameObject child = new GameObject();
                child.transform.parent = transform;
                SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
                renderer.sprite = backgroundSprite;
                child.transform.position = new Vector3(w * spriteWidth - (spriteHeight * width / 2), h * spriteHeight - (spriteHeight * height / 2), 0);
                childTransforms.Add(child.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in childTransforms)
        {
            float deltaX = cameraTransform.position.x - child.transform.position.x;
            float deltaY = cameraTransform.position.y - child.transform.position.y;

            if (deltaX > thresholdX) child.transform.position = new Vector3(child.position.x + spriteWidth * width, child.position.y, child.position.z);
            if (deltaX <= -thresholdX) child.transform.position = new Vector3(child.position.x - spriteWidth * width, child.position.y, child.position.z);
            if (deltaY > thresholdY) child.transform.position = new Vector3(child.position.x, child.position.y + spriteHeight * height, child.position.z);
            if (deltaY <= -thresholdY) child.transform.position = new Vector3(child.position.x, child.position.y - spriteHeight * height, child.position.z);
        }
    }
}
