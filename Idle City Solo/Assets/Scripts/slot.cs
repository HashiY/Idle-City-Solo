using UnityEngine;

[CreateAssetMenu(fileName = "New Slot", menuName = "Collection/Slot")]

public class slot : ScriptableObject
{
    [HideInInspector]
    public GameController _GameController;
    public int idSlot;

    public Cards slotCard;
    public Cards initialCard;

    public bool isPurchased; //se ja comprou terreno  
    public bool isMax;      // se ja esta maximizado
    public bool isAutoProduction; // se ja tem autoproduçao

    public double slotPrice; 
    public int slotLevel = 1;
    public int upgrades; // quantas vezes passou de nivel
    public int totalUpgrades;

    public double upgradePrice;

    public double slotProduction;
    public float slotTimeProduction;

    public int slotProductionMultiplier = 1; //conforme passa de nivel tem um bonus de multiplicador
    public float slotProductionReduction = 1; 

    public void reset()
    {
        if(idSlot == 0)
        {
            isPurchased = true;
        }
        else
        {
            isPurchased = false;
        }
        slotCard = initialCard;
        slotLevel = 1;
        upgrades = 0;
        totalUpgrades = 0;
        slotProductionMultiplier = 1;
        slotProductionReduction = 1;
        isAutoProduction = false;
    }

    public void startSlotScriptable()
    {
        int mult = 1;
        if(totalUpgrades > 0)
        {
            mult = totalUpgrades;
        }
        //vai aumentando o level do terreno e a produçao aumenta (gold)
        //se o level chega em (5,10,30,...) o mulyiplicador aumenta
        slotProduction = slotCard.production * slotCard.productionMultiplier * slotProductionMultiplier * mult * _GameController.multiplierBonus * _GameController.multiplierBonusTemp;
        //reduzir o tempo de produçao
        slotTimeProduction = slotCard.timeProduction / slotCard.productionReduction / slotProductionReduction / _GameController.reductionBonus / _GameController.reductionBonusTemp;
        //preço do upg 
        upgradePrice = slotProduction * slotProductionMultiplier * 1.5f;
    }

}
