using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MonsterType
{
    MushRoom,
    Dragon
}

[System.Serializable]
public abstract class SpawnStrategy : MonoBehaviour
{
    public abstract void OnTriggerStay(Collider other);
}

[System.Serializable]
public class MushRoomSpawnStrategy : SpawnStrategy
{
    public override void OnTriggerStay(Collider other)
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

[System.Serializable]
public class DragonSpawnStrategy : SpawnStrategy
{
    public override void OnTriggerStay(Collider other)
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

public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField] private MonsterType monsterType;
    [SerializeField] private SpawnStrategy spawnMonster;

    private void Start()
    {
        switch (monsterType)
        {
            case MonsterType.MushRoom:
                spawnMonster = new MushRoomSpawnStrategy();
                break;
            case MonsterType.Dragon:
                spawnMonster = new DragonSpawnStrategy();
                break;
        }
    }
}
