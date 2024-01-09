using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRayCast : MonoBehaviour, IAttackable
{
    private bool canDealDamage;
    [SerializeField] private List<GameObject> hasDealtDamage;

    [SerializeField] private float weaponLength;
    [SerializeField] private float weaponDamage;

    public float Damage => weaponDamage;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
        //if (canDealDamage)
        //{
        //    RaycastHit hit;

        //    int layerMask = 1 << 9;

        //    if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
        //    {
        //        if (hit.transform.TryGetComponent(out Monster monster) && !hasDealtDamage.Contains(hit.transform.gameObject))
        //        {
        //            monster.Hit(this);
        //            hasDealtDamage.Add(hit.transform.gameObject);
        //        }
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster) && canDealDamage)
            monster.Hit(this);
    }
    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }
    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }

    public void Attack(IHitable hitable)
    {
        hitable.Hp -= Damage;
    }
}
