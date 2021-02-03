using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAnimation : MonoBehaviour
{
    public TextMesh production, shadow;

    //e o texto do numero q aparece na frente quando pega as moedas

    // Start is called before the first frame update
    void Start()
    {
        production.GetComponent<Renderer>().sortingLayerName = "HUD";
        production.GetComponent<Renderer>().sortingOrder = 99;

        shadow.GetComponent<Renderer>().sortingLayerName = "HUD";
        shadow.GetComponent<Renderer>().sortingOrder = 98;

        Destroy(this.gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
