using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rewardInfo : MonoBehaviour
{
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public GameController _GameController;

    public Image iconReward;
    public Image bgReward;
    public Text rewardDescription;
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public Cards cards;

    private double qtd;

    public void showReward(int rewardType) // 0 - ouro, 1 - gem, 2 - card
    {
        switch (rewardType)
        {
            case 0:
                qtd = Random.Range((float)_GameController.getGoldAccumulated() / 5, (float)_GameController.getGoldAccumulated() / 3);
                _GameController.getGold(qtd);
                iconReward.sprite = _GameController.IconCoin[1];
                bgReward.sprite = _GameController.BGCard[4]; // fundo
                rewardDescription.text = _GameController.currencyConverter(qtd) + "golds";
                break;
            case 1:
                qtd = Random.Range(10,25);
                _GameController.getGems((int)qtd);
                iconReward.sprite = _GameController.IconGem;
                bgReward.sprite = _GameController.BGCard[1]; // fundo
                rewardDescription.text = qtd.ToString() + "gems";
                break;
            case 2:
                qtd = 1;
                
                _GameController.getCard(cards, (int)qtd);
                iconReward.sprite = cards.spriteCard;
                rewardDescription.text = qtd.ToString() + " " + cards.cardName;


                switch (cards.rarityCard)
                {
                    case rarity.COMUM:
                        bgReward.sprite = _GameController.BGCard[0];
                        break;
                    case rarity.RARA:
                        bgReward.sprite = _GameController.BGCard[1];
                        break;
                    case rarity.EPICA:
                        bgReward.sprite = _GameController.BGCard[2];
                        break;
                    case rarity.LENDARIA:
                        bgReward.sprite = _GameController.BGCard[3];
                        break;
                }

                break;

        }
    }


}
