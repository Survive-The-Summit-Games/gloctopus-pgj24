using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    private static ProgressionManager instance;

    public static ProgressionManager Instance {  get { return instance; } }

    [Range(0, 10)]
    public int currentLevel = 0;

    MapGenerator mapGenerator;
    public Transform bubbleEmitter;


    public int baseWidth;
    public int baseHeight;

    public int baseRandomFillPercent;

    public LayerMask mapMeshLayerMask;

    public GameObject roomMarker;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        GenerateMap();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.gameObject.tag == "Gloctopus") {
            collision.gameObject.GetComponent<SimpleMovement>().enabled = true;
            NextLevel(); 
        }
    }
    void GenerateMap()
    {

        mapGenerator = GetComponentInChildren<MapGenerator>();
        mapGenerator.width = (int)(baseWidth * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.height = (int)(baseHeight * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.randomFillPercent = baseRandomFillPercent + currentLevel;
        Vector2Int exit = mapGenerator.GenerateMap();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = exit;
        bubbleEmitter.transform.position = new Vector3(exit.x, exit.y, 0);

        Debug.Log("room centers");
        foreach (Vector2Int roomCenter in mapGenerator.roomCenters)
        {
            if (Physics2D.OverlapCircle(roomCenter, 1f, mapMeshLayerMask) == null)
            {
                Debug.Log(roomCenter);
                Instantiate(roomMarker, new Vector3(roomCenter.x, roomCenter.y, 0), Quaternion.identity);
            }
        }
    }

    void NextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GenerateMap();
    }
}
