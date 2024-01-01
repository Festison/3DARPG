using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject weaponSheath;
    [SerializeField] private GameObject weapon;

    private GameObject currentWeaponInHand;
    private GameObject currentWeaponInSheath;

    void Start()
    {
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
    }

    public void DrawWeapon()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
        Destroy(currentWeaponInSheath);
    }

    public void SheathWeapon()
    {
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        Destroy(currentWeaponInHand);
    }

    public void StartDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<AttackRayCast>().StartDealDamage();
    }
    public void EndDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<AttackRayCast>().EndDealDamage();
    }
}
