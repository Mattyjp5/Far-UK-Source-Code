using UnityEngine;
using System.Collections;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public LayerMask groundLayer; // Layer mask to specify the ground layer
    public bool lockedToGround;
    public GameObject modelBase; // Reference to the base of the model
    private bool beingDragged;
    public float minimumYPosition = 0f; // Configurable minimum Y position, defaults to 0

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        beingDragged = true;
        
        // Get desired position from mouse
        Vector3 desiredPosition = GetMouseAsWorldPoint() + mOffset;
        
        // CRITICAL: Enforce minimum Y position - this line ensures we NEVER go below Y=0
        desiredPosition.y = Mathf.Max(desiredPosition.y, minimumYPosition);
        
        // Try to get ground height if ground layer is set
        if (groundLayer != 0) // 0 = nothing selected
        {
            float groundHeight = GetGroundYPosition(desiredPosition);
            // Only apply ground height if we found something
            if (groundHeight > -9000f) // arbitrary large negative number
            {
                desiredPosition.y = Mathf.Max(desiredPosition.y, groundHeight);
            }
        }
        
        // Apply final position
        transform.position = desiredPosition;

        // Lock the model base to the ground if applicable
        UpdateModelBasePosition();
    }
    
    // Extracted method to handle model base positioning
    private void UpdateModelBasePosition()
    {
        if (lockedToGround && modelBase != null)
        {
            // Find the ground using the layer
            RaycastHit hit;
            Vector3 rayStart = new Vector3(modelBase.transform.position.x, modelBase.transform.position.y + 100f, modelBase.transform.position.z);
            
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 200f, groundLayer))
            {
                // Move base to match ground height, but never below minimumYPosition
                float yPos = Mathf.Max(hit.point.y, minimumYPosition);
                modelBase.transform.position = new Vector3(
                    modelBase.transform.position.x, 
                    yPos,
                    modelBase.transform.position.z
                );
            }
            else
            {
                // If no ground found, ensure base is at minimum Y
                Vector3 basePos = modelBase.transform.position;
                if (basePos.y < minimumYPosition)
                {
                    modelBase.transform.position = new Vector3(basePos.x, minimumYPosition, basePos.z);
                }
            }
        }
    }

    // Method to get the ground level based on the ground layer
    float GetGroundYPosition(Vector3 currentPosition)
    {
        // Use a very large ray to ensure we hit ground
        Vector3 rayStart = new Vector3(currentPosition.x, currentPosition.y + 100f, currentPosition.z);
        RaycastHit hit;
        
        // Visualize the ray in scene view for debugging
        Debug.DrawRay(rayStart, Vector3.down * 200f, Color.red, 0.1f);
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 200f, groundLayer))
        {
            return hit.point.y;
        }
        
        // Return a value that will be ignored
        return -10000f;
    }

    void LateUpdate()
    {
        // Final failsafe - ensure position never goes below minimum Y
        if (transform.position.y < minimumYPosition)
        {
            Vector3 fixedPosition = transform.position;
            fixedPosition.y = minimumYPosition;
            transform.position = fixedPosition;
        }
        
        // Handle rotation while dragging
        if (beingDragged)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObject(-45f);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(45f);
            }
        }
    }

    private void RotateObject(float angle)
    {
        transform.Rotate(0, angle, 0);
    }

    void OnMouseUp()
    {
        // Apply position constraints one final time when releasing
        Vector3 safePosition = transform.position;
        safePosition.y = Mathf.Max(safePosition.y, minimumYPosition);
        transform.position = safePosition;
        
        // If we have a rigidbody, set its position too and zero out velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = safePosition;
        }
        
        beingDragged = false;
        
        // Start continuous position checking for a short time after release
        StartCoroutine(EnforceMinYPositionForDuration(2.0f));
    }
    
    // Coroutine to ensure object stays above minimum Y for a duration after releasing
    private System.Collections.IEnumerator EnforceMinYPositionForDuration(float duration)
    {
        float endTime = Time.time + duration;
        
        while (Time.time < endTime)
        {
            // Check every frame for the specified duration
            if (transform.position.y < minimumYPosition)
            {
                Vector3 safePosition = transform.position;
                safePosition.y = minimumYPosition;
                transform.position = safePosition;
                
                // Also apply to rigidbody if present
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.position = safePosition;
                }
            }
            
            yield return null; // Wait for next frame
        }
    }
}