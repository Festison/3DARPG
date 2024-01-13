using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SpawnStrategy
{
    public abstract void Spawn();
}

public class MonsterSpawnPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                MonsterObjPool.Instance.PopMonster("Mushroom", Quaternion.identity);
            }           
        }    
    }
}
