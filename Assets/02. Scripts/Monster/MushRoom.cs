using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushRoom : MonsterSpawnPoint
{ 
    public override void Init()
    {
        spawnMonster = SpawnStrategy.Factory.Create(SpawnStrategy.MonsterType.MushRoom);
    }

    public void OnTriggerStay(Collider other)
    {
        Debug.Log("Ãæµ¹Áß");
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                MonsterObjPool.Instance.PopMonster("Mushroom", Quaternion.identity);
            }
        }
    }
}
