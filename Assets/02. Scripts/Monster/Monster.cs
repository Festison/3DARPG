using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character, IAttackable, IHitable, IDieable
{
    [SerializeField] [Tooltip("몬스터 스텟 SO")] private MonsterData monsterStatus;
    [SerializeField] private float hp;

    [Header("몬스터 UI")]
    [SerializeField] private Image HpBarImage;
    [SerializeField] private float lerpSpeed = 10;

    [Header("몬스터 상태")]
    [SerializeField] private bool isAttack;

    private Animator monAnimator;
    [SerializeField] private GameObject hitEffect;

    public float Damage { get => monsterStatus.Damage; set => monsterStatus.Damage = value; }

    public float Hp
    {
        get => hp;
        set
        {
            hp = value;

            if (hp <= 0)
            {
                Die();
            }
        }
    }

    public float MaxHp { get => monsterStatus.MaxHp; set => monsterStatus.MaxHp = value; }

    private int xPos;
    private int zPos;
    private void OnEnable()
    {
        xPos = Random.Range(1, 20);
        zPos = Random.Range(1, 20);
        transform.position = new Vector3(xPos, 0, zPos);
    }

    private void Start()
    {
        monAnimator = GetComponent<Animator>();
        hp = monsterStatus.Hp;
    }

    private void Update()
    {
        HpLerp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
            player.Hit(this);
    }

    public void OnMonsterAttack()
    {
        isAttack = true;
    }

    public void OnMonsterAttackEnd()
    {
        isAttack = false;
    }

    public void HpLerp()
    {
        HpBarImage.fillAmount = Mathf.Lerp(HpBarImage.fillAmount, Hp / MaxHp, Time.deltaTime * lerpSpeed);
    }

    public void Hit(IAttackable attackable)
    {
        Hp -= attackable.Damage;
        HitVFX();
    }

    public void Hit(float Damage)
    {
        Hp -= Damage;
        HitVFX();
    }

    public void HitVFX()
    {
        Instantiate(hitEffect, transform.position + Vector3.up, Quaternion.identity);
    }

    public void Attack(IHitable hitable)
    {
        hitable.Hp -= Damage;
    }

    public void Die()
    {
        monAnimator.SetTrigger("Die");
        Destroy(this.gameObject, 1);
        MonsterObjPool.Instance.ReturnPool(this.gameObject);
    }

}