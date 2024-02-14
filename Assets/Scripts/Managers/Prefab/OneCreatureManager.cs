using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OneCreatureManager : MonoBehaviour
{
    public CardAsset cardAsset;
    public CharacterAsset charAsset;
    public PlayerTeam playerTeam;
    public CreatureType creatrueType;

    [Header("Text Component References")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;

    [Header("GameObject References")]
    public GameObject healthIcon;
    public GameObject attackIcon;

    [Header("Image References")]
    public Image cardIllustration;
    public GameObject creatureGlowImage;
    public GameObject creatureSelectedImage;
    public Image frameImage;
    public Image crownImage;
    public Image hidingImage;
    public Image allureImage;

    // Logic
    private bool selectedNow = false;
    public bool SelectedNow
    {
        get
        {
            return selectedNow;
        }
        set
        {
            selectedNow = value;
            creatureSelectedImage.SetActive(value);
        }
    }

    private void Awake()
    {
        if (cardAsset != null)
        {
            ReadCreatureFromAsset();
        }
        else if (charAsset != null)
        {
            ReadCharacterFromAsset();
        }
    }

    private bool canAttackNow = false;
    public bool CanAttackNow
    {
        get
        {
            return canAttackNow;
        }

        set
        {
            canAttackNow = value;

            if (playerTeam == PlayersManager.Instance.myPlayer.playerTeam)
            {
                creatureGlowImage.SetActive(value);
            }
        }
    }

    private bool hiding;
    public bool Hiding
    {
        get { return hiding; }
        set
        {
            hiding = value;
            hidingImage.enabled = value;
        }
    }

    private bool allure;
    public bool Allure
    {
        get { return allure; }
        set
        {
            allure = value;
            allureImage.enabled = value;
        }
    }

    public void ReadCreatureFromAsset()
    {
        // 1) Add illustration
        cardIllustration.sprite = cardAsset.creatureIllustration;
        // 2) Add health
        healthText.text = cardAsset.maxHealth.ToString();
        // 3) Add attack
        attackText.text = cardAsset.attack.ToString();
        // 4) Set creature type
        creatrueType = cardAsset.creatureType;

        Hiding = cardAsset.hide;
    }

    public void ReadCharacterFromAsset()
    {
        // 1) Add illustration
        cardIllustration.sprite = charAsset.charIllustration;
        // 2) Add health
        healthText.text = charAsset.maxHealth.ToString();
        // 3) Add attack
        attackText.text = charAsset.attack.ToString();
        // 4) Set creature type
        creatrueType = CreatureType.King;
    }

    public void UpdateHealthValue(int amount, int healthAfter)
    {
        if (amount < 0)
        {
            healthText.text = healthAfter.ToString();
        }
        else if (amount > 0)
        {
            healthText.text = healthAfter.ToString();
        }
    }

    public void UpdateAttackValue(int amount, int attackAfter)
    {
        if (amount < 0)
        {
            attackText.text = attackAfter.ToString();
        }
        else if (amount > 0)
        {
            attackText.text = attackAfter.ToString();
        }
    }

}
