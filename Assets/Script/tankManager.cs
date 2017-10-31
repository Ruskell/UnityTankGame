using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tankManager : MonoBehaviour {

    public GameObject tankBullet, bulletPos;
    Animator anim;
    AudioSource audio;
    public SpriteRenderer sprite, turret;
    public Text power, angle, healthDisplay, fuelDisplay, health2Display;
    Rigidbody2D rigBody;
    public bool active; 
    public float speed;
    public float shotPower = 100;
    public float health = 100;
    public float fuel = 100;
    public bool ai;
    public int level = 4;
    static int stage;
    float bulletSize = 1, tankSize = 0.55f;
    System.Random rand = new System.Random();


    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        turret = GetComponentsInChildren<SpriteRenderer>()[1];
        rigBody = GetComponent<Rigidbody2D>();
        power = GameObject.Find("Power Display").GetComponent<Text>();
        angle = GameObject.Find("Angle Display").GetComponent<Text>();
        healthDisplay = GameObject.Find("Health Display").GetComponent<Text>();
        health2Display = GameObject.Find("Health Display 2").GetComponent<Text>();
        fuelDisplay = GameObject.Find("Fuel Display").GetComponent<Text>();
        speed = 1;
        active = false;
        print(level);
        if (PlayerPrefs.GetInt("level") != 0) level = PlayerPrefs.GetInt("level");
        audio = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {

        if (!active || stage == 2) StopAllCoroutines();
        //Test if active
        if (!active) audio.Stop();
        if (active && !ai)
        {
            //Left Movement Control
            if (Input.GetKey(KeyCode.LeftArrow) && fuel > 0)
            {
                if (!audio.isPlaying) audio.Play();
                anim.SetInteger("state", 1);
                fuel -= Time.deltaTime * 25;
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                anim.SetInteger("state", 0);
                audio.Stop();
            }

            //Right Movement Control
            if (Input.GetKey(KeyCode.RightArrow) && fuel > 0)
            {
                if (!audio.isPlaying) audio.Play();
                anim.SetInteger("state", 1);
                fuel -= Time.deltaTime * 25;
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                anim.SetInteger("state", 0);
                audio.Stop();
            }

            fuelDisplay.text = Convert.ToInt32(fuel).ToString();

            //Turret Control
            if (Input.GetKey(KeyCode.UpArrow)  && turret.transform.localRotation.eulerAngles.z < 45)
            {
                turret.transform.Rotate(Vector3.forward, 30 * Time.deltaTime);
                angle.text = Convert.ToInt32(turret.transform.localRotation.eulerAngles.z).ToString();
            }

            if (Input.GetKey(KeyCode.DownArrow) && turret.transform.localRotation.eulerAngles.z > 0.4)
            {
                turret.transform.Rotate(Vector3.back, 30 * Time.deltaTime);
                angle.text = Convert.ToInt32(turret.transform.localRotation.eulerAngles.z).ToString();
            }

            if (Input.GetKey(KeyCode.Space)){
                shotPower += Time.deltaTime * 200;
                if (shotPower > 400) shotPower = 400;
                power.text = Convert.ToInt32((shotPower - 100) / 3).ToString();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                GameObject go = Instantiate(tankBullet, bulletPos.transform.position, bulletPos.transform.rotation) as GameObject;
                if (turret.transform.rotation.eulerAngles.z > 180) go.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, (turret.transform.rotation.eulerAngles.z - 360) / 45) * shotPower);
                else go.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, turret.transform.rotation.eulerAngles.z / 45) * shotPower);
                go.transform.localScale = new Vector3(bulletSize, bulletSize, 1);
                shotPower = 100;
            }
        }
    }

    private void FixedUpdate()
    {
        if (active && !ai && fuel > 0) rigBody.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rigBody.velocity.y);
    }

    public void updateHealth(int amt)
    {
        health += amt;
        if (health < 0) health = 0;
        if (!ai)
        {
            healthDisplay.text = health.ToString();
            if (health == 0)
            {
                active = false;
                PlayerPrefs.DeleteAll();
                Destroy(gameObject);
                GameObject.Find("mainCam").GetComponent<mainMenu>().levelController(-1);
            }
        }
        if (ai)
        {
            health2Display.text = health.ToString();
            if (health == 0)
            {
                level--;
                active = false;
                Destroy(gameObject);
                PlayerPrefs.SetInt("level", level);
                GameObject.Find("mainCam").GetComponent<mainMenu>().levelController(level);
            }
        }
    }


    public void aiTurn()
    {
        StopAllCoroutines();

        // 0 none, 1 left, 2 right randomised movement
        System.Random rand = new System.Random();
        stage = 0;

        int movement = rand.Next(0, 5);
        double amount = rand.NextDouble() * 3;

        StartCoroutine(moveAI(movement, amount));
        StartCoroutine(turretAI());
        
        //StartCoroutine(shotAI(powerAI));
        //
    }

    IEnumerator moveAI(int move, double amt)
    {
        if (!active || stage != 0) yield return null;
        else
        {
            while (stage != 0) yield return new WaitForSeconds(0.5f);
            print("movement called: " + move);
            if (move == 1 || move == 2)
            {
                print("moving left");
                audio.Play();
                for (float i = 0; i < amt; i += Time.deltaTime)
                {
                    if (fuel > 0 && sprite.transform.position.x > 1 && sprite.transform.position.x < 8.64)
                    {
                        rigBody.velocity = new Vector2(-1 * speed, rigBody.velocity.y);
                        fuel -= Time.deltaTime * 25;
                    }
                    else if (sprite.transform.position.x < 1)
                    {
                        rigBody.velocity = new Vector2(1 * speed, rigBody.velocity.y);
                        break;
                    }
                    yield return new WaitForSeconds(0.01f);
                }
            }
            if (move == 3 || move == 4)
            {
                print("moving right");
                audio.Play();
                for (float i = 0; i < amt/2; i += Time.deltaTime)
                {
                    if (fuel > 0 && sprite.transform.position.x > 1 && sprite.transform.position.x < 8.64) {
                        rigBody.velocity = new Vector2(1 * speed, rigBody.velocity.y);
                        fuel -= Time.deltaTime * 25;
                    }
                    else if (sprite.transform.position.x > 8.64)
                    {
                        rigBody.velocity = new Vector2(-1 * speed, rigBody.velocity.y);
                        break;
                    }
                    yield return new WaitForSeconds(0.01f);
                }
            }
            audio.Stop();
            stage = 1;
            print("Move stage 1 set");
        }
    }

    IEnumerator turretAI()
    {
        while (stage != 1)
        {
            print("stage: " + stage + "    waiting for stage1");
            yield return new WaitForSeconds(0.5f);
        }
        if (stage ==  1)
        {

            print("shoot stage 1 reached");
            Launcher2D L2D = GetComponent<Launcher2D>();
            Vector3 playerPos = GameObject.FindGameObjectWithTag("tankLeft").transform.position;

            GameObject tankRight = GameObject.FindGameObjectWithTag("tankRight");
            Vector3 enemyPos = tankRight.transform.position;

            TrajectoryPredictor tp = tankRight.GetComponent<TrajectoryPredictor>();
            tp.Predict2D(L2D.launchPoint.position, L2D.launchPoint.right * L2D.force, Physics2D.gravity);

            RaycastHit2D hit = tp.hitInfo2D;

            float xMax = Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x);
            
            while (hit.point.x > playerPos.x && xMax <= Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x))
            {
                yield return new WaitForSeconds(0.02f);
                
                turret.transform.Rotate(Vector3.forward, Time.deltaTime * 30);
                tp.Predict2D(L2D.launchPoint.position, L2D.launchPoint.right * L2D.force, Physics2D.gravity);
                hit = tp.hitInfo2D;

                if(xMax <= Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x)) xMax = Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x);
            }

            float xMin = Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x);
            while (hit.point.x > playerPos.x && xMin <= Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x))
            {
                yield return new WaitForSeconds(0.02f);

                turret.transform.Rotate(Vector3.back, Time.deltaTime * 30);
                tp.Predict2D(L2D.launchPoint.position, L2D.launchPoint.right * L2D.force, Physics2D.gravity);
                hit = tp.hitInfo2D;

                if (xMin <= Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x)) xMin = Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x);
            }

            xMax = Mathf.Abs((new Vector2(enemyPos.x, enemyPos.y) - hit.point).x);
            while (hit.point.x < playerPos.x )
            {
                yield return new WaitForSeconds(0.02f);
                
                turret.transform.Rotate(Vector3.back, Time.deltaTime * 30);

                tp.Predict2D(L2D.launchPoint.position, L2D.launchPoint.right * L2D.force, Physics2D.gravity);
                hit = tp.hitInfo2D;
            }

            float extraForce = rand.Next(level * -1, level) / 1.8f;
            print("extraForce = " + extraForce);
            L2D.force += extraForce;
            L2D.Launch();
            stage = 2;
            L2D.Reset();
            StopAllCoroutines();

            print("shot fired, method ending");
        }
    }
    

    private void OnCollisionEnter2D(Collision2D col)
    {
        print("collision called on obj = " + col.gameObject.tag);
        if (col.gameObject.tag == "careDrop")
        {
            int val = col.gameObject.GetComponent<dropScript>().dropValue;
            if (val == 0) val = rand.Next(1, 6);
            if (val == 1)
            {
                bulletSize -= 0.3f;
                if (bulletSize < 0.4) bulletSize = 0.4f;
            }
            else if (val == 2)
            {
                bulletSize += 0.3f;
                if (bulletSize > 2) bulletSize = 2;
            }
            else if (val == 3) updateHealth(50);
            else if (val == 4)
            {
                tankSize -= 0.4f;
                if (tankSize < 0.25) tankSize = 0.25f;
                updateTankSize();
            }
            else if (val == 5)
            {
                tankSize += 0.4f;
                if (tankSize > 2.5) tankSize = 2.5f;
                updateTankSize();
            }
            Destroy(col.gameObject);
        }
        else return;

    }

    private void updateTankSize()
    {
        transform.localScale = new Vector3(tankSize,tankSize, 1);
    }
}
