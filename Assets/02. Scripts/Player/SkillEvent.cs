using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEvent : MonoBehaviour
{
    [SerializeField] private GameObject slashEffect;

    private void OnSlashEffect()
    {
        slashEffect.SetActive(true);
    }

    private void OffSlashEffect()
    {
        slashEffect.SetActive(false);
    }

}
