using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // gerenciar cenas

using System;
using System.Runtime.Serialization.Formatters.Binary; // para trabalhar com arquivos binarios
using System.IO;//para usar arquivos

public enum GameState
{
    TITLE, GAMEPLAY, COLLECTION, UPGRADE, CUT, BOOSTER
}
public enum rarity
{
    COMUM, RARA, EPICA, LENDARIA
}




public class GameController : MonoBehaviour
{
    public bool isReset;
    public bool isChosseCard; // se permite selecionar a carta do slot
    public bool isOrganizeRarity; // organizar as raridades das cartas no panelcollection

    [Header("Gerenciamento Sprites HUD")] // cabeçalho
    public Sprite IconGem;
    public Sprite IconBag;
    public Sprite[] IconCoin; // 0 - moeda inativa , 1 - moeda ativa
    public Sprite[] slotBg; // 0 - inativo, 1 - ativo
    public Sprite[] bgUpgradeHud; // 0 - inativo, 1 - ativo, 2 -maximizado
    public Color[] colorTxt;//0 - inativa, 1 - ativa
    public Sprite[] BGCard; // 0-comum, 1-raro, 2-epico, 3-lrgendario, 4-nao tem

    public cardCollection[] slotCollection;
    public cardCollection[] slotChosse;

    [Header("HUD Gameplay")]
    public GameObject panelTitle;
    public GameObject panelGameplay;
    public GameObject panelFume;
    public Text coinTxt;
    public Text gemsTxt;

    public Text coinTitleTxt;
    public Text gemsTitleTxt;

    public GameObject panelQuest;
    public Text questTxt;
    public bool isQuest; // indica se ha quest ativas
    public int idQuest; // indice da quest atual
    [TextArea] // colocando isso encima do string a area de descriçao aumenta
    public string[] questDescription; // descriçao da quest

    public GameObject btnUpgradeMode;
    public GameObject btnCollectionMode;
    public GameObject qtdBegs;
    public Text qtdBegsTxt;

    public GameObject panelCollection;

    [Header("CUT Compra")]
    public GameObject panelBuy;
    public Text buyDescriptionTxt;
    public Image iconBuild;

    public GameObject panelChosseCard;

   [Header("CUT Reward")]
    public GameObject panelReward;
    public Text rewardDescriptionTxt;
    public Image iconReward;
    private string prevWindow;

    [Header("HUD Booster")]
    public GameObject panelOpenBooster;
    public openSuitCase _openSuitCase;
    public Text boosterPriceTxt;
    public GameObject[] panelQtdBags;
    public Text[] panelQtdBagsTxt;

    [Header("Scriptables")]
    public Cards[] Cards;
    public slot[] Slot;

    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public List<Cards> cardComum;
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public List<Cards> cardRara;
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public List<Cards> cardEpica;
    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public List<Cards> cardLegendaria;

    [Header("Prefabs")]
    public GameObject coinPrefab;
    public GameObject txtPrefab;

    [Header("Variaveis de Gameplay")]
    public GameState currentState;
    private double gold, goldAccumulated;
    private int gems, gemsAccumulated;

    // determina o tempo para segurar o botao para começar o up
    //e de quanto a quanto tempo acontece esse up
    public float delayLoopUpgrade, delayBetweenUpgrade;
    public string[] accumulated;

    public int[] progressSlot;
    public int[] progressCard;

    public double[] rewardCard; // gold q vai ganhar se ja tiver a carta

    public int qtdSuitCaseComum; // quantas maletas comum ja comprou
    public double suitCasePrice; //preço inicial 
    public int[] suitCasePriceGem; // colocar para as 3 raridades

    public List<int> suitBags; // 0 - commum ,1-rar, 2-epica, 3-lendaria

    [Header("Bonus de Gameplay")]
    public int multiplierBonus;
    public int multiplierBonusTemp;
    public float reductionBonus;
    public float reductionBonusTemp;

    // [SerializeField] // para aparecer no gamecontroller da unity mesmo sendo private
    private SlotController[] _slotController;

    public Animator fadeAnimator; 

