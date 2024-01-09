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
    public float Damage { get; }
    public void Attack(IHitable hitable);
}

public class Character : MonoBehaviour
{
    
}
