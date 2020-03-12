using UnityEngine;

public class GazeCursor : MonoBehaviour
{
    /// <summary>
    /// The cursor (this object) mesh renderer
    /// </summary>
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Grab the mesh renderer that is on the same object as this script.
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        gameObject.GetComponent<Renderer>().material.color = Color.green;

        // If you wish to change the size of the cursor you can do so here
        gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Do a raycast into the world based on the user's head position and orientation.
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;

        RaycastHit gazeHitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out gazeHitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask))
        {
            // If the raycast hit a hologram, display the cursor mesh.
            meshRenderer.enabled = true;
            // Move the cursor to the point where the raycast hit.
            transform.position = gazeHitInfo.point;
            // Rotate the cursor to hug the surface of the hologram.
            transform.rotation = Quaternion.FromToRotation(Vector3.up, gazeHitInfo.normal);
        }
        else
        {
            // If the raycast did not hit a hologram, hide the cursor mesh.
            meshRenderer.enabled = false;
        }
    }
}
