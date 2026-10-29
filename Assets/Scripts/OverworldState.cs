using UnityEngine;

public static class OverworldState
{
    public static bool hasSaved = false;
    public static Vector2 playerPosition;

    public static bool returningFromRun = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        hasSaved = false;
        returningFromRun = false;
    }
}
