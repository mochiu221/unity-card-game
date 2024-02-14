using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DeckVisual : MonoBehaviour {

    public PlayerTeam playerTeam;

    [Header("UI")]
    public TextMeshProUGUI cardCountText;
    public GameObject deckCube;
    public GameObject emptyDeckImage;
    public float heightOfOneCard = 0.01f;

    private int cardsInDeck = 0;
    public int CardsInDeck
    {
        get
        {
            return cardsInDeck;
        }
        set
        {
            if (value < 0)
            {
                cardsInDeck = 0;
            }
            else
            {
                cardsInDeck = value;
            }

            // Update remaining card count in deck
            cardCountText.text = cardsInDeck.ToString();

            // Update deck visual
            if (cardsInDeck > 0)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, - heightOfOneCard * value);
                deckCube.transform.localPosition = new Vector3(deckCube.transform.localPosition.x, deckCube.transform.localPosition.y, heightOfOneCard * ( 1 + value / 2) );
                deckCube.transform.localScale = new Vector3(deckCube.transform.localScale.x, deckCube.transform.localScale.y, heightOfOneCard * value);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, - heightOfOneCard);
                deckCube.transform.localPosition = new Vector3(deckCube.transform.localPosition.x, deckCube.transform.localPosition.y, heightOfOneCard * ( 1 + 1 / 2) );
                deckCube.transform.localScale = new Vector3(deckCube.transform.localScale.x, deckCube.transform.localScale.y, heightOfOneCard);
                emptyDeckImage.SetActive(true);
            }
        }
    }
}