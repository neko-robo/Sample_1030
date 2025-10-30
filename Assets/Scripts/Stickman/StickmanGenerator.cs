using System.Collections.Generic;
using UnityEngine;

public class StickmanGenerator : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] stickPrefabsTeam0; // Prefabs for team 0
    [SerializeField] private GameObject[] stickPrefabsTeam1; // Prefabs for team 1
    [SerializeField] private Transform spawnPointTeam0;      // Left-side spawn position
    [SerializeField] private Transform spawnPointTeam1;      // Right-side spawn position
    [SerializeField] private float spawnInterval = 5f;       // Seconds between spawns
    [SerializeField] private GameObject destroyBoderObj;
    private List<Stick3Ctrl> ctrlList1;
    private List<Stick3Ctrl> ctrlList2;

    private float _timer;

    public void Start()
    {
        ctrlList1 = new List<Stick3Ctrl>();
        ctrlList2 = new List<Stick3Ctrl>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;

        SpawnUnit(1);

        _timer = 0f;
    }

    private void SpawnUnit(int team)
    {
        GameObject[] pool = team == 0 ? stickPrefabsTeam0 : stickPrefabsTeam1;
        if (pool == null || pool.Length == 0) return;

        GameObject prefab = pool[Random.Range(0, pool.Length)];
        Transform point = team == 0 ? spawnPointTeam0 : spawnPointTeam1;

        GameObject obj = Instantiate(prefab, point.position, Quaternion.identity);
        Stick3Ctrl ctrl = obj.GetComponent<Stick3Ctrl>();
        ctrl.Init(team, destroyBoderObj);

        if (team == 0)
        {
            ctrlList1.Add(ctrl);
        }
        else
        {
            ctrlList2.Add(ctrl);
        }
    }

    public List<Stick3Ctrl> GetStickListByTeam(int team)
    {
        List<Stick3Ctrl> result = new List<Stick3Ctrl>();

        if (team == 0)
        {
            foreach (Stick3Ctrl ctrl in ctrlList1)
            {
                if (ctrl != null)
                {
                    result.Add(ctrl);
                }
            }
        }
        else
        {
            foreach (Stick3Ctrl ctrl in ctrlList2)
            {
                if (ctrl != null)
                {
                    result.Add(ctrl);
                }
            }
        }

        return result;
    }
}
