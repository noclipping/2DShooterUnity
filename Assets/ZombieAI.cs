using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public Transform player;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float moveSpeed = 2f;
    public LayerMask obstacleMask; // Set in the inspector to detect obstacles

    private List<Vector2> path = new List<Vector2>();
    private int currentWaypoint = 0;
    private float nextWaypointDistance = 0.1f;
    private bool isCalculatingPath = false;
    private float pathRecalculateThreshold = 2.0f; // How far the player needs to move to trigger a recalculation
    private Vector3 lastPlayerPosition;
    private bool hasReachedEndOfPath = false;
    private float recalculateCooldown = 0.5f; // Cooldown between forced recalculations
    private float timeSinceLastRecalculation = 0f;

    // For tracking how long the zombie stays on a waypoint
    private float timeOnWaypoint = 0f;
    private float maxTimeOnWaypoint = 2.0f; // Max time allowed on a waypoint before recalculating

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
        lastPlayerPosition = player.position; // Initial player position
    }

    void Update()
    {
        timeSinceLastRecalculation += Time.deltaTime;

        // If we reached the end of the path, force a recalculation after cooldown
        if (hasReachedEndOfPath && timeSinceLastRecalculation >= recalculateCooldown)
        {
            Debug.Log("Recalculating path after reaching the end.");
            StartCoroutine(CalculatePath(transform.position, player.position));
            hasReachedEndOfPath = false; // Reset the flag
            timeSinceLastRecalculation = 0f; // Reset the cooldown
        }

        // Recalculate if the player has moved significantly
        else if (!isCalculatingPath && Vector3.Distance(player.position, lastPlayerPosition) > pathRecalculateThreshold)
        {
            Debug.Log("Recalculating path due to player movement.");
            StartCoroutine(CalculatePath(transform.position, player.position));
            lastPlayerPosition = player.position; // Update the player's last known position
        }

        // Move along the path
        if (path.Count > 0 && currentWaypoint < path.Count)
        {
            MoveTowardsWaypoint();
            RotateTowardsPlayer();  // Rotate the zombie to face the player while moving
        }
        else if (currentWaypoint >= path.Count)
        {
            // If we reach the last waypoint, set flag to trigger path recalculation
            Debug.Log("Reached the end of the path. Waiting for recalculation.");
            hasReachedEndOfPath = true; // Mark that we've reached the end
        }

        // Check if the zombie is close enough to attack the player
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
        }
    }

    void MoveTowardsWaypoint()
    {
        if (currentWaypoint >= path.Count)
        {
            return; // No more waypoints
        }

        Vector2 targetPosition = path[currentWaypoint];
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 movement = direction * moveSpeed * Time.deltaTime;

        // Check if there's an obstacle between the zombie and the next waypoint
        if (Physics2D.Raycast(transform.position, direction, Vector2.Distance(transform.position, targetPosition), obstacleMask))
        {
            Debug.Log("Obstacle detected! Recalculating path...");
            StartCoroutine(CalculatePath(transform.position, player.position));
            return;
        }

        // Move the zombie
        transform.position += (Vector3)movement;

        // Check if the waypoint is reached
        if (Vector2.Distance(transform.position, targetPosition) < nextWaypointDistance)
        {
            currentWaypoint++;
            Debug.Log("Reached waypoint " + currentWaypoint);
            timeOnWaypoint = 0f; // Reset time spent on the waypoint when moving to the next one
        }
        else
        {
            // Track time spent on the current waypoint
            timeOnWaypoint += Time.deltaTime;
            if (timeOnWaypoint > maxTimeOnWaypoint)
            {
                Debug.Log("Stuck on waypoint for too long. Recalculating path...");
                StartCoroutine(CalculatePath(transform.position, player.position));
                timeOnWaypoint = 0f; // Reset the timer after triggering recalculation
            }
        }
    }

    // Rotate the zombie to face the player
    // Rotate the zombie to face the player
    void RotateTowardsPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle to account for the sprite's default orientation
        float rotationOffset = -90f; // Adjust this value based on the orientation of your sprite
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + rotationOffset)); // Rotate the zombie on the Z-axis
    }

    void AttackPlayer()
    {
        PlayerStats playerStats = player.GetComponent<PlayerController>().playerStats;
        if (playerStats != null)
        {
            playerStats.hp -= attackDamage;
            Debug.Log($"Zombie attacked! Player HP: {playerStats.hp}");
        }
    }

    // Coroutine to calculate the A* path over multiple frames
    IEnumerator CalculatePath(Vector2 startPosition, Vector2 targetPosition)
    {
        isCalculatingPath = true;
        List<Node> openSet = new List<Node>();
        HashSet<Vector2> closedSet = new HashSet<Vector2>();

        Node startNode = new Node(startPosition, null, 0, GetHeuristic(startPosition, targetPosition));
        Node targetNode = new Node(targetPosition, null, 0, 0);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Find node with the lowest F cost
            Node currentNode = openSet[0];
            foreach (Node node in openSet)
            {
                if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                {
                    currentNode = node;
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            // Check if we reached the target
            if (Vector2.Distance(currentNode.Position, targetNode.Position) < 0.5f)
            {
                path = RetracePath(startNode, currentNode);
                currentWaypoint = 0;
                isCalculatingPath = false;
                Debug.Log("Path found to player.");
                yield break; // Path found, stop the coroutine
            }

            // Explore neighbors
            foreach (Vector2 neighborPosition in GetNeighbors(currentNode.Position))
            {
                if (closedSet.Contains(neighborPosition)) continue;

                // Use a raycast to detect obstacles
                if (Physics2D.Raycast(currentNode.Position, (neighborPosition - currentNode.Position).normalized, Vector2.Distance(currentNode.Position, neighborPosition), obstacleMask))
                {
                    continue; // Skip if an obstacle is detected
                }

                float newMovementCost = currentNode.G + Vector2.Distance(currentNode.Position, neighborPosition);
                Node neighborNode = new Node(neighborPosition, currentNode, newMovementCost, GetHeuristic(neighborPosition, targetNode.Position));

                if (!openSet.Contains(neighborNode) || newMovementCost < neighborNode.G)
                {
                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Add(neighborNode);
                    }
                }
            }

            // Yield to spread the pathfinding over multiple frames
            yield return null;
        }

        isCalculatingPath = false; // No path found
        Debug.Log("No path found.");
    }

    // Retrace the path from the target node back to the start node
    List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    // Calculate neighbors (4-directional movement for simplicity)
    List<Vector2> GetNeighbors(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>();
        float stepSize = 1f; // Increase step size for larger moves

        // Add the 4 cardinal directions (N, S, E, W)
        neighbors.Add(position + Vector2.up * stepSize);    // North
        neighbors.Add(position + Vector2.down * stepSize);  // South
        neighbors.Add(position + Vector2.left * stepSize);  // West
        neighbors.Add(position + Vector2.right * stepSize); // East

        return neighbors;
    }

    // Heuristic (distance between current node and target)
    float GetHeuristic(Vector2 positionA, Vector2 positionB)
    {
        return Vector2.Distance(positionA, positionB); // Euclidean distance
    }
}

// Node class used in A* pathfinding
public class Node
{
    public Vector2 Position;
    public Node Parent;
    public float G; // Cost from the start node
    public float H; // Heuristic cost to the target node

    public float F { get { return G + H; } } // Total cost

    public Node(Vector2 position, Node parent, float g, float h)
    {
        this.Position = position;
        this.Parent = parent;
        this.G = g;
        this.H = h;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        Node node = (Node)obj;
        return Position.Equals(node.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
