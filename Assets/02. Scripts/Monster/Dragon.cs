using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonsterSpawnPoint
{
    public override void Init()
    {
        spawnMonster = SpawnStrategy.Factory.Create(SpawnStrategy.MonsterType.Dragon);
    }

    public void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                MonsterObjPool.Instance.PopMonster("Dragon", Quaternion.identity);
            }
        }
    }
}
