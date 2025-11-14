using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum CardState
{
    OPEN,
    CLOSE
}

public class Card : MonoBehaviour
{
    [Header("Card Info")]
    [SerializeField] private CardState currentState = CardState.OPEN;
    [SerializeField] private string cardName;
    [SerializeField] private int cardValue;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer currentSprite;
    [SerializeField] private Sprite cardOpenSprite;
    [SerializeField] private Sprite cardCloseSprite;

    [Header("Events")]
    [SerializeField] private GameEvent OnDoneMovingCard;
    [SerializeField] private float moveSpeed = 6f;

    private static readonly Dictionary<string, int> cardValues = new Dictionary<string, int>()
    {
        { "Ace", 11 },
        { "Jack", 10 },
        { "Queen", 10 },
        { "King", 10 }
    };

    private bool doneMoving = false;
    private bool movingCard = false;

    private Vector3 targetMove;

    private void Update()
    {
        if (movingCard)
            MovingCard();
    }

    public void MoveCard(Vector3 target)
    {
        targetMove = target;
        movingCard = true;
    }

    private void MovingCard()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetMove, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetMove) < 0.001f)
        {
            movingCard = false;
            doneMoving = true;

            transform.position = targetMove;

            if (currentState == CardState.OPEN)
                OnDoneMovingCard.Raise();

            switch (SingletonHub.Instance.Get<GameManager>().GetTurnState())
            {
                case TurnState.PLAYER:
                    SingletonHub.Instance.Get<UIManager>().ShowHitStandButton(true);
                    break;
                case TurnState.DEALER:
                    SingletonHub.Instance.Get<GameManager>().GiveDealerCard();
                    break;
            }
        }
    }

    public void ChangeShade()
    {
        currentSprite.color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void SetupCard(Sprite cardSprite, CardState state)
    {
        cardOpenSprite = cardSprite;
        currentState = state;

        InitializeCardValue();

        if (currentState == CardState.CLOSE)
            CloseCard();
        else
            OpenCard();
    }

    private string CleanCardName(string input)
    {
        string result = input.Replace("Poker Card", "")
                         .Replace("(Clone)", "")
                         .Trim();

        return result;
    }

    private void InitializeCardValue()
    {
        string cleaned = CleanCardName(cardOpenSprite.name);

        string[] parts = cleaned.Split(' ');
        string rank = parts[0];

        if (cardValues.TryGetValue(rank, out int value))
        {
            cardValue = value;
        }
        else
        {
            if (int.TryParse(rank, out int numberValue))
                cardValue = numberValue;
            else
                cardValue = 0;
        }

        cardName = cleaned;
    }

    public void SetCardState(CardState cardState) => currentState = cardState;

    public int GetCardValue() => cardValue;
    public CardState GetCardState() => currentState;
    public void CloseCard() => currentSprite.sprite = cardCloseSprite;
    public void OpenCard()
    {
        currentSprite.sprite = cardOpenSprite;
        if (doneMoving)
            OnDoneMovingCard.Raise();
    }
}
