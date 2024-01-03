using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField] private string monsterName;
    public string ZombieName { get { return monsterName; } }

    [SerializeField] private int hp;
    public int Hp { get { return hp; } }

    [SerializeField] private int damage;
    public int Damage { get { return damage; } }

    [SerializeField] private float sightRange;
    public float SightRange { get { return sightRange; } }

    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
}
