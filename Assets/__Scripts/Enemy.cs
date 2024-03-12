using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Set in Inspector: Enemy")]
    public float speed = 10f; // The speed in m/s
    public float fireRate = 0.3f; // Seconds/shot (Unused)
    public float health = 10;
    public int score = 100; // Points earned for destroying this
    public float showDamageDuration = 0.1f; // # seconds to show damage
    public float powerUpDropChance = 1f; // Chance to drop a power-up

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// All the Materials of this & its children
    public bool showingDamage = false;
    public float damageDoneTime; // Time to stop showing damage
    public bool notifiedOfDestruction = false; // Will be used later

    private float continuousDamage;
    private float continuousDamageDuration;
    protected BoundsCheck bndCheck;

    public void ApplyContinuousDamage(float damagePerSecond, float duration)
    {
        continuousDamage = damagePerSecond;
        continuousDamageDuration = duration;
        StartCoroutine(ContinuousDamageEffect());
    }

    private IEnumerator ContinuousDamageEffect()
    {
        float endTime = Time.time + continuousDamageDuration;

        while (Time.time < endTime)
        {
            health -= continuousDamage * Time.deltaTime;

            // Check if the enemy has been destroyed
            if (health <= 0)
            {
                HandleDestruction();
                break;
            }

            yield return null;
        }
    }

    private void HandleDestruction()
    {
        // Handle destruction logic here
        if (!notifiedOfDestruction)
        {
            Main.S.ShipDestroyed(this);
        }
        notifiedOfDestruction = true;

        // Destroy this enemy
        Destroy(gameObject);
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i=0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // This is a property: A method that acts like a field
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();

        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            // We're off the bottom, so destroy this GameObject
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // Hurt this Enemy
                ShowDamage();
                // Check if the projectile is a laser
                if (p.type == WeaponType.laser)
                {
                    // Apply continuous damage for the laser
                    ApplyContinuousDamage(Main.GetWeaponDefinition(p.type).laserDamagePerSecond, Main.GetWeaponDefinition(p.type).laserDuration);
                }
                else
                {
                    // Get the damage amount from the Main WEAP_DICT
                    health -= Main.GetWeaponDefinition(p.type).damageOnHit;


                    if (health <= 0)
                    {
                        // Tell the Main singleton that this ship was destroyed
                        if (!notifiedOfDestruction)
                        {
                            Main.S.ShipDestroyed(this);
                        }
                        notifiedOfDestruction = true;
                        // Destroy this enemy
                        Destroy(this.gameObject);
                    }
                }
                    Destroy(otherGO);
                    break;

                    default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                    break;
                }
    }

   

    void ShowDamage()
    {
       
            foreach (Material m in materials)
            {
                m.color = Color.red;
            }
        
            
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i=0; i<materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
