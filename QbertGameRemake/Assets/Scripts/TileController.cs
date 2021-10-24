using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public bool touched = false;
    public Material touchedMat;
    public Material untouchedMat;
    public Material touched2Mat;
    public Mesh untouchedMesh;
    public Mesh touchedMesh;
    public int[] directions;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Renderer>().material = untouchedMat;
        transform.GetComponent<MeshFilter>().mesh = untouchedMesh;
        int num = Random.Range(0, directions.Length);
        int num1 = Random.Range(0, directions.Length);
        int num2 = Random.Range(0, directions.Length);
        transform.rotation = Quaternion.Euler(new Vector3(directions[num], directions[num1], directions[num2]));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void IsTouched()
    {
        touched = true;
        transform.GetComponent<Renderer>().material = touchedMat;
        transform.GetComponent<MeshFilter>().mesh = touchedMesh;
    }
}