    // Start is called before the first frame update
    void Start()
    {   // esse print aparece na unity e descobre a pasta q o jogo esta sendo salvo
        print(Application.persistentDataPath);

        
        //se nao tem o save
        if (File.Exists(Application.persistentDataPath + "/saveData.dat") == false)
        {
            saveGameData();
        }
        else if (File.Exists(Application.persistentDataPath + "/saveData.dat") == true)
        {
            loadGameData();//load
        }

        foreach (Cards c in Cards)
        {

            c.isLiberate = true;

            switch (c.rarityCard)
            {
                case rarity.COMUM:
                    cardComum.Add(c);
                    break;
                case rarity.RARA:
                    cardRara.Add(c);
                    break;
                case rarity.EPICA:
                    cardEpica.Add(c);
                    break;
                case rarity.LENDARIA:
                    cardLegendaria.Add(c);
                    break;
            }
            //se nao tem o save
            if (File.Exists(Application.persistentDataPath + "/cardData" + c.idCard + ".dat") == false)
            {
                saveCard(c);
            }
            else if (File.Exists(Application.persistentDataPath + "/cardData" + c.idCard + ".dat") == true)
            {
                loadCard(c);//load
            }
        }

        foreach(slot s in Slot)
        {
            //se nao tem o save
            if (File.Exists(Application.persistentDataPath + "/slotData" + s.idSlot + ".dat") == false)
            {
                saveSlot(s);
            }
            else if (File.Exists(Application.persistentDataPath + "/slotData" + s.idSlot + ".dat") == true)
            {
                loadSlot(s);//load
            }
        }

        panelFume.SetActive(false);
        panelQuest.SetActive(false);
        panelCollection.SetActive(false);
        panelOpenBooster.SetActive(false);
        panelReward.SetActive(false);
        _openSuitCase._GameController = this; // para ter acesso 

        if (isQuest)
        {
            panelQuest.SetActive(true);
            switch (idQuest)
            {
                case 0://sem nenhum botao
                    btnCollectionMode.SetActive(false);
                    btnUpgradeMode.SetActive(false);
                    break;
                case 1://o botao de upg aparece
                    btnCollectionMode.SetActive(false);
                    btnUpgradeMode.SetActive(true);
                    break;
                case 2://continua
                    btnCollectionMode.SetActive(false);
                    btnUpgradeMode.SetActive(true);
                    break;
                case 3://os 2 da para usar
                    btnCollectionMode.SetActive(true);
                    btnUpgradeMode.SetActive(true);
                    break;
                default: // para quando estiver numa quest maior os btn estiverem abilidados
                    btnCollectionMode.SetActive(true);
                    btnUpgradeMode.SetActive(true);
                    break;
            }
            questTxt.text = questDescription[idQuest];
        }

        _slotController = FindObjectsOfType(typeof(SlotController)) as SlotController[];
        foreach ( SlotController s in _slotController) //para cada script do slot q tem essa variavel
        {
            s._GameController = this; // esta recebendo a propria instancia
            s.Slot._GameController = this;
            s.SlotStart();
        }

        
        boosterPriceTxt.text = currencyConverter(suitCasePrice);
        coinTxt.text = currencyConverter(gold);
        gemsTxt.text = gems.ToString();

        coinTitleTxt.text = currencyConverter(gold);
        gemsTitleTxt.text = gems.ToString();

        checkBags();

        fadeAnimator.SetTrigger("fadeOut");

        //      getGold(100e+10d); // para testar o jogo 
        //getGems(100000);
    }

    public void getGold(double qtd)
    {
        gold += qtd;
        if(qtd > 0)
        {
            goldAccumulated += qtd;
        }

        coinTxt.text = currencyConverter(gold);
        //0 Q
        if(isQuest == true && idQuest == 0 && goldAccumulated >= 20)
        {
            updateQuest();
        }

        saveGameData();
    }

    public void getGems(int qtd)
    {
        gems += qtd;
        if(gems > 0)
        {
            gemsAccumulated += qtd;
        }
        gemsTxt.text = gems.ToString();

        saveGameData();
    }

    public string currencyConverter(double valor)
    {
        string r = "";         // temporaria
        string valorTemp = ""; // para concatenaçao
        double temp = 0; // para fazer as divisoes

        //valor do golds e outros
        //acrescentar aa,bb ate qq
        if (valor >= 1e+12D)
        {
            temp = valor / 1e+12D;
            valorTemp = temp.ToString("N1");
            r = removeZero(valorTemp) + "T";
        }
        else if (valor >= 1e+9D) 
        {
            temp = valor / 1e+9D;
            valorTemp = temp.ToString("N1"); 
            r = removeZero(valorTemp) + "B"; 
        }
        else if (valor >= 1e+6D) 
        {
            temp = valor / 1e+6D;
            valorTemp = temp.ToString("N1"); 
            r = removeZero(valorTemp) + "M"; 
        }
        else if (valor >= 1e+3D) // 1e+3 -> 1000 ... D -> converter para double
        {
            temp = valor / 1e+3D;
            valorTemp = temp.ToString("N1"); // N1 -> uma casa decimal // se for 1 -> 1,0
            r = removeZero(valorTemp) + "K"; // para indicar q 1000 = K
        }
        else
        {
            r = valor.ToString("N0");//N0 nao mostra numero com virgula
        }



        return r;
    }

