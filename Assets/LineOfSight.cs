using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public LayerMask targetLayer; // Layer of the objects to check for line of sight
    public LayerMask obstructionLayer; // Layer of the walls/obstructions
    public float checkRadius; // Radius within which to check for objects

    private void Update()
    {
        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, checkRadius, targetLayer);

        foreach (var target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector2 directionToTarget = (targetTransform.position - transform.position).normalized;

            float distanceToTarget = Vector2.Distance(transform.position, targetTransform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer);

            bool isVisible = hit.collider == null;
            SetRendererState(target.gameObject, isVisible);
        }
    }

    private void SetRendererState(GameObject obj, bool state)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = state;
        }

        // Recursively set the state for all children
        foreach (Transform child in obj.transform)
        {
            SetRendererState(child.gameObject, state);
        }
    }
}
