using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unitycoding.UIWidgets;
using UnityEngine.Events;

public class mainMenu : MonoBehaviour {
    bool bTurn;
    bool gameState = false;
    public bool resetData = false;
    public bool reloadLevel = false;
    static bool instantiated { get; set; }
    int levelTest = 0;
    public int infoStage = -1;
    tankManager tankA, tankB;
    Text timer;
    float timeLeft = 30;
    CanvasGroup playerUI;
    Text playerID, playerTurn, enemyID, enemyTurn;
    public GameObject pubCareDrop;
    public MessageBox messageBox;

    // Use this for initialization
    void Start () {
        StopAllCoroutines();
        timer = GameObject.Find("timeLeft").GetComponent<Text>();
        System.Random rand = new System.Random();
        int randTurn = rand.Next(0, 2);
        if (randTurn == 0) bTurn = false;
        else bTurn = true;
        tankA = GameObject.FindGameObjectWithTag("tankLeft").GetComponent<tankManager>();
        tankB = GameObject.FindGameObjectWithTag("tankRight").GetComponent<tankManager>();
        playerUI = GameObject.Find("playerUI").GetComponent<CanvasGroup>();
        playerID = GameObject.Find("Player").GetComponent<Text>();
        playerTurn = GameObject.Find("Player Turn").GetComponent<Text>();
        enemyID = GameObject.Find("Enemy").GetComponent<Text>();
        enemyTurn = GameObject.Find("Enemy Turn").GetComponent<Text>();

        playerUI.alpha = 0f;
        GameObject.Find("gameOver").GetComponent<Image>().CrossFadeAlpha(0, 0.01f, false);
        GameObject.Find("restart").GetComponent<Image>().CrossFadeAlpha(0, 0.01f, false);
        GameObject.Find("restart").GetComponent<Button>().interactable = false;

        if(PlayerPrefs.GetInt("level") == 1)
        {
            GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("ArcadeLoginScreenIntro");
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().volume = 0.25f;

            GetComponent<AudioSource>().Play();
        }

        if (PlayerPrefs.GetInt("level") != 0)
        {
            print(PlayerPrefs.GetInt("level"));
            GameObject.Find("startUI").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("startUI").GetComponent<CanvasGroup>().interactable = false;
            if (!gameState) StartCoroutine(fadeOut(true));
            StartCoroutine(waitStartGame());
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (resetData)
        {
            PlayerPrefs.DeleteAll();
            print("All data deleted");
            resetData = false;
        }
        if(reloadLevel){
            reloadLevel = false;
            print("Level Reloaded");
            SceneManager.LoadScene("testLevel");
        }

        if (gameState && Input.GetButtonDown("Cancel"))
        {
            endGame();
        }
        if (gameState)
        {
            timeLeft -= Time.deltaTime;
            timer.text = Math.Round(timeLeft, 0).ToString();
        }
        if (timeLeft < 0.2) swapTurn();
    }

    public void onStartClick()
    {
        GameObject.FindGameObjectWithTag("playButton").GetComponent<Button>().enabled = false;
        GameObject.Find("Info").GetComponent<Button>().enabled = false;
        GameObject.Find("Info").GetComponent<FadeUI>().fadeUIout();
        GameObject.Find("Exit").GetComponent<Button>().enabled = false;
        GameObject.Find("Exit").GetComponent<FadeUI>().fadeUIout();
        if (!gameState) StartCoroutine(fadeOut(true));
        StartCoroutine(waitStartGame());
    }

    IEnumerator waitStartGame()
    {
        yield return new WaitForSeconds(6f);
        swapTurn();
    }
    
    public void onEndClick()
    {
        print("onEndClick Invoked");
        //Initiate.Fade("testLevel", Color.black, 2.0f);
        Application.Quit();
    }

    public void onInfoClick()
    {
        StartCoroutine(messageWaiter());
    }

    IEnumerator messageWaiter()
    {
        string[] options = new string[2];
        options[0] = "Affirmative";
        options[1] = "Negative";
        Button play = GameObject.Find("Play").GetComponent<Button>();
        Button exit = GameObject.Find("Exit").GetComponent<Button>();
        Button info = GameObject.Find("Info").GetComponent<Button>();

        play.enabled = false;
        exit.enabled = false;
        info.enabled = false;

        if (infoStage == 0)
        {
            messageBox.Show("Game Info", "Batallion Blitz is a turn-based artillery shooter game, where the player is pitted against an ever increasingly difficult AI tank.", msgCallback, options);
            while (infoStage == 0) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 1)
        {
            messageBox.Show("Game Info", "Use your turret angle and projectile power to blast the enemy to smithereens, but be aware of your positioning as the enemy has a greater chance of hitting you while you are exposed.", msgCallback, options);
            while (infoStage == 1) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 2)
        {
            messageBox.Show("Game Info", "The enemy will get harder and harder throughout each of the three levels so use your fuel each turn to try and collect the supply drops to give your tank the advantage.", null, msgCallback, options);
            while (infoStage == 2) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 3)
        {
            messageBox.Show("Controls", "Use UP and DOWN to control your turret angle.\n\nHold SPACE to power up your shot, release to fire.", null, msgCallback, options);
            while (infoStage == 3) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 4)
        {
            messageBox.Show("Supply Drops", "This is the Health Boost Supply Drop\n\nIt will raise your HP by 50", Resources.Load<Sprite>("healthUp"), msgCallback, options);
            while (infoStage == 4) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 5)
        {
            messageBox.Show("Supply Drops", "This is the Size Increase Supply Drop\n\nIt will raise your tank size", Resources.Load<Sprite>("sizeUp"), msgCallback, options);
            while (infoStage == 5) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 6)
        {
            messageBox.Show("Supply Drops", "This is the Size Decrease Supply Drop\n\nIt will lower your tank size", Resources.Load<Sprite>("sizeDown"), msgCallback, options);
            while (infoStage == 6) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 7)
        {
            messageBox.Show("Supply Drops", "This is the Bullet Enhance Supply Drop\n\nIt will raise your bullet size", Resources.Load<Sprite>("bulletUp"), msgCallback, options);
            while (infoStage == 7) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 8)
        {
            messageBox.Show("Supply Drops", "This is the Bullet Diminish Supply Drop\n\nIt will lower your bullet size", Resources.Load<Sprite>("bulletDown"), msgCallback, options);
            while (infoStage == 8) yield return new WaitForSeconds(0.3f);
        }

        if (infoStage == 9) messageBox.Show("Supply Drops", "This is the Random Supply Drop\n\nIt holds one of the other 5 supply drops inside", Resources.Load<Sprite>("randomBox"), msgCallback, options);
        infoStage = 0;
        play.enabled = true;
        info.enabled = true;
        exit.enabled = true;
    }

    public void msgCallback(string result)
    {
        if (result == "Negative") infoStage = -1;
        else infoStage++;
    }

    //Fade UI out
    IEnumerator fadeOut(bool bFade)
    {
        if (bFade)
        {

            yield return new WaitForSeconds(2);
            for (float i = 4; i >= 0; i -= Time.deltaTime)
            {
                if (GetComponent<BlurOptimized>().blurSize > 0) GetComponent<BlurOptimized>().blurSize -= 0.05f;
                else if (GetComponent<BlurOptimized>().blurSize < 0) GetComponent<BlurOptimized>().enabled = false;

                if (transform.position.y > 0) transform.position = new Vector3(0, transform.position.y - 0.007f, -10);
                else if (transform.position.y < 0) transform.position = new Vector3(0, 0, -10);

                if (Camera.main.orthographicSize < 5) Camera.main.orthographicSize += 0.012f;
                else if (Camera.main.orthographicSize > 5) Camera.main.orthographicSize = 5;

                if(Camera.main.orthographicSize > 4.5 && playerUI.alpha < 1) playerUI.alpha += 0.01f;

                yield return null;
            }
        }
    }


    //Turn Controller
    public void swapTurn()
    {
        timeLeft = 30f;
        gameState = true;
        print("swapTurn Invoked");
        if (bTurn)
        {
            tankA.active = true;
            tankA.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            tankA.GetComponent<tankManager>().fuel = 100;
            playerID.color = new Color(0, 0.1764f, 0.6863f);
            playerTurn.color = new Color(0, 0.1764f, 0.6863f, 1);

            enemyID.color = new Color(0.196f, 0.196f, 0.196f);
            enemyTurn.color = new Color(0.196f, 0.196f, 0.196f, 0);
            tankB.active = false;
            tankB.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
        else
        {
            tankA.active = false;
            tankA.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            playerID.color = new Color(0.196f, 0.196f, 0.196f);
            playerTurn.color = new Color(0.196f, 0.196f, 0.196f, 0);

            tankB.active = true;
            tankB.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            tankB.GetComponent<tankManager>().fuel = 100;
            enemyID.color = new Color(0.6078f, 0, 0);
            enemyTurn.color = new Color(0.6078f, 0, 0, 1);

            print("AI turn");
            tankB.GetComponent<tankManager>().aiTurn();
        }
        bTurn = !bTurn;

        System.Random rand = new System.Random();
        int upperBound = 30;
        if (PlayerPrefs.GetInt("level") == 1) upperBound = 6;
        int spawnCrate = rand.Next(0,upperBound);
        crateCreate(spawnCrate);
    }

    private void crateCreate(int boxNum)
    {
        if (boxNum > 5) return;
        print("Crate Created: " + boxNum);
        Sprite[] imgList = new Sprite[6];
        imgList[0] = Resources.Load("randomBox", typeof(Sprite)) as Sprite;
        imgList[1] = Resources.Load("bulletDown", typeof(Sprite)) as Sprite;
        imgList[2] = Resources.Load("bulletUp", typeof(Sprite)) as Sprite;
        imgList[3] = Resources.Load("healthUp", typeof(Sprite)) as Sprite;
        imgList[4] = Resources.Load("sizeDown", typeof(Sprite)) as Sprite;
        imgList[5] = Resources.Load("sizeUp", typeof(Sprite)) as Sprite;

        System.Random rand = new System.Random();
        GameObject obj = Instantiate(pubCareDrop, new Vector2(rand.Next(-1, 2), 5.5f), new Quaternion(0, 0, 0, 0)) as GameObject;
        obj.GetComponent<SpriteRenderer>().sprite = imgList[boxNum];
        obj.GetComponent<dropScript>().dropValue = boxNum;
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(rand.Next(-100, 100), 20) * rand.Next(1, 4));
    }

    //end game control
    void endGame()
    {
        tankA.active = false;
        tankB.active = false;
        GetComponent<BlurOptimized>().enabled = true;
        if(gameState) StartCoroutine(fadeIn(true));

        StartCoroutine(waitForFade(5f));
        gameState = false;
        
    }

    IEnumerator gameLoss()
    {
        int testTest = levelTest;
        messageBox.Show("Defeat", "You have lost the war soldier.\nYou have disappointed your country.", null, gameOverTest, "Retry!", "Cower and Flee");
        while (levelTest == testTest)
        {
            
            yield return new WaitForSeconds(0.4f);

        }
        Destroy(messageBox);
        if (!gameState) Application.Quit();
        else Initiate.Fade("firstTimeLoad", Color.black, 2.0f);
    }

    private void gameOverTest(string arg)
    {
        if (arg == "Cower and Flee" || arg == "Desert the Battle")
        {
            gameState = false;
            levelTest = -1;
        }
        else levelTest++;
    }

    public void levelController (int level)
    {
        print(level);
        StopAllCoroutines();
        if (level >= 0) StartCoroutine(nextLevel(level));
        else if (level < 0) StartCoroutine(gameLoss());
    }

    IEnumerator nextLevel(int level)
    {
        if (level == 3)
        {
            openBox("Congratulations!", "Well played Soldier!\nYou have completed level 1.", null, gameOverTest, "Press On", "Desert the Battle");
            while (levelTest != 1) yield return new WaitForSeconds(0.4f);
            if (!gameState) Application.Quit();
        }
        if (level == 2)
        {
            openBox("Congratulations!", "Well played Soldier!\nYou have completed level 2.", null, gameOverTest, "Press On", "Desert the Battle");
            while (levelTest != 1) yield return new WaitForSeconds(0.4f);
            if (!gameState) Application.Quit();
        }
        if (level == 1)
        {
            openBox("Congratulations!", "Outstanding!\n\nYou finished the game.\nNow roam free in arcade mode.", null, gameOverTest, "Let's Go!", "I can't say no!");
            while (levelTest != 1) yield return new WaitForSeconds(0.4f);
        }
        Initiate.Fade("testLevel", Color.black, 2.0f);

    }

    void openBox(string t, string m, Sprite icon, UnityAction<string> method, string msg1, string msg2)
    {
        messageBox.Show(t, m, icon, method, msg1, msg2);
    }

    IEnumerator waitForFade(float wait)
    {
        while (true)
        {
            yield return new WaitForSeconds(wait);
            GameObject.Find("restart").GetComponent<FadeUI>().fadeUIin();
            GameObject.Find("gameOver").GetComponent<FadeUI>().fadeUIin();
            GameObject.Find("restart").GetComponent<Button>().interactable = true;

        }
    }

    IEnumerator fadeIn(bool bFade)
    {
        if (bFade)
        {
            
            for (float i = 4; i >= 0; i -= Time.deltaTime)
            {
                if (this.GetComponent<BlurOptimized>().blurSize < 10) this.GetComponent<BlurOptimized>().blurSize += 0.05f;

                if (transform.position.y < 1.42) transform.position = new Vector3(0, transform.position.y + 0.007f, -10);
                else if (transform.position.y > 1.42) transform.position = new Vector3(0, 1.42f, -10);

                if (Camera.main.orthographicSize > 2.4) Camera.main.orthographicSize -= 0.012f;
                else if (Camera.main.orthographicSize < 2.4) Camera.main.orthographicSize = 2.4f;


                if (playerUI.alpha > 0) playerUI.alpha -= 0.02f;

                yield return null;
            }
        }
    }

}
