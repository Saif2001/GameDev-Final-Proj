using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A script to control the player's GunSabre
 */

public class GunSabreController : MonoBehaviour
{

    public Animator animator;
    public Transform reticule;
    public Transform gunBarrel;
    public LayerMask enemyMask;
    public RangedAttack projectilePrefab;

    public float gunRange = 10f;

    private bool inMeleeStance = true;
    private bool isAttacking = false;

    public int charge = 5;
    private int maximumCharge = 10;

    public int bladeDamage = 50;
    public int gunDamage = 80;

    //public float bossHealth = GameObject.Find("Boss_Present").GetComponent<Bosses>().health;

    void Start()
    {

    }

    // Update animations
    private void LateUpdate()
    {
        animator.SetBool("inMeleeStance", inMeleeStance);
        animator.SetBool("isAttacking", isAttacking);
        
        // Ensure that the attack flag is cleared at the end of the update (after it has been used elsewhere)
        isAttacking = false;
    }

    // A function to chage the current gunsabre stance from an external script (PlayerController)
    public void ChangeStance()
    {
        inMeleeStance = !inMeleeStance;
    }

    // A function to attack with the gunsabre. Called from PlayerController
    public void Attack()
    {
        if (inMeleeStance)
        {
            // Perform melee attack (this is done using an animation to swing a collider)
            isAttacking = true;
        }
        else
        {
            // Perform ranged attack
            if (charge > 0)
            {
                RaycastHit hit;
                RaycastHit hitBoss;
                if (Physics.SphereCast(gunBarrel.position, 1f, reticule.position - gunBarrel.position, out hit, gunRange, enemyMask))
                {
                    // If an enemy is hit, call the GetHit function within the enemy controller
                    hit.collider.SendMessage("GetHit");
                    Debug.Log("Hit target");
                }

                //Damage kills enemies in one shot. This is an alternative function for decrementing boss health
                if (Physics.Raycast(gunBarrel.position, reticule.position - gunBarrel.position, out hitBoss, gunRange))
                {
                    Debug.Log("Hit Boss");
                    GameObject.Find("Boss_Present").GetComponent<Bosses>().health -= gunDamage;
                    GameObject.Find("Boss_Past").GetComponent<Bosses>().health -= gunDamage;
                }
                // Decrement the charge after each shot
                charge--;
                // Update the animator bool
                isAttacking = true;
            }
        }
    }

    // Triggers whenever the collider on the gun sabre hits another collider
    private void OnTriggerEnter(Collider hit)
    {
        // If in melee stance and the hit object is an enemy
        if (inMeleeStance)
        {
            if (hit.tag == "Enemy")
            {
                // Damage the enemy
                hit.SendMessage("GetHit");
                // Increase the gunsabre charge
                if (charge < maximumCharge)
                    charge++;
            }
        }
    }

    // Gizmos for debugging purposes
    private void OnDrawGizmosSelected()
    {
        if (!inMeleeStance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(gunBarrel.position, (reticule.position - gunBarrel.position).normalized * gunRange);
        }
    }
}
