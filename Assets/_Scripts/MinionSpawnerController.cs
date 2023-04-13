using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinionSpawnerController : NetworkBehaviour
{
    [SerializeField] GameObject mutant;
    private float spawnTime;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnMinionServerRpc();
        spawnTime = Time.time;  
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - spawnTime > 5)
        {
        SpawnMinionServerRpc();
            spawnTime = Time.time;
        }
    }
    [ServerRpc]
    void SpawnMinionServerRpc()
    {
        GameObject minion = Instantiate(mutant);
        minion.GetComponent<NetworkObject>().Spawn();
    }
}
