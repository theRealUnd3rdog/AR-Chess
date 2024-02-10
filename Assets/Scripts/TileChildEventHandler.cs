using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileChildEventHandler : MonoBehaviour
{
    // Event invoked when a child is added to the tile
    public UnityEvent onChildAdded;

    // Event invoked when a child is removed from the tile
    public UnityEvent onChildRemoved;

    private void Start()
    {
        // Listen for changes in the number of child objects
        CheckChildCount();
    }

    private void OnTransformChildrenChanged()
    {
        // When the number of child objects changes, check the child count
        CheckChildCount();
    }

    private void CheckChildCount()
    {
        // Get the current number of child objects
        int childCount = transform.childCount;

        // Check if child count has increased or decreased
        if (childCount > 0)
        {
            // Child added
            onChildAdded.Invoke();
        }
        else
        {
            // No children remaining
            onChildRemoved.Invoke();
        }
    }
}
