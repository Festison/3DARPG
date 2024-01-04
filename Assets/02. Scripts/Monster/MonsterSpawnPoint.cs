using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour
{
    MonsterObjPool monsterPool;

    private void Start()
    {
        monsterPool = MonsterObjPool.Instance;
    }
    private void FixedUpdate()
    {
        
    }

    public void MushRommCreate()
    {
        monsterPool.PopMonster("Mushroom", transform.position, Quaternion.identity);
    }
}
