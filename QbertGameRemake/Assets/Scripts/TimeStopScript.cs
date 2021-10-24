using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopScript : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject currentTile;
    bool moving = false;
    public GameObject bottom;
    public AudioSource SFX;
    public AudioClip Jump;
    public Rigidbody rb;
    public float dir;
    static public float movementScale = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        SFX = GameObject.Find("SFX").GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        StartCoroutine("MoveBall");


    }

    IEnumerator Fall()
    {
        RaycastHit hit;
        while (!Physics.Raycast(bottom.transform.position, Vector3.down, out hit, 0.2f, 1 << 6))
        {
            //Debug.Log("not onGround");
            rb.isKinematic = false;
            yield return new WaitForFixedUpdate();
        }
        rb.isKinematic = true;
    }

    IEnumerator MoveBall()
    {
        yield return StartCoroutine("Fall");
        while (true)
        {


            if (moving == false)
            {

                yield return new WaitForSeconds(0.2f);

                if (Random.value >= 0.5)
                {
                    dir = 0;
                    StartCoroutine(MovePos(transform.position, transform.position + new Vector3(-1, -1, 0)));
                }
                else
                {
                    dir = -90;
                    StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, -1, -1)));
                }

            }

            yield return new WaitForEndOfFrame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5)
        {
            Destroy(this.gameObject);
        }
    }
    public IEnumerator MovePos(Vector3 startPos, Vector3 endPos)
    {
        moving = true;
        float timer = 0;
        while (timer < 1.1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, dir, 0), timer);
            Vector3 LerpedPos = Vector3.Lerp(startPos, endPos, timer);
            transform.position = LerpedPos + new Vector3(0, Mathf.Sin(timer * Mathf.PI - 0.1564348f));
            timer += 0.05f;
            yield return new WaitForSeconds(movementScale);
        }
        //transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(bottom.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            //Debug.Log("Hello There");
            SFX.PlayOneShot(Jump);
            moving = false;
        }
        else
        {
            //Debug.Log("Help me");
            rb.isKinematic = false;
            rb.useGravity = true;
        }


        yield return new WaitForEndOfFrame();
    }
}
