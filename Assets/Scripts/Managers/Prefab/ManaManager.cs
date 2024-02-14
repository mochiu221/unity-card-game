using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class ManaManager : MonoBehaviour
{
    public int TestFullMana;
    public int TestTotalManaThisTurn;


    public int maxTotalMana = 99;
    private int totalMana;
    public int TotalMana
    {
        get{ return totalMana; }

        set
        {
            // Debug.Log("Changed total mana to: " + value);

            if (value > maxTotalMana)
                totalMana = maxTotalMana;
            else if (value < 0)
                totalMana = 0;
            else
                totalMana = value;

            // Update the text
            ProgressText.text = string.Format("{0}\n/{1}", availableMana.ToString(), totalMana.ToString());
        }
    }

    private int availableMana;
    public int AvailableMana
    {
        get{ return availableMana; }

        set
        {
            // Debug.Log("Changed mana this turn to: " + value);

            if (value > totalMana)
                availableMana = totalMana;
            else if (value < 0)
                availableMana = 0;
            else
                availableMana = value;

            // Update the text
            ProgressText.text = string.Format("{0}\n/{1}", availableMana.ToString(), totalMana.ToString());

        }
    }
    public TextMeshProUGUI ProgressText;

    // Testing
    /*
    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            TotalMana = TestTotalManaThisTurn;
            AvailableMana = TestFullMana;
        }
    }
    */
}
