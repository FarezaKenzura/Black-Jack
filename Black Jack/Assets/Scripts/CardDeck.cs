using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class CardDeck : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private List<Sprite> cardDeck = new List<Sprite>();

    public void BuildDeck()
    {
        cardDeck.Clear();

        int count = atlas.spriteCount;
        Sprite[] cardArray = new Sprite[count];

        atlas.GetSprites(cardArray);

        cardDeck.AddRange(cardArray);
    }

    public void Shuffle()
    {
        int n = cardDeck.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            Sprite temp = cardDeck[i];
            cardDeck[i] = cardDeck[randomIndex];
            cardDeck[randomIndex] = temp;
        }
    }

    public Sprite GetCard()
    {
        Sprite card = cardDeck[0];
        cardDeck.Remove(card);
        return card;
    }
}