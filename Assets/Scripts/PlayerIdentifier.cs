using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1; // 1 for Player 1, 2 for Player 2

    public int PlayerNumber => playerNumber;
}
