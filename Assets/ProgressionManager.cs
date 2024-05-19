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
        List<Vector2Int> roomCenters = mapGenerator.roomCenters;
        Vector2IntHeight heightComparer = new Vector2IntHeight();
        roomCenters.Sort(heightComparer);
        Vector2Int spawnpoint = Vector2Int.zero;
        for (int i = 0; i < roomCenters.Count; i++)
        {
            Vector2Int roomCenter = roomCenters[i];
            if (Physics2D.OverlapCircle(roomCenter, 1f, mapMeshLayerMask) == null)
            {
                if (spawnpoint == Vector2.zero)
                {
                    spawnpoint = roomCenter;
                } else
                {
                    Debug.Log(roomCenter);
                    Instantiate(roomMarker, new Vector3(roomCenter.x, roomCenter.y, 0), Quaternion.identity);
                }
            }
        }

        Transform player = FindAnyObjectByType<SimpleMovement>().transform;
        player.position = new Vector3(spawnpoint.x, spawnpoint.y, player.transform.position.z);
    }

    void NextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GenerateMap();
    }
}

public class Vector2IntHeight : IComparer<Vector2Int>
{
    public int Compare(Vector2Int a, Vector2Int b)
    {
        return a.y.CompareTo(b.y);
    }
}
