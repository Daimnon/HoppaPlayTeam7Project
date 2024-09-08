using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset; 
    [SerializeField] private float zoomFactor = 2f;

    private float initialPlayerScale;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned!");
            return;
        }

        offset = transform.position - player.position;
        initialPlayerScale = player.localScale.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        float currentScale = player.localScale.x;
        float scaleRatio = currentScale / initialPlayerScale;

        Vector3 adjustedOffset = offset * scaleRatio * zoomFactor;

        transform.position = player.position + adjustedOffset;

        transform.LookAt(player);
    }
}
