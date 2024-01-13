using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPortal : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
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
