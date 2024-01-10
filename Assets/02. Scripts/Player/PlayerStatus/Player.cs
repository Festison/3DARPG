using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Player : Character,IHitable,IDieable
{

    [SerializeField] private float hp=500;
    [SerializeField] private float maxHp=500;
    [SerializeField] private float lerpSpeed = 10;

    public float Hp { get => hp;
        set
        {
            hp = value;

            if (hp<=0)
            {
                Die();
            }
        }
        }

    private void Update()
    {
        PlayerHpLerp();
    }
    

    public void Die()
    {
        
    }

    public void PlayerHpLerp()
    {
        UIManager.Instance.playerHpImg.fillAmount = Mathf.Lerp(UIManager.Instance.playerHpImg.fillAmount, Hp / maxHp, Time.deltaTime * lerpSpeed);
        UIManager.Instance.playerHpText.text = (Hp + " / " + maxHp);
    }

    public void Hit(IAttackable attackable)
    {
        Hp -= attackable.Damage;
    }
}
