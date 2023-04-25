using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinionSpawnerController : MonoBehaviour
{
    [SerializeField] GameObject mutant;
    [SerializeField] string line;
    private float spawnTime;
    
    void Start()
    {
        spawnTime = Time.time;
     }

    void Update()
    {

        if (!NetworkManager.Singleton.IsServer) return;
        if (Time.time - spawnTime > 5)
        {
        //SpawnMinionServerRpc();
            spawnTime = Time.time;
            //spawn = false;
            GameObject minion = Instantiate(mutant, transform.position, Quaternion.identity);
            minion.GetComponent<MutantController>().SetTag(gameObject.tag);
            minion.GetComponent<MutantController>().SetLine(line);
            minion.GetComponent<NetworkObject>().Spawn();
        }
    }
}
