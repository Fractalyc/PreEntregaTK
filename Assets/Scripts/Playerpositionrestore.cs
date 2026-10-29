using UnityEngine;

public class PlayerPositionRestore : MonoBehaviour
{
    private void Awake()
    {
        if (!OverworldState.hasSaved) return;

        Vector2 pos = OverworldState.playerPosition;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        
        OverworldState.hasSaved = false;
    }
}
