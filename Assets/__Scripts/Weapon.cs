using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield power-up.
/// Items marked [NI] below are Not Implemented in the IGDPD book.
/// </summary>
public enum WeaponType
{
    none, // The default / no weapons
    blaster, // A simple blaster
    spread, // Two shots simultaneously
    missile, // [NI] Homing missiles
    laser, // [NI] Damage over time
    shield // Raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // Letter to show on the power-up
    public Color color = Color.white; // Color of Collar & power-up
    public GameObject projectilePrefab; // Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Amount of damage caused
    public float delayBetweenShots = 0;
    public float velocity = 20; // Speed of projectiles  
    public float trackingSpeed = 1.0f;
    public float laserScaleIncrease = 80.0f; // Y-scale increase for the laser projectile
    public float laserDamagePerSecond = 5.0f; // Damage per second for the laser
    public float laserDuration = 2.0f; // Duration of laser damage


}
public class Weapon : MonoBehaviour {
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired
    private Renderer collarRend;


    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Call SetType() for the default _type of WeaponType.none
        SetType(_type);

        // Dynamically create an anchor for all Projectiles
        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }

       

        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // You can fire immediately after _type is set.

        if (type == WeaponType.missile)
        {
            // Activate FindNearestEnemy when the missile type is set
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                MissileActivated(nearestEnemy.transform);
            }
        }

    }

    private float laserEffectTime; // Time when the laser effect should end

    public void Fire()
    {
        Debug.Log("Weapon Fired:" + gameObject.name);
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                p = MakeProjectile(); // Make middle Projectile
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

            case WeaponType.missile:
                MissileActivated();  
                break;

            case WeaponType.laser:
                LaserActivated();
                break;
        }
    }

    private void MissileActivated()
    {
        // Find the nearest enemy and make the missile track it
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            MissileActivated(nearestEnemy.transform);
        }
    }

    private void MissileActivated(Transform target)
    {
        Projectile missile = MakeProjectile();
        missile.rigid.velocity = Vector3.up * def.velocity;

        if (target != null)
        {
            missile.isMissile = true;
            missile.target = target;
        }
    }



    private GameObject FindNearestEnemy()
    {
        

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = enemies[0];
            float nearestDistance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        return null;
    }

    private void LaserActivated()
    {
        // Activate the laser effect
        laserEffectTime = Time.time + def.laserDuration;
        Projectile laserProjectile = MakeProjectile();
        // Set the type for the laser projectile
        laserProjectile.type = WeaponType.laser;
        laserProjectile.transform.position = collar.transform.position;
        laserProjectile.transform.up = collar.transform.up;

        StartCoroutine(LaserEffect());
    }

    private IEnumerator LaserEffect()
    {
        float damagePerSecond = def.laserDamagePerSecond;
        float duration = def.laserDuration;

        // Get the laser projectile created by MakeProjectile()
        Projectile laserProjectile = PROJECTILE_ANCHOR.GetChild(PROJECTILE_ANCHOR.childCount - 1).GetComponent<Projectile>();

        while (Time.time < laserEffectTime)
        {
            // Apply continuous damage to enemies within the laser
            RaycastHit[] hits = Physics.RaycastAll(collar.transform.position, collar.transform.up, 100f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.ApplyContinuousDamage(damagePerSecond, duration);
                    }
                }
            }

            // Update the position of the laser projectile to match the collar
            laserProjectile.transform.position = collar.transform.position;

            // Extend the y scale of the laser projectile
            Vector3 newScale = laserProjectile.transform.localScale;
            newScale.y += def.laserScaleIncrease * Time.deltaTime;
            laserProjectile.transform.localScale = newScale;

            yield return null;
        }

        // Deactivate the laser effect
        ResetLaser();
    }

    private void ResetLaser()
    {
        // Reset the y scale of the laser projectile
        Projectile laserProjectile = PROJECTILE_ANCHOR.GetChild(PROJECTILE_ANCHOR.childCount - 1).GetComponent<Projectile>();
        Vector3 newScale = laserProjectile.transform.localScale;
        newScale.y = 1.0f;
        laserProjectile.transform.localScale = newScale;
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        
        go.transform.position = collar.transform.position;

     

        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        p.weaponDef = def;

        lastShotTime = Time.time;
        return p;
    }
}
