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


            if (cardNumber==0 || cardNumber ==13 || cardNumber ==26 || cardNumber ==39) 
            {
                values[i] = 11;
            }
            else if(cardNumber < 9)
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
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
        if (player.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points == 21)
        {
            EndGame();
        }

    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
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
