using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    // player transform
    // sprite
    // idle, walk
    [SerializeField] private Direction playerDirection;
    [SerializeField] private int playerSpeed;

    public Direction PlayerDirection { get { return playerDirection; } set { playerDirection = value; } }
    public int PlayerSpeed { get { return playerSpeed; } }
}

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right
}