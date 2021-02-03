using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardCollection : MonoBehaviour
{
    public bool isClickable; // essa carta e clicavel
    public SlotController sc;
    public slot s;

    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public GameController _GameController;
    [HideInInspector]
    public Cards Card;

    public Text cardName;
    public Text levelCard;
    public Text progressCard;

    public Image spriteCard;
    public Image bgCard;
    public Image progressBar;


  public void upgradeInfoCard()
    {
        if(Card.isLiberate == false) // se nao tem
        {
            bgCard.sprite = _GameController.BGCard[4]; // 4- nao tem
            spriteCard.sprite = Card.shadownCard; // imagem da sombra
            cardName.text = "?";
            levelCard.text = "0";
            progressCard.text = "";
            progressBar.fillAmount = 0;
        }
        else
        {
            switch (Card.rarityCard)
            {
                case rarity.COMUM:
                    bgCard.sprite = _GameController.BGCard[0];
                    break;
                case rarity.RARA:
                    bgCard.sprite = _GameController.BGCard[1];
                    break;
                case rarity.EPICA:
                    bgCard.sprite = _GameController.BGCard[2];
                    break;
                case rarity.LENDARIA:
                    bgCard.sprite = _GameController.BGCard[3];
                    break;
            }

            spriteCard.sprite = Card.spriteCard; 
            cardName.text = Card.cardName;
            levelCard.text = Card.levelCard.ToString();
            progressCard.text = Card.cardCollected + "/" + _GameController.progressCard[Card.levelCard - 1];

            float fillAmount = 0;
            if(Card.cardCollected > 0)
            {
                fillAmount = (float)Card.cardCollected / _GameController.progressCard[Card.levelCard - 1];
            }

            progressBar.fillAmount = fillAmount;
        }
    }

    public void setCard()
    {
        _GameController.activeSlot(s, sc, Card.idCard);
    }
}
