# 05-Space-SHMUP
 
Sierra Conroy

PRE-TUNED VARIABLES: 
                 
   (Blaster containing only     case WeaponType.blaster:
    one projectile)             p = MakeProjectile();
                                p.rigid.velocity = vel;
                                break;
--
                                 case WeaponType.spread:     
    (Spread contains             p = MakeProjectile(); // Make middle Projectile
    only three projectiles)      p = MakeProjectile(); // Make right Projectile
                                 p = MakeProjectile(); // Make left Projectile
                     
--
                            public float damageOnHit = 0; // Amount of damage caused
   (Original                public float continuousDamage = 0; // Damage per second (Laser)
    damage variables)       public float velocity = 20; // Speed of projectiles                     
                     


POST-TUNED VARIABLES:
         
   (Added two projectiles         p = MakeProjectile();
   to Blaster = 3 total)          p.rigid.velocity = vel;

                                  p = MakeProjectile();
                                  p.transform.rotation = Quaternion.AngleAxis(-50, Vector3.back);
                                  p.rigid.velocity = p.transform.rotation * vel;

                                  p = MakeProjectile();
                                  p.transform.rotation = Quaternion.AngleAxis(50, Vector3.back);
                                  p.rigid.velocity = p.transform.rotation * vel;

--

 (Added two projectiles to Spread      case WeaponType.spread:     
        = 5 total)                     p = MakeProjectile(); // Make middle Projectile
                                       p = MakeProjectile(); // Make right Projectile
                                       p = MakeProjectile(); // Make left Projectile
   
                                       p = MakeProjectile(); // Make another Projectile
                                       p.transform.rotation = Quaternion.AngleAxis(-15, Vector3.back);
                                       p.rigid.velocity = p.transform.rotation * vel;

                                       p = MakeProjectile(); // Make another Projectile
                                       p.transform.rotation = Quaternion.AngleAxis(15, Vector3.back);
                                       p.rigid.velocity = p.transform.rotation * vel;
--

                            public float damageOnHit = 20; // Amount of damage caused
   (Changed                 public float continuousDamage = 20; // Damage per second (Laser)
    damage variables)       public float velocity = 25; // Speed of projectiles   
 

TWO ADDED ELEMENTS:


         
   