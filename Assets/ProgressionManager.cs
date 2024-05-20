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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GenerateMap();
        GetComponent<AudioSource>().Play();
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
        mapGenerator.width = baseWidth; //(int)(baseWidth * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.height = baseHeight; //(int)(baseHeight * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.randomFillPercent = baseRandomFillPercent;
        Vector2Int exit = mapGenerator.GenerateMap();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = exit;
        bubbleEmitter.transform.position = new Vector3(exit.x, exit.y, 0);

        Debug.Log("room centers");
        List<Vector2Int> roomCenters = mapGenerator.roomCenters;
        Vector2IntHeight heightComparer = new Vector2IntHeight();
        List<Vector2Int> candidates = new List<Vector2Int>();
        foreach (Vector2Int roomCenter in roomCenters)
        {
            if (Physics2D.OverlapCircle(roomCenter, 0.5f, mapMeshLayerMask) == null)
            {
                candidates.Add(roomCenter);
            }
        }
        candidates.Sort(heightComparer);
        candidates.Reverse();
        Vector2Int spawnpoint = candidates[0];
        candidates.Remove(spawnpoint);

        foreach (Vector2Int candidate in candidates)
        {
            GameObject mine = Instantiate(roomMarker, new Vector3(candidate.x, candidate.y, 0), Quaternion.identity);
        }

        Transform player = FindAnyObjectByType<SimpleMovement>().transform;
        player.position = new Vector3(spawnpoint.x, spawnpoint.y, player.transform.position.z);
    }

    void NextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public class Vector2IntHeight : IComparer<Vector2Int>
{
    public int Compare(Vector2Int a, Vector2Int b)
    {
        return a.y.CompareTo(b.y);
    }
}
