using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject[] FireBois;
    public GameObject Snake;
    public GameObject TimeFreezePrefab;
	int currEnemies = 0;
	int maxEnemies = 3;
    int enemyIndex = 0;
    static public bool timestop = false; 
    // Start is called before the first frame update
    void Start()
    {
        timestop = false;
        StartCoroutine(spawnEnemy());
        FireBois= GameObject.FindGameObjectsWithTag("EnemyDis");
        for (int i = 0; i < FireBois.Length; i++)
        {
            FireBois[i].GetComponent<Rigidbody>().useGravity = false;
            FireBois[i].GetComponent<Rigidbody>().isKinematic = true;
            FireBois[i].SetActive(false);

        }
        Snake.GetComponent<Rigidbody>().useGravity = false;
        Snake.GetComponent<Rigidbody>().isKinematic = true;
        Snake.SetActive(false);

    }	

    // Update is called once per frame
    void Update()
    {

    }

    bool RandomBool()
    {
    	return Random.value >= 0.5;
    }

    IEnumerator spawnEnemy()
    {
        while (true)
        {   
            yield return new WaitForSeconds(6.0f);
            if(!timestop)
            {
                
                currEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
                Debug.Log(currEnemies);
                if (!Snake.active)
                {
                    Debug.Log("Snake");
                    GameObject enemyClone = Snake;
                    enemyClone.SetActive(true);
                    enemyClone.gameObject.tag = "Enemy";
                    Vector3 pos = transform.position;
                    if(RandomBool())
                    {
                        pos += new Vector3 (0, 0, -1);
                    }
                    else
                    {
                        pos += new Vector3 (-1, 0, 0);
                    }
                    enemyClone.transform.position = pos;
                    enemyClone.GetComponent<SnakeScript>().Start();
                }                
                else if(currEnemies < maxEnemies)
                {

                    int randomNum = Random.Range(0, 10);
                    if(randomNum < 8)
                    {
                        Debug.Log("spawn an enemy");
                        GameObject enemyClone = FireBois[enemyIndex];
                        enemyIndex = (enemyIndex + 1) % 3;

                        enemyClone.SetActive(true);
                        enemyClone.gameObject.tag = "Enemy";
                        Vector3 pos = transform.position;
                        if (RandomBool())
                        {
                            pos += new Vector3(0, 0, -1);
                        }
                        else
                        {
                            pos += new Vector3(-1, 0, 0);
                        }
                        enemyClone.transform.position = pos;
                        yield return null;
                        enemyClone.GetComponent<EnemyScript>().Start();
                    }
                    else
                    {
                        Debug.Log("spawn a timefreeze");
                        GameObject enemyClone = Instantiate(TimeFreezePrefab);
                        Vector3 pos = transform.position;
                        if (RandomBool())
                        {
                            pos += new Vector3(0, 0, -1);
                        }
                        else
                        {
                            pos += new Vector3(-1, 0, 0);
                        }
                        enemyClone.transform.position = pos;
                    }

                }
                //yield return new WaitForSeconds(EnemyScript.movementScale);
                        
            }
            yield return null;
        }
    }
}
