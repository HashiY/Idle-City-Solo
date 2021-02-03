using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Collection/Card")]
//criar um novo asset do tipo collection -> card com o nome new card
public class Cards : ScriptableObject
{
    public int idCard;
    public string cardName;
    public Sprite spriteCard;
    public Sprite shadownCard;
    public rarity rarityCard;

    public bool isLiberate;
    public int levelCard = 1;
    public int cardCollected;
    public double production;
    public float timeProduction;

    public int productionMultiplier = 1; // multiplicador de tempo
    public float productionReduction = 1; // reduçao de tempo

    public bool isMax; // se a carta esta maximizada


    public void reset()
    {
        if(idCard == 0)
        {
            isLiberate = true;
        }
        else
        {
            isLiberate = false;
        }
        levelCard = 1;
        productionMultiplier = 1;
        productionReduction = 1;
        cardCollected = 0;
        isMax = false;
    }



}
