using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour
{
    public GameObject player;
    GameObject snake;
    public GameObject topBlock;
    public AudioSource SFX;
    public AudioClip DiskSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CallMovePos()
    {
        snake = GameObject.Find("Egg");
        if (!(snake is null))
        {
            snake.GetComponent<SnakeScript>().callDiskAI(gameObject.transform.position);
        }
        StartCoroutine(MovePos());
    }

    public IEnumerator MovePos()
    {
        SFX.PlayOneShot(DiskSound);
        player.GetComponent<CapsuleCollider>().enabled = false;
        float timer = 0;
        while (timer < 0.3f)
        {
            Vector3 LerpedPos = Vector3.Lerp(transform.position, new Vector3(3, 8, 2), timer);
            transform.position = LerpedPos;
            timer += 0.01f;
            yield return new WaitForSeconds(0.03f);
        }
        transform.DetachChildren();
        transform.GetComponent<MeshRenderer>().enabled = false;
        player.GetComponent<CapsuleCollider>().enabled = true;
        timer = 0;
        while (timer < 0.3f)
        {
            Vector3 LerpedPos = Vector3.Lerp(player.transform.position, new Vector3(3, 7, 2), timer);
            player.transform.position = LerpedPos;
            timer += 0.008f;
            yield return new WaitForSeconds(0.005f);
        }
        player.GetComponent<Qbert_Script>().moving = false;
        RaycastHit hit;
        if (Physics.Raycast(player.GetComponent<Qbert_Script>().bottom.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Tile")
            {
                player.GetComponent<Qbert_Script>().currentTile = hit.transform.gameObject;
                if (hit.transform.GetComponent<TileController>().touched == false)
                {
                    hit.transform.GetComponent<TileController>().IsTouched();
                    player.GetComponent<Qbert_Script>().touchedTiles += 1;
                    player.GetComponent<Qbert_Script>().score += 25;
                    player.GetComponent<Qbert_Script>().ScoreText.SetText("Score: " + player.GetComponent<Qbert_Script>().score.ToString());
                    if (player.GetComponent<Qbert_Script>().touchedTiles >= player.GetComponent<Qbert_Script>().tiles.Length)
                    {
                        player.GetComponent<Qbert_Script>().moving = true;
                        player.GetComponent<Qbert_Script>().StartCoroutine(player.GetComponent<Qbert_Script>().Ending());
                    }
                    else
                    {
                        player.GetComponent<Qbert_Script>().moving = false;
                    }

                }
                else
                {
                    player.GetComponent<Qbert_Script>().moving = false;
                }
            }
            topBlock.transform.GetComponent<TileController>().IsTouched();


    
            Destroy(this.gameObject);
            yield return new WaitForEndOfFrame();
        }
    }
}