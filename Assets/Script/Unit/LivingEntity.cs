using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : ViewObject, IDamageable {

    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath; // 델리게이트 메소드

    protected virtual void Start()
    {
        health = startingHealth;
        dead = false;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // TODO : hit 변수로 어떤 처리들을 할 예정
        // hit 지점에 파티클을 생성하거나 ... etc
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")] // ? 동작 안되는거 같다
    public virtual void Die()
    {
        if(OnDeath != null)
        {
            OnDeath();
        }
    }
}
