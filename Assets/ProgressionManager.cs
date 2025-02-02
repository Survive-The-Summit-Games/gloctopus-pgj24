using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    private static ProgressionManager instance;

    public static ProgressionManager Instance {  get { return instance; } }

    int currentLevel = 0;

    MapGenerator mapGenerator;
    Transform bubbleEmitter;


    [SerializeField] int baseWidth;
    [SerializeField] int baseHeight;

    [SerializeField] int baseRandomFillPercent;

    public LayerMask mapMeshLayerMask;

    [SerializeField]
    GameObject[] possibleEnemies;
    
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if (instance != null && instance != this) Destroy(this.gameObject); 
        else instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bubbleEmitter = FindObjectOfType<SuckScript>().transform;
        mapGenerator = FindObjectOfType<MapGenerator>();
        GenerateMap();
        GameObject.FindGameObjectWithTag("Depth_Counter").GetComponent<TextMeshProUGUI>().SetText("Depth: " + (currentLevel + 1));
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
        mapGenerator.width = baseWidth; //(int)(baseWidth * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.height = baseHeight; //(int)(baseHeight * Mathf.Pow(0.9f, (float)currentLevel));
        mapGenerator.randomFillPercent = baseRandomFillPercent;
        Vector2Int exit = mapGenerator.GenerateMap();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = exit;
        bubbleEmitter.transform.position = new Vector3(exit.x, exit.y, 0);

        // TOOD: Create custom size graphs https://arongranberg.com/astar/docs/gridgraph.html
        AstarPath.active.Scan();

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
            int spawnedEnemies = 0;
            int limit = UnityEngine.Random.Range(1, currentLevel + 1);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (Physics2D.OverlapCircle(candidate + new Vector2Int(i, j), 0.5f, mapMeshLayerMask) == null)
                    {
                        GameObject enemy = Instantiate(possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Length)], new Vector3(candidate.x, candidate.y, 0), Quaternion.identity);
                        enemy.transform.parent = null;
                        spawnedEnemies++;
                    }

                    if (spawnedEnemies >= limit) break;
                }
                if (spawnedEnemies >= limit) break;
            }
        }

        Transform player = GameObject.FindGameObjectWithTag("GloctopusParent").transform;
        player.position = new Vector3(spawnpoint.x, spawnpoint.y, player.transform.position.z);
    }

    public void NextLevel()
    {
        currentLevel++;
        GameObject.FindGameObjectWithTag("FadeInOut").GetComponent<FadeInFadeOut>().FadeOut();
    }
}

public class Vector2IntHeight : IComparer<Vector2Int>
{
    public int Compare(Vector2Int a, Vector2Int b)
    {
        return a.y.CompareTo(b.y);
    }
}
