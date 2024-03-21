using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerSpawnObject : NetworkBehaviour
{
    [SerializeField] private GameObject _blackSpawn;
    [SerializeField] private GameObject _whiteSpawn;
    private GameObject _spawnedObject;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            GetComponent<PlayerSpawnObject>().enabled = false;
    }

    private void Update()
    {
        if (_spawnedObject == null && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObject(PlayerManager.GetPlayerInfo(base.Owner).team == PieceTeam.White ? _whiteSpawn : _blackSpawn, transform, this);
        }

        if (_spawnedObject != null && Input.GetKeyDown(KeyCode.Alpha2))
        {
            DespawnObject(_spawnedObject);
        }
    }

    [ServerRpc]
    public void SpawnObject(GameObject obj, Transform player, PlayerSpawnObject script)
    {
        GameObject spawned = Instantiate(obj, player);
        ServerManager.Spawn(spawned);

        SetSpawnedObject(spawned, script);
    }

    [ObserversRpc]
    public void SetSpawnedObject(GameObject spawned, PlayerSpawnObject script)
    {
        script._spawnedObject = spawned;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }
}
