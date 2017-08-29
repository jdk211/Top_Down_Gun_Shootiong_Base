using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform muzzle; // 발사 위치
    public Projectile projectile;
    public float msBetweenShots = 100; // ms
    public float muzzleVelocity = 35;

    private float nextShotTime;

    public void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);
        }
    }


}
