using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

public class GraphManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject tickMarkPrefab; // Includes child: TickMesh + TextMeshPro
    [SerializeField] private GameObject axisLinePrefab; // LineRenderer or cube

    [Header("Graph Settings")]
    [SerializeField] private float baseSpacing = 1f;
    [SerializeField] private int ticksPerAxis = 10;
    [SerializeField] private Vector2 tickSize = new Vector2(0.1f, 0.25f);

    [Header("Label Settings")]
    [SerializeField] private float labelZoomScale = 0.2f;
    [SerializeField] private float labelOffsetMultiplier = 0.6f;
    [SerializeField] private bool showOriginLabel = true;
    [SerializeField] private int labelIntervalX = 1;
    [SerializeField] private int labelIntervalZ = 1;
    [SerializeField] private Color positiveLabelColor = Color.white;
    [SerializeField] private Color negativeLabelColor = Color.white;

    [Header("Customization Settings")]
    [SerializeField] private List<Material> axisLineMaterials = new();
    [SerializeField] private Material tickMaterial;
    [SerializeField] private float lineRendererWidth = 0.05f;

    private List<TickMark> xTicks = new();
    private List<TickMark> zTicks = new();
    private List<GameObject> allAxisLines = new();
    private Dictionary<Vector3, LineRenderer> allAxisLineRenderers = new();

    private float lastOrthoSize = -1f;
    private bool graphVisible = true;
    private bool graphOff = false;
    private Vector2Int labelOrigin = Vector2Int.zero;

    [Header("Sound")]
    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip buttonClick;

    private class TickMark
    {
        public GameObject root;
        public Transform tickMesh;
        public TextMeshPro label;
    }

    void Start()
    {
        mainCamera = Camera.main;
        CreateAxisLines();
        ForceRefresh();
    }

    private void CreateAxisLines()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject axis = Instantiate(axisLinePrefab, transform);
            allAxisLines.Add(axis);

            Vector3 direction = i switch
            {
                0 => Vector3.forward,
                1 => Vector3.right,
                2 => -Vector3.forward,
                3 => -Vector3.right,
                _ => Vector3.zero
            };

            LineRenderer lr = axis.GetComponent<LineRenderer>();
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, direction * 1000f);
            lr.startWidth = lr.endWidth = lineRendererWidth;

            lr.material = axisLineMaterials.Count switch
            {
                // Assign material based on material list count
                1 => axisLineMaterials[0],
                2 => direction == Vector3.right || direction == -Vector3.right
                    ? axisLineMaterials[0]
                    : axisLineMaterials[1],
                >= 4 => axisLineMaterials[i],
                _ => lr.material
            };

            allAxisLineRenderers[direction] = lr;
        }
    }

    private void UpdateGraph()
    {
        if (player == null || mainCamera == null) return;

        float orthoSize = mainCamera.orthographicSize;
        if (Mathf.Approximately(orthoSize, lastOrthoSize)) return;
        lastOrthoSize = orthoSize;

        transform.position = player.position;
        transform.rotation = Quaternion.identity;

        float zoomRatio = orthoSize / 5f;
        float spacing = baseSpacing * zoomRatio;

        UpdateTicks(xTicks, Vector3.right, Vector3.forward, spacing, ticksPerAxis, true);
        UpdateTicks(zTicks, Vector3.forward, Vector3.right, spacing, ticksPerAxis, false);

        UpdateAxisLines();
    }

    private void UpdateTicks(List<TickMark> ticks, Vector3 mainDir, Vector3 tickDir, float spacing, int count, bool isX)
    {
        int totalTicks = count * 2 + 1;

        while (ticks.Count < totalTicks)
        {
            GameObject tickRoot = Instantiate(tickMarkPrefab, transform);
            Transform tickMesh = tickRoot.transform.Find("TickMesh");
            TextMeshPro label = tickRoot.GetComponentInChildren<TextMeshPro>();

            // Apply material to tick mesh
            if (tickMaterial != null && tickMesh != null)
            {
                Renderer r = tickMesh.GetComponent<Renderer>();
                if (r != null) r.material = tickMaterial;
            }

            ticks.Add(new TickMark { root = tickRoot, tickMesh = tickMesh, label = label });
        }

        for (int i = 0; i < totalTicks; i++)
        {
            int offset = i - count;
            Vector3 pos = transform.position + mainDir * (offset * spacing);

            if (pos == transform.position)
            {
                ticks[i].root.SetActive(false);
                continue;
            }

            TickMark tick = ticks[i];
            tick.root.transform.position = pos;
            tick.root.transform.rotation = Quaternion.identity;

            if (tick.tickMesh != null)
            {
                tick.tickMesh.rotation = Quaternion.LookRotation(tickDir);
                tick.tickMesh.localScale = new Vector3(tickSize.x * spacing, tickSize.x * spacing, tickSize.y * spacing);
            }

            if (tick.label != null)
            {
                int value = offset + (isX ? labelOrigin.x : labelOrigin.y);
                int interval = isX ? labelIntervalX : labelIntervalZ;

                bool show = (value != 0 && interval > 0 && value % interval == 0) || (value == 0 && showOriginLabel);
                tick.label.gameObject.SetActive(show);

                if (show)
                {
                    Vector3 labelOffset = isX
                        ? -Vector3.forward * (spacing * labelOffsetMultiplier)
                        : Vector3.right * (spacing * labelOffsetMultiplier);

                    tick.label.transform.position = pos + labelOffset;
                    tick.label.transform.rotation = Quaternion.Euler(90, 0, 0);
                    tick.label.fontSize = spacing * labelZoomScale;
                    tick.label.text = value.ToString();

                    tick.label.color = value >= 0 ? positiveLabelColor : negativeLabelColor;
                }
            }
        }

        while (ticks.Count > totalTicks)
        {
            Destroy(ticks[^1].root);
            ticks.RemoveAt(ticks.Count - 1);
        }
    }

    private void UpdateAxisLines()
    {
        foreach (var kvp in allAxisLineRenderers)
        {
            kvp.Value.SetPosition(0, player.position);
            kvp.Value.SetPosition(1, player.position + kvp.Key * 1000f);
            kvp.Value.startWidth = kvp.Value.endWidth = lineRendererWidth;
        }
    }

    private void ForceRefresh()
    {
        lastOrthoSize = -1f;
        UpdateGraph();
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        ForceRefresh();
    }

    public void SetPlayerStill(bool still)
    {
        if (graphOff) return;
        graphVisible = still;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(graphVisible);

        if (still) ForceRefresh();
    }
    
    public void GraphOffButton()
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);
        graphOff = !graphOff;
        graphVisible = !graphOff;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(graphVisible);

        if (graphVisible) ForceRefresh();
    }

    public void ResetLabelOrigin()
    {
        labelOrigin = Vector2Int.zero;
        ForceRefresh();
    }

    public void SetLabelOrigin(Vector2Int origin)
    {
        labelOrigin = origin;
        ForceRefresh();
    }

    private IEnumerator FadeInTicks(List<TickMark> ticks)
    {
        float duration = 0.5f;
        float timer = 0f;

        while (timer < duration)
        {
            float alpha = timer / duration;

            foreach (var tick in ticks)
            {
                if (tick.label != null)
                {
                    var color = tick.label.color;
                    color.a = alpha;
                    tick.label.color = color;
                }

                var renderComponent = tick.tickMesh?.GetComponent<Renderer>();
                if (renderComponent != null && renderComponent.material.HasProperty("_Color"))
                {
                    Color c = renderComponent.material.color;
                    c.a = alpha;
                    renderComponent.material.color = c;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
