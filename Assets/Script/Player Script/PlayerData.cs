using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    // player transform
    // sprite
    // idle, walk
    [SerializeField] private Direction playerDirection;
    [SerializeField] private float playerSpeed = 4.5f;

    public Direction PlayerDirection { get { return playerDirection; } set { playerDirection = value; } }
    public float PlayerSpeed { get { return playerSpeed; } }
}

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right
}