using UnityEngine;

public class water : MonoBehaviour
{
    [Tooltip("Speed of the UV scroll. Positive or negative values allowed.")]
    public Vector2 scrollSpeed = new Vector2(0.5f, 0f);

    private Renderer rend;
    private Vector2 currentOffset = Vector2.zero;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        currentOffset += scrollSpeed * Time.deltaTime;
        rend.material.mainTextureOffset = currentOffset;
    }
}
