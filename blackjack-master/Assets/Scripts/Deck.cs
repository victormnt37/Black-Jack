using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;


    public int[] values = new int[52];
    int cardIndex = 0;

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }
    private void InitCardValues()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            int cardNumber = i % 13; // Determina el número de la carta (0-12)


            if (cardNumber == 0 || cardNumber == 13 || cardNumber == 26 || cardNumber == 39)
            {
                values[i] = 11;
            }
            else if (cardNumber < 9)
            {
                values[i] = cardNumber + 1; // Las cartas del 2 al 10 tienen el valor de su número
            }
            else
            {
                values[i] = 10; // Las cartas J, Q, K tienen valor 10
            }
        }
    }
    private void ShuffleCards()
    {
        System.Random rng = new System.Random();
        int n = faces.Length;
        while (n > 1)
        {
            n--;

            // Generamos un número aleatorio en el rango [0, n]
            int k = rng.Next(n + 1);

            // Guardamos temporalmente la cara de la carta en la posición k
            Sprite tempFace = faces[k];
            int tempValue = values[k];

            // Intercambiamos la cara de la carta en la posición k con la cara de la carta en la posición n
            faces[k] = faces[n];
            values[k] = values[n];

            // Colocamos la carta temporalmente guardada en la posición n
            faces[n] = tempFace;
            values[n] = tempValue;
        }
    }


    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }
        if (player.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points == 21)
        {
            EndGame();
        }

    }

    private void CalculateProbabilities()
    {
        // Obtener la puntuación actual del jugador y la carta visible del crupier
        int playerPoints = player.GetComponent<CardHand>().points;
        if (dealer.GetComponent<CardHand>().cards.Count > 1)
        {


            // Calcular la probabilidad de que el crupier tenga más puntuación que el jugador
            float probabilityDealerWins = CalculateProbabilityDealerWins(playerPoints);

            // Calcular la probabilidad de que el jugador obtenga entre 17 y 21 si pide una carta
            float probabilityPlayer17to21 = CalculateProbabilityPlayer17to21(playerPoints);

            // Calcular la probabilidad de que el jugador obtenga más de 21 si pide una carta
            float probabilityPlayerBust = CalculateProbabilityPlayerBust(playerPoints);

            // Actualizar el texto del objeto de texto con las probabilidades calculadas
            probMessage.text = "Deal > Play " + probabilityDealerWins.ToString("P2") +
                            "\n 17 <= x <= 21: " + probabilityPlayer17to21.ToString("P2") +
                            "\n x > 21: " + probabilityPlayerBust.ToString("P2");
        }
    }

    public float CalculateProbabilityDealerWins(int playerPoints)
    {
        int dealerVisibleCardValue = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        int dealerHiddenCardValue = dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().value;

        int casosfavorables = 0;

        int totalDealerPoints = dealerVisibleCardValue + dealerHiddenCardValue;
        if (totalDealerPoints > playerPoints) { casosfavorables++; }

        int casosgeneral = 1;

        foreach (int cardValue in values)
        {
            if (dealerVisibleCardValue + cardValue > playerPoints)
            {
                casosfavorables++;
            }
            casosgeneral++;
        }

        // Probabilidad de que el dealer gane
        float probabilityDealerWins = (float)casosfavorables / casosgeneral;

        Debug.Log(probabilityDealerWins);

        return probabilityDealerWins;
    }

    public float CalculateProbabilityPlayer17to21(int playerPoints)
    {
        float probabilidadTotal = 0f;
        int casosFavorables = 0;
        int casos = 0;

        // Calcular el número de cartas restantes de cada valor deseado
        foreach (int valor in values)
        {
            if (valor + playerPoints >= 17 && valor + playerPoints <= 21)
            {
                casosFavorables++;
            }
            casos++;

        }
        probabilidadTotal = (float)casosFavorables / casos;
        Debug.Log(probabilidadTotal);

        return probabilidadTotal;
    }
    public float CalculateProbabilityPlayerBust(int playerPoints)
    {
        float probabilidadTotal = 0f;
        int casosFavorables = 0;
        int casos = 0;

        // Calcular el número de cartas restantes de cada valor deseado
        foreach (int valor in values)
        {
            int totalPuntos = playerPoints + valor;

            // Contabilizar las cartas que llevarían al jugador a exceder 21 puntos
            if (totalPuntos > 21)
            {
                casosFavorables++;
            }

            // Contabilizar todas las cartas restantes
            casos++;
        }

        probabilidadTotal = (float)casosFavorables / casos;
        Debug.Log(probabilidadTotal);

        return probabilidadTotal;
    }



    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        //Repartimos carta al jugador
        PushPlayer();

        // Comprobar si el jugador se pasa de 21
        int playerPoints = player.GetComponent<CardHand>().points;
        if (playerPoints > 21)
        {
            // El jugador se pasa de 21, terminar el juego
            EndGame();
        }
    }

    public void Stand()
    {
        // Bloquear el botón "Hit"
        hitButton.interactable = false;

        // Repartir cartas al crupier si tiene 16 puntos o menos
        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }

        // Finalizar el juego
        EndGame();
    }


    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    void EndGame()
    {
        // Desactivar los botones de hit y stand
        hitButton.interactable = false;
        stickButton.interactable = false;

        // Mostrar la carta oculta del crupier
        dealer.GetComponent<CardHand>().InitialToggle();

        // Obtener las puntuaciones del jugador y el crupier
        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerPoints = dealer.GetComponent<CardHand>().points;

        // Mostrar por pantalla quién ha ganado
        if (playerPoints > 21)
        {
            finalMessage.text = "El dealer ha ganado."; // El jugador se pasa de 21
        }
        else if (dealerPoints > 21)
        {
            finalMessage.text = "El jugador ha ganado."; // El crupier se pasa de 21
        }
        else if (playerPoints > dealerPoints)
        {
            finalMessage.text = "El jugador ha ganado."; // El jugador tiene más puntos que el crupier
        }
        else if (playerPoints < dealerPoints)
        {
            finalMessage.text = "El dealer ha ganado."; // El crupier tiene más puntos que el jugador
        }
        else
        {
            finalMessage.text = "Empate."; // Ambos tienen la misma cantidad de puntos
        }
    }


}
