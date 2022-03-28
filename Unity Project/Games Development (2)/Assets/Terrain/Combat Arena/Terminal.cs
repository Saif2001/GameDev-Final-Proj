using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *  A script to control the terminal used to charge the teleporter
 */

public class Terminal : MonoBehaviour
{
    [SerializeField] private float charge = 0f;
    public float chargeRequired = 5f;

    public ArenaController arena;
    public Teleporter teleporter;
    public Text chargeMessage;

    // Called from the player controller if the terminal is interacted with
    public void Interact(GunSabreController gunsabre)
    {
        // If the gunsabre has no charge, then the terminal cannot be charged
        if (gunsabre.charge <= 0)
        {
            chargeMessage.text = "Insufficient Charge";
            chargeMessage.enabled = true;
            Invoke("DisableText", 3f);
            return;
        }

        // Charge the terminal with gunsabre charge until either the gunsabre runs out of charge or the terminal is fully charged
        while (gunsabre.charge > 0)
        {
            // Drain gunsabre charge and charge the terminal by one point each per loop
            gunsabre.charge--;
            charge++;

            // If the terminal is fully charged, unlpck the teleporter and mark the arena as complete to stop enemies spawning
            if (charge >= chargeRequired)
            {
                teleporter.Unlock();
                arena.Complete();
                chargeMessage.text = "Teleporter Fully Charged";
                chargeMessage.enabled = true;
                Invoke("DisableText", 3f);
                return;
            }
        }
        // Briefly display the current terminal charge on the UI
        chargeMessage.text = "Charge Collected: " + charge + "/10";
        chargeMessage.enabled = true;
        Invoke("DisableText", 3f);

    }

    private void DisableText()
    {
        chargeMessage.enabled = false;
    }
}
