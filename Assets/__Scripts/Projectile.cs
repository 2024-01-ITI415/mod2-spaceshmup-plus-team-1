using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private BoundsCheck bndCheck;
    private Renderer rend;
    public WeaponDefinition weaponDef;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;
    public bool isMissile = false;
    public float trackingSpeed = 1.0f;
    public Transform target;
    private float laserDamage; // Damage per second for the laser


    // This public property masks the field _type and takes action when it is set
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
    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
        laserDamage = 0f;
    }

    private void Update()
    {
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }

        if (isMissile && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            rigid.velocity = direction * trackingSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, trackingSpeed * Time.deltaTime);
        }
    }

    

    ///<summary>
    /// Sets the _type private field and colors this projectile to match the
    /// WeaponDefinition.
    /// </summary>
    /// <param name="eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType)
    {
        // Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;

        if (type == WeaponType.missile)
        {
            trackingSpeed = def.trackingSpeed;
        }

        if (type == WeaponType.laser)
        {
            rend.material.color = def.projectileColor;
            laserDamage = def.laserDamagePerSecond;
        }

    }

    public void SetMissileTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
