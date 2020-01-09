using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunScript : MonoBehaviour
{
    /// <summary>
    /// 
    /// 
    /// Logika strelby
    /// 
    /// 
    /// </summary>

    public AudioSource audioSource;
    public AudioClip shootSound;

    public enum GunType
    {
        SingleFire,
        SemiAuto,
        FullAuto
    }

    [SerializeField] private GunType gunType;
    [SerializeField] private float singleFireTimeout;
    [SerializeField] private float fireTimer;

    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private float soundIntensity = 20f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;

    [SerializeField] private int totalAmmo;
    [SerializeField] private int magazineSize;
    [SerializeField] private int loadedAmmo;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private bool canShoot;


    [SerializeField] private Text ammoText;
    [SerializeField] private Text fireModeTextField;
    public string fireModeTextMsg = "";

    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;

    private Collider[] enemies;

    public enum FireMode
    {
        SingleFire,
        SemiAutoFire,
        FullAutoFire
    }

    private int layerMask = ~(1 << 10); //ignpruje player layer

    [SerializeField] private FireMode fireMode;

    private float nextTimeToFire = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canShoot = true;
        loadedAmmo = magazineSize;
        ChangeFireMode();
    }
    void OnEnable()
    {
        ChangeFireMode();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (loadedAmmo <= 0)
        {
            canShoot = false;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (magazineSize <= totalAmmo && loadedAmmo != magazineSize)
            {
                StartCoroutine(Reload());
            }
        }

        ChangeFireMode();

        fireTimer += Time.deltaTime;
        if (fireTimer > singleFireTimeout + 1)
        {
            fireTimer = singleFireTimeout + 1;
        }

        switch (fireMode)
        {
            case FireMode.SingleFire:
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (canShoot && fireTimer >= singleFireTimeout)
                    {
                        fireTimer = 0;
                        Shoot();
                    }
                }
                break;
            case FireMode.SemiAutoFire:
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (canShoot)
                        Shoot();
                }
                break;
            case FireMode.FullAutoFire:
                
                if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
                {
                    if (canShoot)
                    {
                        nextTimeToFire = Time.time + 1f / fireRate;
                        Shoot();
                    }
                }
                break;
        }
        UpdateAmmoText();
    }
    void ChangeFireMode()
    {
        int currentFireMode = (int)fireMode;
        switch (gunType)
        {
            case GunType.SingleFire: //ma timeout na kazdou strelu
                currentFireMode = 0;
                break;
            case GunType.SemiAuto: //nema timeout
                currentFireMode = 1;
                break;
            case GunType.FullAuto:
                if(currentFireMode == 0) //pokud je firemode single zmen ho na semiauto
                    currentFireMode = 1;
                if (Input.GetKeyDown(KeyCode.B)) //pri zmacknuti zmen mod u fullauto zbrane
                    currentFireMode = currentFireMode == 2 ? 1 : currentFireMode + 1;
                break;
        }
        fireMode = (FireMode)currentFireMode;
        UpdateFireModeText();
    }
    private IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
        loadedAmmo = magazineSize;
        totalAmmo -= magazineSize;
    }
    private void Shoot()
    {
        muzzleFlash.Play();
        audioSource.PlayOneShot(shootSound);

        loadedAmmo--;

        Collider[] enemies = Physics.OverlapSphere(fpsCam.transform.position, soundIntensity, enemyLayer);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponentInParent<EnemyController>().OnDetection();
        }
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layerMask))
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.tag == "Torso")
            {
                EnemyController enemy = hit.transform.GetComponentInParent<EnemyController>();
                enemy.TakeDamage(damage);
            }
            else if (hit.collider.tag == "Head")
            {
                EnemyController enemy = hit.transform.GetComponentInParent<EnemyController>();
                enemy.TakeDamage((float)(damage * 1.5));
            }
            else if (hit.collider.tag == "Limb")
            {
                EnemyController enemy = hit.transform.GetComponentInParent<EnemyController>();
                enemy.TakeDamage((float)(damage * 0.25));
            }
        }
        //GameObject impactObject = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        //Destroy(impactObject, 2f);
    }
    
   
    private void UpdateAmmoText()
    {
        ammoText.text = loadedAmmo + " / " + totalAmmo;


    }
    private void UpdateFireModeText()
    {
        switch (fireMode){
            case FireMode.SingleFire:
                fireModeTextMsg = "Single";
                break;
            case FireMode.SemiAutoFire:
                fireModeTextMsg = "Semi Auto";
                break;
            case FireMode.FullAutoFire:
                fireModeTextMsg = "Full Auto";
                break;
        }
        fireModeTextField.text = fireModeTextMsg;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fpsCam.transform.position, soundIntensity);
    }
}
