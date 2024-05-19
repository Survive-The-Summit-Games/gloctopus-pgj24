using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    [Range(0, 10)]
    public int currentLevel = 0;

    MapGenerator mapGenerator;


    public int baseWidth;
    public int baseHeight;

    public int baseRandomFillPercent;

    public LayerMask mapMeshLayerMask;

    public Collider2D entranceCollider;
    public Collider2D exitCollider;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        mapGenerator = GetComponentInChildren<MapGenerator>();
        mapGenerator.width = (int)(baseWidth * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.height = (int)(baseHeight * Mathf.Pow(0.9f, (float) currentLevel));
        mapGenerator.randomFillPercent = baseRandomFillPercent + currentLevel;
        mapGenerator.GenerateMap();
    }

    void NextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        //Debug.Log(Physics2D.IsTouchingLayers(exitCollider, mapMeshLayerMask));
        //Debug.Log(exitCollider.IsTouchingLayers(mapMeshLayerMask));
    }
}
