using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {

    public enum FireMode { Auto, Burst, Single}
    public FireMode firemode;

    public Transform[] projectileSpawn; // 발사 위치
    public Projectile projectile;
    public float msBetweenShots = 100; // ms
    public float muzzleVelocity = 35;
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = .3f;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f); // x min, y max
    public Vector2 recoilAngleMinMax = new Vector2(3, 5); // x min, y max
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    MuzzleFlash muzzleflash;
    private float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurdst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurdst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    private void LateUpdate()
    {
        // animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * -recoilAngle;

        if(!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    void Shoot()
    {
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {
            if(firemode == FireMode.Burst)
            {
                if(shotsRemainingInBurdst == 0)
                {
                    return;
                }

                shotsRemainingInBurdst--;
            }
            else if(firemode == FireMode.Single)
            {
                if(!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            
            for(int i = 0; i < projectileSpawn.Length; i++)
            {
                if(projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
            // 반동
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload()
    {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine("AnimateReload");
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;

        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // 보간
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle; 

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }


    public void Aim(Vector3 aimPoint)
    {
        if(!isReloading)
            transform.LookAt(aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurdst = burstCount;
    }
}
