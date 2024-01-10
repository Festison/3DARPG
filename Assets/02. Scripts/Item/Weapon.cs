using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IAttackable
{
    private bool canDealDamage=false;
    [SerializeField] private float weaponDamage;
    [SerializeField] private BoxCollider weaponAtkRange;

    public float Damage { get => weaponDamage; set => weaponDamage = value; }

    void Start()
    {
        weaponAtkRange = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster) && canDealDamage)
            monster.Hit(this);
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        weaponAtkRange.enabled = true;
    }
    public void EndDealDamage()
    {
        weaponAtkRange.enabled = false;
        canDealDamage = false;
    }

    public void Attack(IHitable hitable)
    {
        hitable.Hp -= Damage;
    }
}
