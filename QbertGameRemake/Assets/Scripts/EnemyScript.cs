using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject currentTile;
    bool moving = false;
    public GameObject bottom;
    public Rigidbody rb;
    static public float movementScale = 0.01f;
    public float dir;
    public static int arrayWidth = 7;
    public static Vector3 top = new Vector3(3, 7, 2);
    public AudioSource SFX;
    public AudioClip Jump;
    public static  BitArray positionArray = new BitArray(8 * 8);
    

    Vector3 last;
    Vector3 cur;

    // Start is called before the first frame update
    public void Start()
    {
        SFX = GameObject.Find("SFX").GetComponent<AudioSource>();
        
        rb = GetComponent<Rigidbody>();
        moving = false;
        rb.isKinematic = true;
        rb.useGravity = true;
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        RaycastHit hit;
        StartCoroutine("MoveBall"); 
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


    public void changeBitArray(Vector3 last, Vector3 cur)
    {
    	positionArray.Set((int)Mathf.Abs(Mathf.Round(top.x - last.x)) * arrayWidth + (int)Mathf.Abs(Mathf.Round(top.z - last.z)), false);
    	positionArray.Set((int)Mathf.Abs(Mathf.Round(top.x - cur.x)) * arrayWidth + (int)Mathf.Abs(Mathf.Round(top.z - cur.z)), true);

    }

    public void changeBitArray(Vector3 v, bool b)
    {
    	positionArray.Set((int)Mathf.Abs(Mathf.Round(top.x - v.x)) * arrayWidth + (int)Mathf.Abs(Mathf.Round(top.z - v.z)), b);
    }

    public bool GetBit(Vector3 v)
    {
    	return positionArray[(int)Mathf.Abs(Mathf.Round(top.x - v.x)) * arrayWidth + (int)Mathf.Abs(Mathf.Round(top.z - v.z))];
    }

    IEnumerator Fall()
    {
    	RaycastHit hit;
    	while(!Physics.Raycast(bottom.transform.position ,Vector3.down, out hit, 0.3f, 1 << 6))
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

		while(true)
		{	
			
			//check whihc can move
			//if not left go right
			//if not right go left
			//else 50/50

			if(moving == false)
			{	
  
				yield return new WaitForSeconds(1.0f);
				bool x = GetBit(transform.position + new Vector3(-1, -1, 0));
				bool z = GetBit(transform.position + new Vector3(0, -1, -1));
				if(x && z)
				{	
					//pass
				}
				else if(x)
				{
					//Debug.Log("moveAway");
					dir = -90;
					StartCoroutine(MovePos(transform.position, transform.position + new Vector3(0, -1, -1)));
					
				}
				else if(z)
				{
					//Debug.Log("moveAway");
					dir = 0;
					StartCoroutine(MovePos(transform.position, transform.position + new Vector3(-1, -1, 0)));
				}
				else
				{
					//Debug.Log("random");
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
			}

			yield return new WaitForEndOfFrame();
		}
		
	}
 
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -150)
		{
            gameObject.tag = "EnemyDis";
            rb.isKinematic = true;
            rb.useGravity = false;
            gameObject.SetActive(false);
            

        }
    }
    public IEnumerator MovePos(Vector3 startPos, Vector3 endPos)
    {
    	last = startPos;
    	cur = endPos;
    	RaycastHit hit;
    	if(Physics.Raycast(cur,Vector3.down,out hit, 2.0f, 1 << 6))
    	{
    		changeBitArray(last, cur);
    	}
    	else
    	{
    		changeBitArray(last, false);
    	}
        

        moving = true;
        float timer = 0;
        while (timer < 1.1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, dir, 0), timer);
            Vector3 LerpedPos = Vector3.Lerp(startPos, endPos, timer);
            transform.position = LerpedPos+new Vector3(0,Mathf.Sin(timer*Mathf.PI- 0.1564348f));
            timer += 0.05f;
            yield return new WaitForSeconds(movementScale);
        }
        //transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        if(Physics.Raycast(bottom.transform.position,Vector3.down,out hit, 1f, 1 << 6))
        {
            //Debug.Log("Hello There");
            moving = false;
            SFX.PlayOneShot(Jump);

        }
        else
        {
            //transform.GetComponent<AudioSource>().PlayOneShot(FallSound);
            //Debug.Log("Help me");
            rb.isKinematic = false;
            rb.useGravity = true;
          
        }
        

        yield return new WaitForEndOfFrame();
    }

}
