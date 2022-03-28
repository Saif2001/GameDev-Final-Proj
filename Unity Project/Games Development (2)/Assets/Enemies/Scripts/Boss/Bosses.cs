using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bosses : MonoBehaviour
{

    //Necessary transforms to follow player and instantiate prefabs
    public Transform Player;
    public Transform Boss_BulletSpawn;
    public Transform Boss;

    //Animator for AoE Effect
    public Animator AoEAnim;


    //Enemy damage and health characteristics
    private float reloadTime = 3f;
    public float bulletForce = 50f;
    public float health = 1000f;

    //Prefabs
    public GameObject Enemy_Bullet;
    public GameObject AoEArch;

    //AoE variables
    private float AoEReload = 2f;
    public bool playerinAoE;


    // Update is called once per frame
    void Update()
    {

        //Follow the player (lookAt)
        transform.LookAt(new Vector3(Player.position.x, Player.position.y, Player.position.z));

        //Fire bullets and perform AoE attack at intervals
        enemyFire();
        AoEAttack();


        //Boss death; disable UI elements and mesh
        if (health <= 0)
        {
            Destroy(Boss);
            GameObject.Find("Boss_Present").SetActive(false);
            GameObject.Find("Block_Backward").SetActive(false);
            GameObject.Find("Block_Forward").SetActive(false);
            GameObject.Find("BossHealthCanvas").SetActive(false);
        }

    }



    void enemyFire()
    {

        Vector3 bulletDirection = Player.position - Boss_BulletSpawn.position;          //Position vector between Empty prefab and player location

        reloadTime -= Time.deltaTime;
        if (reloadTime <= 0)
        {                
            
            //Decrement time and only allow another shot if enough time has passed
            GameObject enemyBullet = Instantiate(Enemy_Bullet, Boss_BulletSpawn.position, Quaternion.identity);     //Spawn enemy's bullet
            enemyBullet.transform.forward = bulletDirection.normalized;                                             //Set forward direction as position vector to player
            enemyBullet.GetComponent<Rigidbody>().AddForce(bulletDirection.normalized * bulletForce, ForceMode.Impulse);        //Add force to imitate bullet

            reloadTime = 3f;
        }
    }

 
    void AoEAttack() 
    {
  
        //Decrement time for use in reload
        AoEReload -= Time.deltaTime;

        if (AoEReload <= 0f)
        {
            //Reset reload time and begin animation component
            AoEReload = 2f;
            AoEAnim.SetTrigger("AoETrigger");

            //Damage player if in AoE killzone
            if (GameObject.Find("AoEShape").GetComponent<AoEAttack>().PlayerinKillzone == true)
            {
                GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health -= 30;
            }



        }

    }

}
