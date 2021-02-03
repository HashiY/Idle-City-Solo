using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class openSuitCase : MonoBehaviour
{
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public GameController _GameController;
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public rarity suitCaseRarity;
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public int qtdRewards;

    public Text qtdRewardsTxt;

    public GameObject btnSuitCase, btnClose;
    public GameObject[] slotReward;

    private bool isGetRaro, isGetEpico, isGetLendario, isGetComum;
    private int idReward;

    
    public void Start()
    {
        btnSuitCase.SetActive(true);
        btnClose.SetActive(false);
        qtdRewardsTxt.text = qtdRewards.ToString();
        idReward = 0;

        foreach (GameObject o in slotReward) // desativar todos os slots
        {
            o.SetActive(false);
            o.GetComponent<rewardInfo>()._GameController = _GameController;
        }

        isGetRaro = false;
        isGetEpico = false;
        isGetLendario = false;
        isGetComum = false;
    }

    public void OpenCase()
    {
        if(_GameController.currentState != GameState.BOOSTER)
        {
            return;
        }

        if(qtdRewards > 0)
        {
            switch (suitCaseRarity)
            {
                case rarity.COMUM:
                    randomReward();
                    break;
                case rarity.RARA:
                    if(isGetRaro == false)
                    {
                        isGetRaro = true;
                        getCard(rarity.RARA);
                    }
                    else if (isGetComum == false)
                    {
                        isGetComum = true;
                        getCard(rarity.COMUM);
                    }
                    else
                    {
                        randomReward();
                    }
                    break;
                case rarity.EPICA:
                    if (isGetEpico == false)
                    {
                        isGetEpico = true;
                        getCard(rarity.EPICA);
                    }
                    else if (isGetRaro == false)
                    {
                        isGetRaro = true;
                        getCard(rarity.RARA);
                    }
                    else if (isGetComum == false)
                    {
                        isGetComum = true;
                        getCard(rarity.COMUM);
                    }
                    else
                    {
                        randomReward();
                    }
                    break;
                case rarity.LENDARIA:
                    if (isGetLendario == false)
                    {
                        isGetLendario = true;
                        getCard(rarity.LENDARIA);
                    }
                    else if (isGetEpico == false)
                    {
                        isGetEpico = true;
                        getCard(rarity.EPICA);
                    }
                    else if (isGetRaro == false)
                    {
                        isGetRaro = true;
                        getCard(rarity.RARA);
                    }
                    else if (isGetComum == false)
                    {
                        isGetComum = true;
                        getCard(rarity.COMUM);
                    }
                    else
                    {
                        randomReward();
                    }
                    break;
            }
            idReward += 1;
            qtdRewards -= 1;
            qtdRewardsTxt.text = qtdRewards.ToString();

            if (qtdRewards == 0)
            {
                btnSuitCase.SetActive(false);
                btnClose.SetActive(true);
            }
        }
    }

    void randomReward() // recompensa aleatoria
    {
        int rand = Random.Range(0, 100);

        if(rand >= 25) // 75% recompensa em ouro
        {
            slotReward[idReward].GetComponent<rewardInfo>().showReward(0);
            slotReward[idReward].SetActive(true);
        }
        else if (rand >= 5)// 20% recompensa em gemas
        {
            slotReward[idReward].GetComponent<rewardInfo>().showReward(1);
            slotReward[idReward].SetActive(true);
        }
        else if (rand < 5)// 5% recompensa em cartas
        {
            randomCard();
        }
    }

    void randomCard()
    {
        int rand = Random.Range(0, 100);

        if (rand >= 99) //1% carta lendaria
        {
            getCard(rarity.LENDARIA);
        }
        else if (rand >= 94)//5% carta epica
        {
            getCard(rarity.EPICA);
        }
        else if (rand >= 74)//20% carta rara
        {
            getCard(rarity.RARA);
        }
        else if (rand < 74)//74% carta comum
        {
            getCard(rarity.COMUM);
        }
    }

    void getCard(rarity r)
    {
        rewardInfo rinfo = slotReward[idReward].GetComponent<rewardInfo>();

        switch (r)
        {//esta pegando as cartas q estao no gamecontroller
            case rarity.COMUM:
                rinfo.cards = _GameController.cardComum[Random.Range(0, _GameController.cardComum.Count)];
                break;
            case rarity.RARA:
                rinfo.cards = _GameController.cardRara[Random.Range(0, _GameController.cardRara.Count)];
                break;
            case rarity.EPICA:
                rinfo.cards = _GameController.cardEpica[Random.Range(0, _GameController.cardEpica.Count)];
                break;
            case rarity.LENDARIA:
                rinfo.cards = _GameController.cardLegendaria[Random.Range(0, _GameController.cardLegendaria.Count)];
                break;
        }

        rinfo.showReward(2); // ativa o 2 que e a carta
        slotReward[idReward].SetActive(true);
    }

    public void closeReward()
    {
        _GameController.changeGameState(GameState.COLLECTION);
        _GameController.panelOpenBooster.SetActive(false);
    }
}
