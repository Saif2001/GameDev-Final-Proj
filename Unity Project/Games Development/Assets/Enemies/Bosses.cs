using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bosses : MonoBehaviour
{

    public Transform Player;
    public Transform Boss_BulletSpawn;

    private float reloadTime = 3f;
    public float bulletForce = 50f;
    public GameObject Enemy_Bullet;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(Player.position.x, Player.position.y, Player.position.z));
        enemyFire();

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
}
