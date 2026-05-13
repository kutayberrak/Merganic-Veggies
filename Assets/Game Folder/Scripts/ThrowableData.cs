using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableData", menuName = "Game/Throwable Data")]
public class ThrowableData : ScriptableObject
{
    [Header("Throwable Info")]
    public ThrowableCategory category;

    [Min(1)]
    public int tier = 1;

    [Min(0)]
    public int score = 1;
}
