using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    NavMeshAgent pathfinder;
    Transform target;

    float attackDistanceThreshold = 1.5f; // 공격 거리 임계값 , 유니티에서 단위 1 = 1 meter 이다

    // Use this for initialization
    protected override void Start () {
        base.Start();

        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }

	/*
	void Update () {
        // 매 프레임 업데이트하면 매우 비싼 처리를 요구
        // 초당 60프레임이라면 1초에 60번을 계산
        //pathfinder.SetDestination(target.position);

	}
    */

    IEnumerator UpdatePath()
    {
        // 1초에 4번 경로를 재 계산
        float refreshRate = 0.25f;

        while(target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

            if (!dead)
            {
                pathfinder.SetDestination(targetPosition);
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
