using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bosses : MonoBehaviour
{

    public Transform Player;
    public Transform Boss_BulletSpawn;
    public Transform Boss;
    public Animator AoEAnim;




    private float reloadTime = 3f;
    public float bulletForce = 50f;
    public float health = 1000f;
    public GameObject Enemy_Bullet;
    public GameObject AoEArch;


    public bool readyToAttack = false;
    private float AoEReload = 2f;
    public bool playerinAoE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(Player.position.x, Player.position.y, Player.position.z));



        //enemyFire();
        readyToAttack = false;

        enemyFire();

        AoEAttack();

        if (health <= 0)
        {
            Destroy(Boss);          //Apparently Doesn't work
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
        {                //Decrement time and only allow another shot if enough time has passed
            GameObject enemyBullet = Instantiate(Enemy_Bullet, Boss_BulletSpawn.position, Quaternion.identity);     //Spawn enemy's bullet
            enemyBullet.transform.forward = bulletDirection.normalized;                                     //Set forward direction as position vector to player
            enemyBullet.GetComponent<Rigidbody>().AddForce(bulletDirection.normalized * bulletForce, ForceMode.Impulse);        //Add force to imitate bullet

            //Debug.Log("FIRING");
            reloadTime = 3f;
        }
    }

 
    void AoEAttack() 
    {
  

        //Debug.Log(Quaternion.Euler(Boss.rotation);
        AoEReload -= Time.deltaTime;
        //Debug.Log(AoEReload);
        //Debug.Log(AoEAnim.GetBool("AoETrigger"));

        if (AoEReload <= 0f)
        {

            AoEReload = 2f;
            AoEAnim.SetTrigger("AoETrigger");
            //GameObject AoEAttackArea = Instantiate(AoEArch, Boss.position + new Vector3(0, 13, 0), Quaternion.Euler(0, 0, 0));



            if (GameObject.Find("AoEShape").GetComponent<AoEAttack>().PlayerinKillzone == true)
            {
                GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health -= 30;
            }



        }

    }

}
