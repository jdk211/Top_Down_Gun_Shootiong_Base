using UnityEngine;

// 발사체
public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    public Color trailColor;
    private float speed = 10;
    private float damage = 1;

    private float lifeTime = 3;
    private float skingWidth = 0.1f; // 적이 다가오고 총알이 나아갈때 부딪히지않을 경우 보정값

    private void Start()
    {
        Destroy(this.gameObject, lifeTime);

        // 발사체와 겹쳐있는 모든 충돌체들의 배열
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Update () {
        float moveDistance = Time.deltaTime * speed;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);	
	}

    private void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + skingWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    //private void OnHitObject(RaycastHit hit)
    //{
    //    IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
    //    if(damageableObject != null)
    //    {
    //        damageableObject.TakeHit(damage, hit);
    //    }
    //    GameObject.Destroy(this.gameObject);
    //}

    private void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(this.gameObject);
    }
}
