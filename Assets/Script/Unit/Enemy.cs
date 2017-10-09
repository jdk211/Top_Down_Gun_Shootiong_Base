using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking };
    State currentState;

    public ParticleSystem deathEffect;
    ParticleSystem.MainModule effectSetting;

    public static event System.Action OnDeathStatic;

    Transform myTrans;
    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Color originalColor;
    Spawner spawner;

    float attackDistanceThreshold = 0.5f; // 공격 거리 임계값 , 유니티에서 단위 1 = 1 meter 이다
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        myTrans = GetComponent<Transform>();
        pathfinder = GetComponent<NavMeshAgent>();
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();
        OnDeath += spawner.OnEnemyDeath;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }

        effectSetting = deathEffect.main;
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();

        if (hasTarget)
        {
            currentState = State.Chasing;

            targetEntity.OnDeath += OnTargetDeath;
        }
    }

    public void SetCharacteristics(Vector3 startPos, float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        myTrans.position = startPos;
        currentState = State.Chasing;
        pathfinder.enabled = true;
        pathfinder.speed = moveSpeed;

        if(hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }

        startingHealth = enemyHealth;

        effectSetting.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;

        StartCoroutine("UpdatePath");
    }

    private void Update()
    {
        if(hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine("Attack");
                }
            }
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if(damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject,
                                            effectSetting.startLifetime.constant); 
            //그냥 ParticleSystem에 있는 startLifeTime은 사용할수없음
            //ParticleSystem.MainModule을 사용해서 사용
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * myCollisionRadius;

        float attackSpeed = 3;
        float percent = 0; // 0 -> 1 , 1 -> 0 으로 변경시켜서 찌르고 다시 돌아오게함

        skinMaterial.color = Color.red;

        bool hasAppliedDamage = false;

        while(percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            // 돌아오게 대칭함수를 이용,  y = 4(-x^2 + x) 같은 함수

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // 보간
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        // 1초에 4번 경로를 재 계산
        float refreshRate = 0.25f;

        while(hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }

    public override void Die()
    {
        ObjectPool.Instance().ReturnObject(this);
        base.Die();
    }

    /* 비싼 처리 비용
	void Update () {
        // 매 프레임 업데이트하면 매우 비싼 처리를 요구
        // 초당 60프레임이라면 1초에 60번을 계산
        //pathfinder.SetDestination(target.position);

	}
    */
}
