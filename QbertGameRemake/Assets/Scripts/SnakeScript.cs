using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    public LayerMask TileMask;
    public ParticleSystem EggCrack;
    public GameObject[] tiles;
    public GameObject currentTile;
    public GameObject Player;
    bool moving = false;
    float dir;
    public GameObject bottom;
    public Rigidbody rb;
    public GameObject Enemy;
    public AudioSource SFX;
    public AudioClip bounceSound;
    public AudioClip FallSound;
    public AudioClip PhoenixBounce;
    public AudioClip EggHatch;
    public AudioClip PhoenixDeath;
    public float JumpMultiplier = 1;
    public GameObject EggMesh;
    public GameObject PheonixMesh;

    static public float movementScale = 0.01f;
    Vector3 diskPos;
    Vector3 last;
    Vector3 cur;

    private SnakeState curState = SnakeState.ball;

    private enum SnakeState
    {
        ball,
        snake,
        disk,

    }

    // Start is called before the first frame update
    public void Start()
    {
        EggMesh.SetActive(true);
        PheonixMesh.SetActive(false);
        SFX = GameObject.Find("SFX").GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        Player = GameObject.Find("Qbert");

        curState = SnakeState.ball;
        moving = false;
        JumpMultiplier = 1;
        rb.isKinematic = true;
        rb.useGravity = true;

        StartCoroutine("MoveBall");
        
        RaycastHit hit;
        if (Physics.Raycast(bottom.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                gameObject.tag = "EnemyDis";
                rb.isKinematic = true;
                rb.useGravity = false;
                gameObject.SetActive(false);
            }
        }
      
            
       
    }

    void ballAI()
    {
        bool x = Enemy.GetComponent<EnemyScript>().GetBit(transform.position + new Vector3(-1, -1, 0));
        bool z = Enemy.GetComponent<EnemyScript>().GetBit(transform.position + new Vector3(0, -1, -1));
        if(x && z)
        {   
            //pass
        }
        else if(x)
        {
            //Debug.Log("moveAway");
            StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, -1, -1)));         
        }
        else if(z)
        {
            //Debug.Log("moveAway");
            StartCoroutine(MovePos(transform.position, transform.position + new Vector3(-1, -1, 0)));
        }
        else
        {
            //Debug.Log("random");
            if (Random.value >= 0.5)
            {
                StartCoroutine(MovePos(transform.position, transform.position + new Vector3(-1, -1, 0)));
            }
            else 
            {
                StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, -1, -1)));
            }
        }
    }


    public void callDiskAI(Vector3 diskPosIn)
    {
        float dx = diskPosIn.x - transform.position.x;
        float dz = diskPosIn.z - transform.position.z;
        if (Mathf.Abs(dx) + Mathf.Abs(dz) <= 4.0f && curState == SnakeState.snake)
        {
            diskPos = diskPosIn + new Vector3(0, 1, 0);
            curState = SnakeState.disk;
        }
    }

    IEnumerator diskAI()
    {
        float dx;
        float dz;
        while (true)
        {
            if (moving == false)
            {
                dx = diskPos.x - transform.position.x;
                dz = diskPos.z - transform.position.z;
                if (Mathf.Abs(dx) > Mathf.Abs(dz))
                {
                    moveX(dx);
                }
                else
                {
                    moveZ(dz);
                }
                yield return new WaitForSeconds(1.0f);
            }

            if (transform.position.y < -75)
            {
                GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject e in Enemies)
                {
                    if (e.name != "Egg")
                    {
                        e.GetComponent<EnemyScript>().changeBitArray(e.transform.position, false);
                        e.gameObject.tag = "EnemyDis";
                        e.GetComponent<Rigidbody>().isKinematic = true;
                        e.GetComponent<Rigidbody>().useGravity = true;
                        e.gameObject.SetActive(false);
                    }
                }
                SFX.PlayOneShot(PhoenixDeath);
               	yield return new WaitForSeconds(6.0f);
                rb.isKinematic = true;
                rb.useGravity = false;
                gameObject.tag = "EnemyDis";
                gameObject.SetActive(false);

            }

            yield return null;
        }


    }

    void snakeAI()
    {

        //Debug.Log("inisSnake");
        float dx = Player.transform.position.x - transform.position.x;
        float dz = Player.transform.position.z - transform.position.z;

        bool canMoveZ = getCanMoveZ(dz);
        bool canMoveX = getCanMoveX(dx);
        if(canMoveZ && canMoveX)
        {
            Debug.Log("both");
            if (Mathf.Abs(dx) > Mathf.Abs(dz))
            {
                moveX(dx);
            }
            else
            {
                moveZ(dz);
            }
        }
        else if (canMoveZ)
        {
            Debug.Log("canMoveZ");
            moveZ(dz);
        }
        else if (canMoveX)
        {
            Debug.Log("canMoveX");
            moveX(dx);
        }
        else
        {
            //do nothing
        }
      

    }

    bool getCanMoveZ(float dz)
    {
        RaycastHit hit;
        if (dz <= 0.5f)
        {
            if(Physics.Raycast(bottom.transform.position + new Vector3(0, -1, -1), Vector3.down, out hit, Mathf.Infinity, TileMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(dz >= 0.5f)
        {
            if(Physics.Raycast(bottom.transform.position + new Vector3(0, 1, 1), Vector3.down, out hit, Mathf.Infinity, TileMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    bool getCanMoveX(float dx)
    {
        RaycastHit hit;
        if (dx <= 0.5f)
        {
            if(Physics.Raycast(bottom.transform.position + new Vector3(-1, -1, 0), Vector3.down, out hit, Mathf.Infinity, TileMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(dx >= 0.5f)
        {
            if(Physics.Raycast(bottom.transform.position + new Vector3(1, 1, 0), Vector3.down, out hit, Mathf.Infinity, TileMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void moveX(float dx)
    {
        if (dx <= 0.5f)
        {
            dir = 90;
            StartCoroutine(MovePos(transform.position, transform.position + new Vector3(-1, -1, 0)));
        }

        else if (dx >= 0.5f)
        {
            dir = -90;
            StartCoroutine(MovePos(transform.position, transform.position + new Vector3(1, 1, 0)));
        }
    }

    void moveZ(float dz)
    {
        if (dz >= 0.5f)
        {
            dir = 180;
            StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, 1, 1)));
        }
        else if (dz <= 0.5f)
        {
            dir = 0;
            StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, -1, -1)));
        }
    }

    IEnumerator Fall()
    {
        RaycastHit hit;
        while (!Physics.Raycast(bottom.transform.position, Vector3.down, out hit, 0.3f))
        {
            
            //Debug.Log("not onGround");
            rb.isKinematic = false;
            yield return new WaitForFixedUpdate();
        }
        rb.isKinematic = true;
    }

    IEnumerator MoveBall()
    {

        bool snakeFirst = true;
        yield return StartCoroutine("Fall");
        Debug.Log("Fell and started AI");
        while (true)
        {

            if (curState == SnakeState.ball)
            {

                if (moving == false)
                {
                    yield return new WaitForSeconds(1.0f);
                    ballAI();
                }
                if (gameObject.transform.position.y < 0.5f)
                {
                    
                    curState = SnakeState.snake;
                    Enemy.GetComponent<EnemyScript>().changeBitArray(transform.position, false);
                    
                }
            }
            else if (curState == SnakeState.snake)
            {
                if(snakeFirst && transform.position.y <= 0.5f)
                {
                    yield return new WaitForSeconds(1.1f);
                    snakeFirst = false;
                    EggMesh.SetActive(false);
                    PheonixMesh.SetActive(true);
                    SFX.PlayOneShot(EggHatch);
                    EggCrack.Play();
                    JumpMultiplier = 3;
                }
                if (moving == false)
                {
                    yield return new WaitForSeconds(1.0f);
                    snakeAI();
                }
                //Debug.Log("inisSnake");

            }
            else
            {
                yield return new WaitForSeconds(1.0f);
                yield return StartCoroutine("diskAI");
            }

            yield return new WaitForEndOfFrame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -200)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            transform.position += new Vector3 (0, 100, 0);
            gameObject.tag = "EnemyDis";
            gameObject.SetActive(false);
        }
    }



    public IEnumerator MovePos(Vector3 startPos, Vector3 endPos)
    {
        RaycastHit hit;
        if(curState == SnakeState.ball)
        {
            last = startPos;
            cur = endPos;
            
            if(Physics.Raycast(cur,Vector3.down,out hit, 2.0f))
            {
                Enemy.GetComponent<EnemyScript>().changeBitArray(last, cur);
            }
            else
            {
                Enemy.GetComponent<EnemyScript>().changeBitArray(last, false);
            }
        }      

        moving = true;
        float timer = 0;
        while (timer < 1.1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, dir, 0), timer);
            Vector3 LerpedPos = Vector3.Lerp(startPos, endPos, timer);
            transform.position = LerpedPos + new Vector3(0, Mathf.Sin(timer * Mathf.PI - 0.1564348f)*JumpMultiplier, 0);
            timer += 0.05f;
            yield return new WaitForSeconds(movementScale*JumpMultiplier);
        }
        //transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        if (Physics.Raycast(bottom.transform.position, Vector3.down, out hit, Mathf.Infinity, TileMask))
        {
            Debug.Log(hit.transform.name);
            if(curState == SnakeState.snake || curState == SnakeState.disk)
            {
                SFX.PlayOneShot(PhoenixBounce);
            }
            else
            {
                SFX.PlayOneShot(bounceSound);
            }

            moving = false;
        }
        else
        {
            Debug.Log("Help me");
            transform.GetComponent<AudioSource>().PlayOneShot(FallSound);
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        yield return new WaitForEndOfFrame();
    }
}