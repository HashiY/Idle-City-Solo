using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinAnimation : MonoBehaviour
{
    public float posY; // posiçao Y no momento instanciado
    public Rigidbody2D coinRB;
    public bool iskick;

    // Start is called before the first frame update
    void Start()
    {
        coinRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < posY && iskick == false)
        {
            iskick = true;
            coinRB.velocity = Vector2.zero;
            coinRB.AddForce(new Vector2(35, 300));
            Destroy(this.gameObject, 1);
        }
    }
}
