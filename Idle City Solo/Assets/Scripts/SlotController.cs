using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    


    [HideInInspector] // para nao aparecer na unity para ninguem mexer
    public GameController _GameController;

    [Header("HUD Terreno")]
    public GameObject huds;
    public SpriteRenderer bgSlot;
    public SpriteRenderer buildSprite;
    public Transform hudPosition;

   [Header("HUD Produçao")]
    public GameObject panelProduction;
    public Image iconCoinProduction;
    public Image loadBar;
    public Text productionTxt;

    [Header("Painel Compra")]
    public GameObject panelPurchase;
    public Image iconCoinPurchase;
    public Text pricePurchaseTxt;

    [Header("HUD Upgrade")]
    public GameObject panelUpgrade;
    public Image bgUpgrade;
    public Image progressBar;
    public Text progressTxt;
    public Image iconCoinUpgrade;
    public Text priceUpgradeTxt;
    public Text slotLevelTxt;

    [Header("GamePlay")]
    public slot Slot;
    private double gold;
    private float tempTime; // contabilizar o tempo passando
    private float fillAmount; // barra de load 

    private bool isInitialized;
    private bool isLoop;

    private Animator anime;
    public AudioSource audioCoin;

    // Start is called before the first frame update
    public void SlotStart()
    {
        huds.transform.position = hudPosition.position; // os paineis(3) vao para a posi da contruçao 
        Slot.startSlotScriptable();

        anime = GetComponent<Animator>();

        //gerenciamento dos Huds
        if (Slot.isPurchased == false)
        {
            panelProduction.SetActive(false);
            panelUpgrade.SetActive(false);
            panelPurchase.SetActive(true);
            buildSprite.enabled = false;
            pricePurchaseTxt.text = _GameController.currencyConverter(Slot.slotPrice);
        }
        else if(Slot.isPurchased == true)
        {
            panelProduction.SetActive(true);
            panelUpgrade.SetActive(false);
            panelPurchase.SetActive(false);

            buildSprite.sprite = Slot.slotCard.spriteCard;
            buildSprite.enabled = true;
        }

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInitialized == false)
        {
            return;
        }
        if(Slot.isPurchased == true)//se tiver comprado
        {
            if(gold == 0 /*&& Slot.isAutoProduction == false */)
            {
                goldProduction();
            }
            else if(gold > 0 && Slot.isAutoProduction == true)
            {
                goldProduction();
            }

            upgradeHudSlot();
        }
        else
        {
            if(_GameController.checkGold(Slot.slotPrice) == true)
            {
                bgSlot.sprite = _GameController.slotBg[1];
                iconCoinPurchase.sprite = _GameController.IconCoin[1];
                pricePurchaseTxt.color = _GameController.colorTxt[1];
            }
            else //se nao tem o dinheiro suficiente para comprar
            {
                bgSlot.sprite = _GameController.slotBg[0];
                iconCoinPurchase.sprite = _GameController.IconCoin[0];
                pricePurchaseTxt.color = _GameController.colorTxt[0];
                
            }
        }
    }

    void goldProduction()
    {
        tempTime += Time.deltaTime; // calcula o tempo passado no flame

        if(Slot.slotTimeProduction <= 0.1f)
        {
            fillAmount = 1; // para nao ficar bugando no load se chegar no 1 

        }
        else
        {
            fillAmount = tempTime / Slot.slotTimeProduction;
        }

        loadBar.fillAmount = fillAmount;

        if(tempTime >= Slot.slotTimeProduction)//maio q o tempo de produçao
        {
            tempTime = 0;
            gold += Slot.slotProduction;
            productionTxt.text = _GameController.currencyConverter(gold);//atualiza o valor

        }
        if(gold > 0)
        {
            iconCoinProduction.gameObject.SetActive(true);
        }
        else
        {
            iconCoinProduction.gameObject.SetActive(false);
        }
    }

    void goldCollect()
    {
        if(gold <= 0)
        {
            return;
        }
        _GameController.getGold(gold);
        

        //Instancia Coin , para adicionar informaçao e força
        GameObject tempCoin = Instantiate(_GameController.coinPrefab, hudPosition.position, hudPosition.localRotation);
        tempCoin.GetComponent<coinAnimation>().posY = hudPosition.position.y;
        tempCoin.GetComponent<Rigidbody2D>().AddForce(new Vector2(35, 400));
        anime.SetTrigger("colect");
        audioCoin.Play();

        //Instancia TXT
        GameObject tempTXT = Instantiate(_GameController.txtPrefab, hudPosition.position + new Vector3(0,0.2f,0), hudPosition.localRotation);
        textAnimation t = tempTXT.GetComponent<textAnimation>();
        t.production.text = "+" + _GameController.currencyConverter(gold);
        t.shadow.text = "+" + _GameController.currencyConverter(gold);
        t.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 200));

        gold = 0;
        productionTxt.text = "0";
    }

    private void upgradeHudSlot()//atualizando
    {
        slotLevelTxt.text = Slot.slotLevel.ToString();
        priceUpgradeTxt.text = _GameController.currencyConverter(Slot.upgradePrice);
        progressTxt.text = Slot.totalUpgrades.ToString();

        float fillAmount = 0;
        if(Slot.upgrades > 0)
        {
            fillAmount = (float)Slot.upgrades / _GameController.progressSlot[Slot.slotLevel - 1];
        }

        progressBar.fillAmount = fillAmount;

        if(Slot.isMax == true)
        {
            bgUpgrade.sprite = _GameController.bgUpgradeHud[2]; // o 2 e o maximizado
        }
        else
        {
            if (_GameController.checkGold(Slot.upgradePrice) == true)//se tem dinheiro para evoluir
            {
                bgUpgrade.sprite = _GameController.bgUpgradeHud[1];
                priceUpgradeTxt.color = _GameController.colorTxt[1];
                iconCoinUpgrade.sprite = _GameController.IconCoin[1];
            }
            else
            {
                bgUpgrade.sprite = _GameController.bgUpgradeHud[0];
                priceUpgradeTxt.color = _GameController.colorTxt[0];
                iconCoinUpgrade.sprite = _GameController.IconCoin[0];
            }
                
        }
    }

    public void upgradeModeSlot() //  e chamando o momdo
    {
        if(Slot.isPurchased == false) // se ainda nao tem o terreno
        {
            return;
        }
        upgradeHudSlot();

        switch (_GameController.currentState)
        {
            case GameState.UPGRADE:
                panelProduction.SetActive(false);
                panelUpgrade.SetActive(true);
                break;
            case GameState.GAMEPLAY:
                panelProduction.SetActive(true);
                panelUpgrade.SetActive(false);
                break;
        }
    }

    public void checkGameState()
    {
        switch (_GameController.currentState)
        {
            case GameState.GAMEPLAY:
                huds.SetActive(true);
   
                switch (Slot.isPurchased)
                {
                    case true:
                        panelProduction.SetActive(true);
                        panelUpgrade.SetActive(false);
                        panelPurchase.SetActive(false);
                        break;
                    case false:
                        panelProduction.SetActive(false);
                        panelPurchase.SetActive(true);
                        panelUpgrade.SetActive(false);
                        break;
                }
                break;

            case GameState.CUT:
                huds.SetActive(false);
                break;

            case GameState.COLLECTION:
                huds.SetActive(false);
                break;

            case GameState.UPGRADE:
                huds.SetActive(true);

                switch (Slot.isPurchased)
                {
                    case true:
                        panelUpgrade.SetActive(true);
                        panelPurchase.SetActive(false);
                        break;
                    case false:
                        panelUpgrade.SetActive(false);
                        panelPurchase.SetActive(true);
                        break;
                }
                break;
        }
    }

    public void upgradeSlot() // up mesmo 
    {
        if (Slot.isMax == true)
        {
            return;
        }

        Slot.upgrades += 1;
        Slot.totalUpgrades += 1;

        _GameController.getGold(Slot.upgradePrice * -1); // para tirar dinheiro

        if(Slot.slotLevel == 1 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(0);

            //1 Q
            if (_GameController.isQuest == true && _GameController.idQuest == 1)
            {
                _GameController.updateQuest();
            }

        }
        else if (Slot.slotLevel == 2 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(1);
            Slot.isAutoProduction = true;
            _GameController.getBag(0,2); // ganha 2 maletas comuns
        }
        else if (Slot.slotLevel == 3 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(0);
            
        }
        else if (Slot.slotLevel == 4 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(1);
            _GameController.getGems(25);

            _GameController.openCutReward(_GameController.IconGem, "Você recebeu <color=#FFFF00>25</color> gemas","upgrade");

            

        }
        else if (Slot.slotLevel == 5 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(0);
            
        }
        else if (Slot.slotLevel == 6 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(1);

        }
        else if (Slot.slotLevel == 7 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(0);

        }
        else if (Slot.slotLevel == 8 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(1);

        }
        else if (Slot.slotLevel == 9 && Slot.upgrades >= _GameController.progressSlot[Slot.slotLevel - 1])
        {
            upgradeStatus(0);
            upgradeStatus(1);
            Slot.isMax = true;
            //esta no 9 e esta indo para o 10
        }
        


        Slot.startSlotScriptable();// para usar a funçao q calcula devolta (recalcular)

        upgradeHudSlot();// para atualizar

        _GameController.saveSlot(Slot);
    }

    private void upgradeStatus(int i)
    {
        //i = 0 , aumenta o multiplicador
        //i = 1 , aumenta o redutor
        Slot.slotLevel += 1;
        Slot.upgrades = 0; // para a barinha de progresso voltar a zero

        switch (i)
        {
            case 0:
                if(Slot.slotLevel == 2)
                {
                    Slot.slotProductionMultiplier += 1;
                }
                else
                {
                    Slot.slotProductionMultiplier += 2;
                }
                break;
            case 1:
                if (Slot.slotLevel == 3)
                {
                    Slot.slotProductionReduction += 1;
                }
                else
                {
                    Slot.slotProductionReduction += 2;
                }
                break;

        }
    }

    private void OnMouseEnter()
    {
        if(_GameController.currentState == GameState.GAMEPLAY && Slot.isPurchased == true)
        {
            goldCollect();
        }
    }

    private void OnMouseDown()
    {
        if (_GameController.currentState == GameState.GAMEPLAY && Slot.isPurchased == true)
        {
            goldCollect();
        }
        else if (_GameController.currentState == GameState.GAMEPLAY && Slot.isPurchased == false && _GameController.checkGold(Slot.slotPrice) == true)
        {
            _GameController.buySlot(Slot, this);
        }
    }

    public void OnPointerDown() //enquanto estiver clicando 
    {
        StartCoroutine("loopUpgrade");
    }

    public void OnPointerUp()// se solta para
    {
        StopCoroutine("loopUpgrade");
        isLoop = false;
    }

    IEnumerator loopUpgrade()
    {    //se tem dinheiro e se nao e maximo
        if(_GameController.checkGold(Slot.upgradePrice) == true && Slot.isMax == false)
        {
            upgradeSlot(); // chama a atualizaçao
        }
        if(isLoop == false)
        {
            //quanto tempo vai esperar antes de começar o proximo comando
            yield return new WaitForSeconds(_GameController.delayLoopUpgrade);
            isLoop = true;
        }

        yield return new WaitForSeconds(_GameController.delayBetweenUpgrade);
        
        if(isLoop == true && _GameController.currentState == GameState.UPGRADE)
        {
            StartCoroutine("loopUpgrade");
        }
        else//para parar o loop quando abrir outras janelas de menu
        {
            isLoop = false;
        }
    }
}
