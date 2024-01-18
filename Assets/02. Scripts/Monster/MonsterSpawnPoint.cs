using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpawnStrategy
{
    public enum MonsterType
    {
        MushRoom,
        Dragon
    }
    public static class Factory
    {
        public static SpawnStrategy Create(MonsterType monstertype)
        {
            switch (monstertype)
            {
                case MonsterType.MushRoom:
                    return new MushRoomSpawnStrategy();
                case MonsterType.Dragon:
                    return new DragonSpawnStrategy();
                default:
                    return null;
            }
        }
    }
}

[System.Serializable]
public class MushRoomSpawnStrategy : SpawnStrategy
{
   
}

[System.Serializable]
public class DragonSpawnStrategy : SpawnStrategy
{   
}

public abstract class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField] public SpawnStrategy spawnMonster;

    private void Start()
    {
        Init();
    }

    public abstract void Init();
}
