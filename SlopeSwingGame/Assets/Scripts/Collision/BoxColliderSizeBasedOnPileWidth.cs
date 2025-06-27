using NUnit.Framework;
using UnityEngine;

public class BoxColliderSizeBasedOnPileWidth : MonoBehaviour
{
    private PileManager pile;
    private BoxCollider boxCollider;
    private Vector3 desiredSize;
    [SerializeField] private float xMinimum;
    [SerializeField] private float yMinimum;
    [SerializeField] private float zMinimum;

    private void FixedUpdate()
    {
        desiredSize.x = pile.Width;
        desiredSize.z = pile.Height;

        if (pile.Width < xMinimum)
        {
            desiredSize.x = xMinimum;
        }
        if (pile.Height < zMinimum)
        {
            desiredSize.z = zMinimum;
        }

        boxCollider.size = desiredSize;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pile = GetComponent<PileManager>();
        boxCollider = GetComponent<BoxCollider>();
        desiredSize = new Vector3(0, boxCollider.size.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
