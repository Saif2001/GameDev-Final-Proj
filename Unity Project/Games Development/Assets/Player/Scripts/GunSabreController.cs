using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSabreController : MonoBehaviour
{

    public Animator animator;
    public Transform reticule;
    public Transform gunBarrel;
    public LayerMask enemyMask;

    public float gunRange = 10f;

    private bool inMeleeStance = true;
    private bool isAttacking = false;

    [SerializeField] private int charge = 5;
    private int maximumCharge = 10;

    void Start()
    {

    }

    private void LateUpdate()
    {
        animator.SetBool("inMeleeStance", inMeleeStance);
        animator.SetBool("isAttacking", isAttacking);
        
        // Ensure that the attack flag is cleared at the end of the update (after it has been used elsewhere)
        isAttacking = false;
    }

    public void ChangeStance()
    {
        inMeleeStance = !inMeleeStance;
    }

    public void Attack()
    {
        if (inMeleeStance)
        {
            // Perform melee attack
            isAttacking = true;
        }
        else
        {
            // Perform ranged attack
            if (charge > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(gunBarrel.position, reticule.position - gunBarrel.position, out hit, gunRange, enemyMask))
                {
                    hit.collider.SendMessage("GetHit");
                    Debug.Log("Hit target");
                }
                charge--;
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

    private void OnDrawGizmosSelected()
    {
        if (!inMeleeStance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(gunBarrel.position, (reticule.position - gunBarrel.position).normalized * gunRange);
        }
    }
}
