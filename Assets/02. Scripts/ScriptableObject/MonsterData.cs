using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data", order = int.MaxValue)]
public class MonsterData : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField] private string monsterName;
    public string MonsterName { get { return monsterName; } }

    [SerializeField] private float hp;
    public float Hp { get { return hp; } set { hp = value; } }

    [SerializeField] private float maxhp;
    public float MaxHp { get { return maxhp; } set { maxhp = value; } }

    [SerializeField] private int damage;
    public float Damage { get { return damage; } }
    public void OnBeforeSerialize()
    {
        Hp = MaxHp;
    }

    public void OnAfterDeserialize()
    {
        
    }
}
    