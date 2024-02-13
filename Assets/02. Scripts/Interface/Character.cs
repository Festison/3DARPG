using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDieable
{
    public void Die();
}

public interface IHitable
{
    public float Hp { get; set; }
    public void Hit(IAttackable attackable);
}

public interface IAttackable
{
    public float Damage { get; set; }
    public void Attack(IHitable hitable);
}

public abstract class Character : MonoBehaviour { }
