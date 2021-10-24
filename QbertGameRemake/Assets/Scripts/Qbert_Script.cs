using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Qbert_Script : MonoBehaviour
{
    public GameObject SnoBoi;
    public GameObject aniatedDeath;
    public GameObject Pause;
    public bool paused;
    public float dir; 
    public GameObject[] tiles; 
    public GameObject currentTile;
    public GameObject Enemy;
    public int touchedTiles = 0;
    public bool moving = false;
    public GameObject bottom;
    public Rigidbody rb;
    public Material[] touchedPads;
    public Material[] colors;
    public int score;
    public int lives;
    public TextMeshProUGUI LivesText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI GameOverText;
    public GameObject Camera;
    public GameObject[] livesImages;
    public AudioSource SFX;
    public AudioSource Music;
    public AudioClip screamInPain;
    public AudioClip Swear;
    public AudioClip splat;
    public AudioClip Ding;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    public AudioClip hitGround;
    public ParticleSystem WinningSnow;
    int isDead = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");

    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == 2)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Pause.SetActive(false);
                paused = false;
                Time.timeScale = 1;

            }
            else
            {
                Pause.SetActive(true);
                paused = true;
                Time.timeScale = 0;
            }
        }
        if (moving == false && isDead == 0 && paused  == false) // Checks if the player is ready to move to next position
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                dir = -90; //Sets the direction for qbert to move in
                StartCoroutine(MovePos(transform.position, transform.position + new Vector3(1, 1, 0))); // Starts corutine for movement to the next position
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                dir = 90;//Sets the direction for qbert to move in
                StartCoroutine(MovePos(transform.position, transform.position + new Vector3(-1, -1, 0))); // Starts corutine for movement to the next position
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                dir = 180;//Sets the direction for qbert to move in
                StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, 1, 1))); // Starts corutine for movement to the next position
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                dir = 0;//Sets the direction for qbert to move in
                StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, -1, -1))); // Starts corutine for movement to the next position
            }
        }
        if (transform.position.y <= -150)// Checks if player falls off the map
        {
            lives -= 1; //decrease lives by 1
            LivesText.SetText("Lives: " + lives.ToString()); // sets text to lives
            if (lives <= 0) // Checks if the player is out of lives
            {
                gameOver(); // Run fameOver function
            }
            else
            {
                StartCoroutine("ResetAfterFalloff"); //Starts coroutine ResetAfterFall
            }

        }
    }

    void gameOver()
    {
    	LivesText.SetText("Lives: 0"); // ensures that the lives text doesn't go under 0
        GameOverText.gameObject.SetActive(true); // shows game over text
        GameOverText.SetText("Game Over");
        isDead = 2;
        moving = true;
        SpawnerScript.timestop = true;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] TimeStops = GameObject.FindGameObjectsWithTag("TimeStop");
        foreach (GameObject e in Enemies)
        {

            Enemy.GetComponent<EnemyScript>().changeBitArray(e.transform.position, false);
            e.gameObject.tag = "EnemyDis";
            e.GetComponent<Rigidbody>().isKinematic = true;
            e.GetComponent<Rigidbody>().useGravity = true;
            e.gameObject.SetActive(false);

        }
        foreach (GameObject e in TimeStops)
        {
            Destroy(e);
        } // sets text to say Game Over
        StartCoroutine("playLoseSound"); // Play Losing sound
        SpawnerScript.timestop = true; // stops spawning enemies
        //Destroy(this.gameObject);
    }

    IEnumerator playLoseSound() // Plays lose sound and deletes player after
    {
    	Music.PlayOneShot(LoseSound);
    	yield return new WaitForSeconds(6.0f);
    	Destroy(this.gameObject);
    }

    public IEnumerator MovePos(Vector3 startPos, Vector3 endPos)
    {
        if(moving == false) //checks if the player is already moving
        {
            moving = true; //prevents player from moving until current moving is done
            float timer = 0;
            yield return new WaitForSeconds(0.2f);
            while (timer < 1.1) // moves player through lerp untyil the player is at the desired destination
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, dir, 0), timer);
                Vector3 LerpedPos = Vector3.Lerp(startPos, endPos, timer);
                transform.position = LerpedPos + new Vector3(0, Mathf.Sin(timer * Mathf.PI - 0.1564348f), 0);//lerped position. Used sine wave to make it look like a jump
                timer += 0.05f;
                yield return new WaitForSeconds(0.01f);
            }
            RaycastHit hit;
            if (Physics.Raycast(bottom.transform.position, Vector3.down, out hit, Mathf.Infinity)) //Shoot raycast to check if there is hitGround below
            {
                if (hit.transform.tag == "Tile")
                {
                    currentTile = hit.transform.gameObject;
                    if (hit.transform.GetComponent<TileController>().touched == false)
                    {
                        SFX.PlayOneShot(Ding);
                        hit.transform.GetComponent<TileController>().IsTouched();
                        touchedTiles += 1;
                        score += 25;
                        ScoreText.SetText("Score: " + score.ToString());
                        if (touchedTiles >= 36)
                        {
                            moving = true;
                            StartCoroutine(Ending());
                        }
                        else
                        {
                            moving = false;
                        }
                        /*
                         ?????????????????????????????? ?????????????????????????
                         ?????????????????????????????? ????????????????????????????
                        ?????????????????????????????? ?????????????????????????????
                        ?????????????????????????????? ?????????????????????????????
                        ?????????????????????????????? ?????????????????????????????
                         ?????????????????????????????? ?????????????????????????????
                        ?????????????????????????????? ?????????????????????????????
                        ?????????????????????????????? ???????????????????????????
                        ?????????????????????????????? ??????????????????????????
                        ?????????????????
                         */
                    }
                    else
                    {
                        moving = false;
                    }
                }
                else if (hit.transform.tag == "Disk")
                {
                    transform.parent = hit.transform;
                    hit.transform.GetComponent<DiskController>().CallMovePos();
                }
                SFX.PlayOneShot(splat);
            }
            else // Player falls to its death
            {
                moving = true;
                SpawnerScript.timestop = true;
                SFX.PlayOneShot(screamInPain);
                rb.isKinematic = false;
                rb.useGravity = true;
                yield return new WaitForSeconds(3);
                SFX.PlayOneShot(hitGround);
            }
        }

        

        yield return new WaitForEndOfFrame();
    }
    public IEnumerator Ending() //removes all enemies, stops spawning, and gives you win text
    {
        int colorNum = 0;
        int i = 0;
        Music.PlayOneShot(WinSound);
        while (i < 30)
        {
            SpawnerScript.timestop = true;
            GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] TimeStops = GameObject.FindGameObjectsWithTag("TimeStop");
            foreach (GameObject e in Enemies)
            {
                
                Enemy.GetComponent<EnemyScript>().changeBitArray(e.transform.position, false);              
                e.gameObject.tag = "EnemyDis";
                e.GetComponent<Rigidbody>().isKinematic = true;
                e.GetComponent<Rigidbody>().useGravity = true;
                e.gameObject.SetActive(false);

            }
            foreach (GameObject e in TimeStops)
            {
                Destroy(e);
            }
            for (int y = 0; y < tiles.Length; y++)
            {
                tiles[y].GetComponent<Renderer>().material = colors[colorNum];
            }
            colorNum++;
            if (colorNum >= 3)
            {
                colorNum = 0;
            }
            i++;
            yield return new WaitForSeconds(0.1f);
        }

        GameOverText.gameObject.SetActive(true);
        WinningSnow.Play();
        GameOverText.SetText("You Win!");
        isDead = 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Enemy") //Check if player is touching enemy or time stop
        {
            isDead = 1;
            moving = true;
            SFX.PlayOneShot(Swear);
            
            lives--;
            LivesText.SetText("Lives: "+lives.ToString());
            if (lives <= 0)
            {
                gameOver();
            }
            else
            {
                StartCoroutine("Reset");
                
            }
            //GameControl.GetComponent<GameController>().StartLiveReset();           
        }
        if(other.transform.tag == "TimeStop")
        {
            Camera.GetComponent<CameraController>().stop();
            Destroy(other.gameObject);
        }
    }

    IEnumerator Reset()//resets player after hitting enemy
    {
        moving = true;
        aniatedDeath.SetActive(true);
        SnoBoi.SetActive(false);
        aniatedDeath.GetComponent<Animator>().SetTrigger("Death");
        SpawnerScript.timestop = true;
        //play reset music
        float timer = 0.0f;

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] TimeStops = GameObject.FindGameObjectsWithTag("TimeStop");
        foreach (GameObject e in Enemies)
        {
            Enemy.GetComponent<EnemyScript>().changeBitArray(e.transform.position, false);              
            e.gameObject.tag = "EnemyDis";
            e.GetComponent<Rigidbody>().isKinematic = true;
            e.GetComponent<Rigidbody>().useGravity = true;
            e.gameObject.SetActive(false);

        }
        foreach (GameObject e in TimeStops)
        {
            moving = true;
            Destroy(e);
        }
        yield return new WaitForSeconds(2f);

        //transform.position = currentTile.transform.position + new Vector3(0, 1, 0);
        SpawnerScript.timestop = false;
        moving = false;
        isDead = 0;
        SnoBoi.SetActive(true);
        aniatedDeath.SetActive(false);
        
    }

    IEnumerator ResetAfterFalloff() //Places player at the top when it falls off the map
    {
        moving = true;
        
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = new Vector3(3, 7, 2);

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] TimeStops = GameObject.FindGameObjectsWithTag("TimeStop");
        foreach (GameObject e in Enemies)
        {        
            Enemy.GetComponent<EnemyScript>().changeBitArray(e.transform.position, false);              
            e.gameObject.tag = "EnemyDis";
            e.GetComponent<Rigidbody>().isKinematic = true;
            e.GetComponent<Rigidbody>().useGravity = true;
            e.gameObject.SetActive(false);
        }
        foreach (GameObject e in TimeStops)
        {
            Destroy(e);
        }
        SpawnerScript.timestop = true;
        //play reset music
        float timer = 0.0f;

        rb.isKinematic = true;
        SpawnerScript.timestop = false;
        moving = false;
        yield return new WaitForSeconds(2f);
        
    }

    


}
