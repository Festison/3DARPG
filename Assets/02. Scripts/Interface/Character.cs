using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    public float Hp { get; set; }
    public void Hit(IAttackable attackable);
}

public interface IAttackable
{
    public float Damage { get; }
    public void Attack(IHitable hitable);
}

public class Character : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float damage;

    public float Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
    
}
