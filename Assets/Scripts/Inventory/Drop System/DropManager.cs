using UnityEngine;
using Mirror;
using Actors.AI;

public class DropManager : NetworkBehaviour
{
    public DropProfile DropsProfile;
    private Enemy enemy;
    
    private void OnEnable()
    {
        enemy = GetComponent<Enemy>();
        enemy.OnActorDeath += OnActorDeath;
    }

    private void OnActorDeath(Actors.NetworkActor obj)
    {
        DropItem();
    }

    public void DropItem()
    {
        if(isServer) ServerDrop();
    }

    [Server]
    private void ServerDrop()
    {
        int percentage = UnityEngine.Random.Range(0, 110);
        foreach(DropItem item in DropsProfile.dropItems)
        {
            if(item.DropPercentage >= percentage)
            {
                GameObject prefab = Instantiate(item.DropObj,transform.position, Quaternion.identity);
                if (prefab) NetworkServer.Spawn(prefab);
                break;
            }
        }
    }
}
