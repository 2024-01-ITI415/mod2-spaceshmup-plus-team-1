using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{

    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    public Vector3 direction = Vector3.down; // Default direction is down
    [SerializeField]
    private WeaponType _type;

    [Header("Set in Inspector")]
    public float speed = 20f; // Speed of the projectile

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
    }

    private void Update()
    {
        if (bndCheck != null && bndCheck.offUp)
        {
            Debug.Log("Projectile is off-screen. Destroying.");
            Destroy(gameObject);
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
        }
        // Move the projectile in the specified direction

    }

    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "Hero")
        {
            // Add logic to handle the hero's destruction or damage
            Destroy(otherGO); // Destroy the hero GameObject

            Destroy(gameObject); // Destroy the projectile
        }
    }
}