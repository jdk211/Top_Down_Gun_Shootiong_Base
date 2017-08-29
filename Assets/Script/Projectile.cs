using UnityEngine;

// 발사체
public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    private float speed = 10;
    private float damage = 1;

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

        if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    private void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(this.gameObject);
    }
}
