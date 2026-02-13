using UnityEngine;

public class PlayerManager : MonoBehaviour  // OBSULITE !!!!
{
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Direction playerDirection;
    [SerializeField] private int playerSpeed;

    public Direction LocalPlayerDirection { get { return playerDirection; } set { playerDirection = value; } }
    public int LocalPlayerSpeed { get { return playerSpeed; } set { playerSpeed = value; } }
    public PlayerData m_PlayerData { get { return m_playerData; } set { m_playerData = value; } }

    /// data terlalu kepisah brow
    /// tapi juga kerasa digabung
    /// TOOD:
    /// benerin arsitektur Player
    /// 
}