    private string removeZero(string valor)
    {
        string r = "";
        //sprite na string 1000,50 -> quebra falando o caractere especifico -> 1000 e 50
        accumulated = valor.Split(','); // , se o sistema for do brasil
        if(accumulated.Length == 1)
        {
            accumulated = valor.Split('.'); // . se fo USA
        }
        if(accumulated[1] != "0") // para remover o 0 se for tipo ,0
        {
            r = accumulated[0] + "." + accumulated[1];
        }
        else
        {
            r = accumulated[0];
        }
        return r;
    }

    IEnumerator fadePlay()
    {
        fadeAnimator.SetTrigger("fadeIn");
        yield return new WaitForSeconds(1);
        panelTitle.SetActive(false);
        fadeAnimator.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1);
        changeGameState(GameState.GAMEPLAY);
    }

    public void Jogar()
    {
        StartCoroutine("fadePlay");
    }

    public void resete()
    {
        if (File.Exists(Application.persistentDataPath + "/saveData.dat") == true)
        {
            File.Delete(Application.persistentDataPath + "/saveData.dat");
        }
        foreach (Cards c in Cards)
        {
            if (File.Exists(Application.persistentDataPath + "/cardData" + c.idCard + ".dat") == true)
            {
                File.Delete(Application.persistentDataPath + "/cardData" + c.idCard + ".dat");
            }
        }
        foreach (slot s in Slot)
        {
            if (File.Exists(Application.persistentDataPath + "/slotData" + s.idSlot + ".dat") == true)
            {
                File.Delete(Application.persistentDataPath + "/slotData" + s.idSlot + ".dat");
            }
        }

        foreach (Cards c in Cards)
        {
            c.reset();
        }
        foreach (slot s in Slot)
        {
            s.reset();
        }

        gold = 0;
        goldAccumulated = 0;
        gems = 0;
        gemsAccumulated = 0;
        isQuest = true;
        idQuest = 0;
        suitCasePrice = 1000;
        qtdSuitCaseComum = 1;
        multiplierBonus = 1;
        reductionBonus = 1;

        saveGameData();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool checkGold (double qtd)
    {
        bool check = false;

        if(gold >= qtd)
        {
            check = true;
        }
        return check;
    }

    public bool checkGem(int qtd)
    {
        bool check = false;

        if (gems >= qtd)
        {
            check = true;
        }
        return check;
    }

    public void changeGameState(GameState newState)
    {
        currentState = newState;
        foreach (SlotController s in _slotController )
        {
            s.checkGameState();
        }
    }

    public void buySlot(slot s, SlotController sc)
    {
        changeGameState(GameState.CUT);
        panelGameplay.SetActive(false);
        panelFume.SetActive(true);

        if(isChosseCard == false)
        {
            activeSlot(s, sc, s.slotCard.idCard);
        }
        else if (isChosseCard == true)
        {
            upgradeChosse(sc,s);
            panelChosseCard.SetActive(true);
        }
    } 

    public void activeSlot(slot s, SlotController sc, int idCard)
    {
        panelChosseCard.SetActive(false);
        s.slotCard = Cards[idCard];

        iconBuild.sprite = s.slotCard.spriteCard;
        buyDescriptionTxt.text = "Você liberou <color=#FFFF00>" + s.slotCard.cardName + "</color>";

        panelBuy.SetActive(true);

        getGold(s.slotPrice * -1); //tirar o dinheiro
        s.isPurchased = true;        // comprou
        s.slotCard.isLiberate = true;//liberou
        s.startSlotScriptable();     //inicializou

        sc.SlotStart();

        //2 Q
        if (isQuest == true && idQuest == 2)
        {
            updateQuest();
        }

        saveSlot(s);
    }

    public void closeCut()
    {
        changeGameState(GameState.GAMEPLAY);
        panelBuy.SetActive(false);
        panelFume.SetActive(false);
        panelGameplay.SetActive(true);
    }

    public void upgradeMode()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                changeGameState(GameState.UPGRADE);
                panelFume.SetActive(true);
                break;
            case GameState.UPGRADE:
                changeGameState(GameState.GAMEPLAY);
                panelFume.SetActive(false);
                break;
            case GameState.CUT:
                changeGameState(GameState.UPGRADE);
                break;
        }
        //chama depois q atualizou
        foreach (SlotController s in _slotController)
        {
            s.upgradeModeSlot();
        }
    }

    public void GetBooster(int boosterType)
    {
        switch (boosterType)
        {
            case 0: // maleta comum

                if(suitBags[0] <= 0) // se nao tiver maleta comum
                {
                    if (checkGold(suitCasePrice) == false) // se nao tiver dinheiro
                    {
                        return;
                    }
                    else
                    {
                        getGold(suitCasePrice * -1); // menos o q foi comprado
                        qtdSuitCaseComum += 1; // para aumentar o preço
                        suitCasePrice = (suitCasePrice * qtdSuitCaseComum) * 1.8f; // aumenta o preço da maleta  + (float)goldAccumulated / 2
                    }
                }
                else
                {
                    suitBags[0] -= 1;
                }
                
                _openSuitCase.suitCaseRarity = rarity.COMUM;
                _openSuitCase.qtdRewards = 3; // recebe as 3 recompensas aleatorias
                _openSuitCase.Start();
                
                boosterPriceTxt.text = currencyConverter(suitCasePrice);

                //3 Q
                if (isQuest == true && idQuest == 3)
                {
                    updateQuest();
                }

                break;

            case 1: // maleta rara
                if (suitBags[boosterType] <= 0) // se nao tiver maleta comum
                {
                    if (checkGem(suitCasePriceGem[boosterType]) == false) // se nao tiver dinheiro
                    {
                        return;
                    }
                    else
                    {
                        getGems(suitCasePriceGem[boosterType] * -1); // menos o q foi comprado
                    }
                }
                else
                {
                    suitBags[boosterType] -= 1;
                }
                
                _openSuitCase.suitCaseRarity = rarity.RARA;
                _openSuitCase.qtdRewards = 5; // recebe as 5 recompensas aleatorias
                _openSuitCase.Start();
                break;

            case 2: // maleta epica
                if (suitBags[boosterType] <= 0) // se nao tiver maleta comum
                {
                    if (checkGem(suitCasePriceGem[boosterType]) == false) // se nao tiver dinheiro
                    {
                        return;
                    }
                    else
                    {
                        getGems(suitCasePriceGem[boosterType] * -1); // menos o q foi comprado
                    }
                }
                else
                {
                    suitBags[boosterType] -= 1;
                }

                _openSuitCase.suitCaseRarity = rarity.EPICA;
                _openSuitCase.qtdRewards = 7; // recebe as 7 recompensas aleatorias
                _openSuitCase.Start();
                break;

            case 3: // maleta legendaria
                if (suitBags[boosterType] <= 0) // se nao tiver maleta comum
                {
                    if (checkGem(suitCasePriceGem[boosterType]) == false) // se nao tiver dinheiro
                    {
                        return;
                    }
                    else
                    {
                        getGems(suitCasePriceGem[boosterType] * -1); // menos o q foi comprado
                    }
                }
                else
                {
                    suitBags[boosterType] -= 1;
                }

                _openSuitCase.suitCaseRarity = rarity.LENDARIA;
                _openSuitCase.qtdRewards = 10; // recebe as 10 recompensas aleatorias
                _openSuitCase.Start();
                break;
        }

        checkBags();

        changeGameState(GameState.BOOSTER);
        panelOpenBooster.SetActive(true);
    }

    public void openCollection()
    {
        checkBags();

        switch (currentState)
        {
            case GameState.GAMEPLAY:
                upgradeCollection();
                changeGameState(GameState.COLLECTION);
                panelFume.SetActive(true);
                panelCollection.SetActive(true);
                break;
            case GameState.COLLECTION:
                changeGameState(GameState.GAMEPLAY);
                panelFume.SetActive(false);
                panelCollection.SetActive(false);
                break;
        }
    }

    void upgradeCollection()
    {
        int i = 0; // indice
        
        if(isOrganizeRarity == false)
        {
            foreach (Cards c in Cards) // em todos os itens da card
            {
                if (slotCollection[i]._GameController == null)
                {
                    slotCollection[i]._GameController = this; // para pegar a instancia do prorpio script
                }
                if(slotCollection[i].Card == null)
                {
                    slotCollection[i].Card = c;
                }
                slotCollection[i].upgradeInfoCard();//para atualizar as informaçoes
                slotCollection[i].gameObject.SetActive(true);//ativa

                i++;
            }
        }
        else // organizar coleçao por raridade
        {
            foreach (Cards c in cardComum) //as comuns
            {
                if (slotCollection[i]._GameController == null)
                {
                    slotCollection[i]._GameController = this; // para pegar a instancia do prorpio script
                }
                if (slotCollection[i].Card == null)
                {
                    slotCollection[i].Card = c;
                }
                slotCollection[i].upgradeInfoCard();//para atualizar as informaçoes
                slotCollection[i].gameObject.SetActive(true);//ativa

                i++;
            }
            foreach (Cards c in cardRara) // em todos os itens da coleçao
            {
                if (slotCollection[i]._GameController == null)
                {
                    slotCollection[i]._GameController = this; // para pegar a instancia do prorpio script
                }
                if (slotCollection[i].Card == null)
                {
                    slotCollection[i].Card = c;
                }
                slotCollection[i].upgradeInfoCard();//para atualizar as informaçoes
                slotCollection[i].gameObject.SetActive(true);//ativa

                i++;
            }
            foreach (Cards c in cardEpica) // em todos os itens da coleçao
            {
                if (slotCollection[i]._GameController == null)
                {
                    slotCollection[i]._GameController = this; // para pegar a instancia do prorpio script
                }
                if (slotCollection[i].Card == null)
                {
                    slotCollection[i].Card = c;
                }
                slotCollection[i].upgradeInfoCard();//para atualizar as informaçoes
                slotCollection[i].gameObject.SetActive(true);//ativa

                i++;
            }
            foreach (Cards c in cardLegendaria) // em todos os itens da coleçao
            {
                if (slotCollection[i]._GameController == null)
                {
                    slotCollection[i]._GameController = this; // para pegar a instancia do prorpio script
                }
                if (slotCollection[i].Card == null)
                {
                    slotCollection[i].Card = c;
                }
                slotCollection[i].upgradeInfoCard();//para atualizar as informaçoes
                slotCollection[i].gameObject.SetActive(true);//ativa

                i++;
            }
        }
    }

    void upgradeChosse(SlotController sc, slot s)
    {
        int i = 0; // indice

            foreach (Cards c in Cards) // em todos os itens da card
            {
                if (slotChosse[i]._GameController == null)
                {
                    slotChosse[i]._GameController = this; // para pegar a instancia do prorpio script
                }
                if (slotChosse[i].Card == null)
                {
                    slotChosse[i].Card = c;
                }

                slotChosse[i].sc = sc;
                slotChosse[i].s = s;

                slotChosse[i].upgradeInfoCard();//para atualizar as informaçoes

                if(slotChosse[i].Card.isLiberate == true)
                {
                    slotChosse[i].gameObject.SetActive(true);//ativa
                }

                 i++;
            }
    }

    public double getGoldAccumulated()
    {
        return goldAccumulated;
    }

    public void getCard(Cards c, int qtd)
    {
        if (c.isLiberate == false)
        {
            c.isLiberate = true;
        }
        if(c.isMax == false)
        {
            c.cardCollected += qtd;
            if(c.cardCollected >= progressCard[c.levelCard - 1]) // se tem carta suficiente para passar de level
            {
                
                int dif = c.cardCollected - progressCard[c.levelCard - 1]; // para zerar
                c.cardCollected = dif;
                c.levelCard += 1;

                switch (c.levelCard)
                {
                    case 2:
                        c.productionMultiplier = 5;
                        break;
                    case 3:
                        c.productionReduction = 5;
                        c.isMax = true;

                        if(dif > 0)
                        {
                            rewardRarity(c.rarityCard);
                        }
                        c.cardCollected = 0;
                        break;
                }
                updateSlots();

                openCutReward(c.spriteCard, "<color=#FFFF00>2"+c.cardName+ "</color> passou de nível", "booster");
            }
        }
        else
        { // dar uma outra recompensa alem da carta ja q esta maximizada
            rewardRarity(c.rarityCard);
        }
        
        upgradeCollection();
        saveCard(c);
    }

    private void rewardRarity(rarity r)
    {
        switch (r)
        {
            case rarity.COMUM:
                getGold(rewardCard[0]);
                break;
            case rarity.RARA:
                getGold(rewardCard[1]);
                break;
            case rarity.EPICA:
                getGold(rewardCard[2]);
                break;
            case rarity.LENDARIA:
                getGold(rewardCard[3]);
                break;
        }
    }

    void updateSlots() // atualizar o slot
    {
        foreach(slot s in Slot)
        {
            if(s._GameController == null)
            {
                s._GameController = this; // para ter acesso ao gamecontroller
            }
            s.startSlotScriptable();
        }
    }

    public void updateQuest()
    {
        idQuest += 1;
        switch (idQuest)
        {
            case 1:// consegue usar o btn do upg se completar a mission
                btnUpgradeMode.SetActive(true);
                break;
            case 3:
                btnCollectionMode.SetActive(true);
                break;
        }
        if(idQuest < questDescription.Length)
        {
            questTxt.text = questDescription[idQuest];
        }
        else
        {
            panelQuest.SetActive(false);
            isQuest = false;
        }

        saveGameData();
    }

    public void openCutReward(Sprite ico, string txt, string prev)
    {
        prevWindow = prev;

        iconReward.sprite = ico;
        rewardDescriptionTxt.text = txt;

        panelGameplay.SetActive(false);
        panelFume.SetActive(true);
        panelReward.SetActive(true);

        changeGameState(GameState.CUT);
    }

    public void closeCutReward()
    {
        panelReward.SetActive(false);

        switch (prevWindow)
        {
            case "upgrade":
                panelGameplay.SetActive(true);
                upgradeMode();
                break;
            case "booster":
                panelGameplay.SetActive(true);
                changeGameState(GameState.BOOSTER);
                break;
        }
    }

    public void checkBags() 
    {
        //para aparecer a quantidade de maletas q possui encima da maleta
        int q = 0;
        foreach (int ii in suitBags)
        {
            q += ii;
        }
        if (q > 0)
        {
            qtdBegs.SetActive(true);
        }
        else
        {
            qtdBegs.SetActive(false);
        }
        qtdBegsTxt.text = q.ToString();

        // para colocar o numero de maletas q cada raridade possui no paenlCollection(booster)
        int i = 0;
        foreach (int b in suitBags)
        {
            panelQtdBagsTxt[i].text = b.ToString();
            if (b > 0)
            {
                panelQtdBags[i].SetActive(true);
            }
            else
            {
                panelQtdBags[i].SetActive(false);
            }
            i++;
        }
    }

    public void getBag(int tipo, int qtd)
    {
        suitBags[tipo] += qtd;
        checkBags();
        string tipoMaleta = "";

        switch (tipo)
        {
            case 0:
                tipoMaleta = "Maleta Comum";
                break;
            case 1:
                tipoMaleta = "Maleta Rara";
                break;
            case 2:
                tipoMaleta = "Maleta Épica";
                break;
            case 3:
                tipoMaleta = "Maleta Lendária";
                break;

        }

        openCutReward(IconBag, "Você recebeu <color=#FFFF00>" + qtd + "</color>" + tipoMaleta, "upgrade");
    }

    public void saveGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat"); // o lugar q salva
        saveDataGame data = new saveDataGame(); // para instanciar

        data.gold = gold;
        data.goldAccumulated = goldAccumulated;
        data.gems = gems;
        data.gemsAccumulated = gemsAccumulated;
        //data.qtdSuitCaseComum = qtdSuitCaseComum;
        data.suitCasePrice = suitCasePrice;
        data.multiplierBonus = multiplierBonus;
        data.reductionBonus = reductionBonus;
        data.isQuest = isQuest;
        data.idQuest = idQuest;

        data.suitBags = new List<int>();
        data.suitBags.Clear(); // limpar
        foreach (int i in suitBags) // salvar cada 
        {
            data.suitBags.Add(i);
        }

        bf.Serialize(file, data);//seriarizando colocando essas informaçoes nesse arquivo
        file.Close();
    }

    public void loadGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open); // o lugar q salva
        saveDataGame data = (saveDataGame)bf.Deserialize(file);//deserializando e coloca na data

        gold = data.gold;
        goldAccumulated = data.goldAccumulated;
        gems = data.gems;
        gemsAccumulated = data.gemsAccumulated;
        //qtdSuitCaseComum = data.qtdSuitCaseComum;
        suitCasePrice = data.suitCasePrice;
        multiplierBonus = data.multiplierBonus;
        reductionBonus = data.reductionBonus;
        isQuest = data.isQuest;
        idQuest = data.idQuest;

        suitBags.Clear(); // limpar
        foreach (int i in data.suitBags) 
        {
            suitBags.Add(i);
        }

        file.Close();
    }

    public void saveCard(Cards c)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/cardData" + c.idCard + ".dat"); // o lugar q salva
        saveCardData data = new saveCardData(); // para instanciar

        data.isLiberate = c.isLiberate;
        data.levelCard = c.levelCard;
        data.cardCollected = c.cardCollected;
        data.productionMultiplier = c.productionMultiplier;
        data.productionReduction = c.productionReduction;
        data.isMax = c.isMax;

        bf.Serialize(file, data);//seriarizando colocando essas informaçoes nesse arquivo
        file.Close();
    }

    public void loadCard(Cards c)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/cardData" + c.idCard + ".dat", FileMode.Open); // o lugar q salva
        saveCardData data = (saveCardData)bf.Deserialize(file);//deserializando e coloca na data

        c.isLiberate = data.isLiberate;
        c.levelCard = data.levelCard;
        c.cardCollected = data.cardCollected;
        c.productionMultiplier = data.productionMultiplier;
        c.productionReduction = data.productionReduction;
        c.isMax = data.isMax;

        file.Close();
    }

    public void saveSlot(slot s)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/slotData" + s.idSlot + ".dat"); // o lugar q salva
        saveDataSlot data = new saveDataSlot(); // para instanciar

        data.idCard = s.slotCard.idCard;
        data.isPurchased = s.isPurchased;
        data.isMax = s.isMax;
        data.isAutoProduction = s.isAutoProduction;
        data.slotLevel = s.slotLevel;
        data.upgrades = s.upgrades;
        data.totalUpgrades = s.totalUpgrades;
        //data.upgradePrice = s.upgradePrice;
        data.slotProductionMultiplier = s.slotProductionMultiplier;
        data.slotProductionReduction = s.slotProductionReduction;

        bf.Serialize(file, data);//seriarizando colocando essas informaçoes nesse arquivo
        file.Close();
    }

    public void loadSlot(slot s)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/slotData" + s.idSlot + ".dat", FileMode.Open); // o lugar q salva
        saveDataSlot data = (saveDataSlot)bf.Deserialize(file);//deserializando e coloca na data

        s.slotCard = Cards[data.idCard]; // vai pegar o id da gamemanager
        s.isPurchased = data.isPurchased;
        s.isMax = data.isMax;
        s.isAutoProduction = data.isAutoProduction;
        s.slotLevel = data.slotLevel;
        s.upgrades = data.upgrades;
        s.totalUpgrades = data.totalUpgrades;
        //s.upgradePrice = data.upgradePrice;
        s.slotProductionMultiplier = data.slotProductionMultiplier;
        s.slotProductionReduction = data.slotProductionReduction;

        file.Close();
    }
}

[Serializable] // pode seriarizar
class saveDataGame // criar todas as avriaveis q quer deixar salva
{
    public double gold, goldAccumulated;
    public int gems, gemsAccumulated;
    public int qtdSuitCaseComum; // quantas maletas comum ja comprou
    public double suitCasePrice; //preço inicial 
 
    public int multiplierBonus;
    public float reductionBonus;

    public List<int> suitBags; // 0 - commum ,1-rar, 2-epica, 3-lendaria

    public bool isQuest;
    public int idQuest;

}

[Serializable] 
class saveCardData
{
    public bool isLiberate;
    public int levelCard;
    public int cardCollected;

    public int productionMultiplier; 
    public float productionReduction; 

    public bool isMax;

}

[Serializable]
class saveDataSlot
{
    public int idCard;

    public bool isPurchased; //se ja comprou terreno  
    public bool isMax;      // se ja esta maximizado
    public bool isAutoProduction; // se ja tem autoproduçao

    public int slotLevel;
    public int upgrades; // quantas vezes passou de nivel
    public int totalUpgrades;

    public double upgradePrice;

    public int slotProductionMultiplier; //conforme passa de nivel tem um bonus de multiplicador
    public float slotProductionReduction;
}
