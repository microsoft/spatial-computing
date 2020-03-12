using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.WSA;

[RequireComponent(typeof(ImageAnalyzer))]
[RequireComponent(typeof(ImageCapture))]
[RequireComponent(typeof(AudioSource))]
public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [SerializeField]
    private GameObject cursor;

    [SerializeField]
    private GameObject labelPrefab;

    [SerializeField]
    private GameObject labelLinePrefab;

    [SerializeField]
    private AudioClip tapClip;

    [SerializeField]
    private AudioClip doneClip;

    [SerializeField]
    private Color32 goodColor = new Color32(0, 102, 51, 255);

    [SerializeField]
    private Color32 errorColor = new Color32(247, 94, 52, 255);

    [SerializeField]
    private float confidenceThreshold = 0.8f;

    private AudioSource audioSource;
    private Dictionary<string, GameObject> visualizationObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void ClearScene()
    {
        foreach (var kvp in visualizationObjects)
        {
            Destroy(kvp.Value);
        }
        visualizationObjects.Clear();
    }

    public void SetCursorColor(Color color)
    {
        if(cursor != null)
        {
            try
            {
                cursor.GetComponent<Renderer>().sharedMaterial.color = color;
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
            
    }

    public void PlayTapSound()
    {
        if(audioSource != null && tapClip != null)
        {
            audioSource.PlayOneShot(tapClip);
        }
    }

    public void PlayDoneSound()
    {
        if (audioSource != null && doneClip != null)
        {
            audioSource.PlayOneShot(doneClip);
        }
    }

    public void HandleResponse(PredictionResults results)
    {
        if(results != null)
        {
            Debug.LogFormat("Results id: {0}", results.Id);

            if(results.Predictions != null)
            {
                CreateVisualizations(results.Predictions.ToArray());
            }
            else
            {
                Debug.Log("Precitions list is null");
            }
        }
        else
        {
            Debug.Log("results object is null");
        }
    }

    public void CreateVisualizations(Prediction[] predictions)
    {
        if (predictions != null && predictions.Length > 0)
        {
            foreach (var prediction in predictions)
            {
                if (prediction.Probability >= confidenceThreshold)
                {
                    if (visualizationObjects.ContainsKey(prediction.TagName))
                    {
                        UpdateVisualization(prediction);
                    }
                    else
                    {
                        CreateVisualization(prediction);
                    }
                }
                else
                {
                    RemoveVisulization(prediction);
                }
            }
        }

        PlayDoneSound();
        SetCursorColor(Color.green);

        ImageCapture.Instance.ResetImageCapture();
    }

    private void RemoveVisulization(Prediction prediction)
    {
        if (visualizationObjects.ContainsKey(prediction.TagName))
        {
            var parentObject = visualizationObjects[prediction.TagName];
            Destroy(parentObject);
            visualizationObjects.Remove(prediction.TagName);
        }
    }

    private void CreateVisualization(Prediction prediction)
    {
        var offset = new Vector3(0f, 0.1f, 0f);

        var parentObject = new GameObject();
        var label = Instantiate(labelPrefab);
        label.transform.parent = parentObject.transform;
        label.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        var labelLine = Instantiate(labelLinePrefab);
        labelLine.transform.parent = parentObject.transform;

        visualizationObjects[prediction.TagName] = parentObject;

        var labelText = label.GetComponentInChildren<TextMesh>();
        labelText.text = prediction.TagName;

        var lookAt = label.GetComponent<LookAtCamera>();
        lookAt.objectToLookAt = Camera.main.gameObject;

        if(prediction.TagName.Contains("OK"))
        {
            labelText.color = goodColor;
            labelLine.GetComponent<MeshRenderer>().material.SetColor("_Color", goodColor);
        }
        else
        {
            labelText.color = errorColor;
            labelLine.GetComponent<MeshRenderer>().material.SetColor("_Color", errorColor);
        }

        var cameraToWorldMatrix = ImageCapture.Instance.worldMat;
        var projectionMatrix = ImageCapture.Instance.projectionMat;

        var imageCenter = prediction.BoundingBox.Center;
        var center = TargetToWorld(projectionMatrix, cameraToWorldMatrix, imageCenter);

        if(center.HasValue)
        {
            parentObject.transform.position = center.Value;
            labelLine.transform.localPosition = new Vector3(0, 0.08f, 0);
            label.transform.localPosition = new Vector3(0, 0.18f, 0);
        }

        SetWorldAnchor(parentObject);
    }

    private void UpdateVisualization(Prediction prediction)
    {
        if (!visualizationObjects.ContainsKey(prediction.TagName))
        {
            throw new InvalidOperationException("Item is not currently tracked in the visualization collection. Please call CreateVisualization before calling this method.");
        }

        var parentObject = visualizationObjects[prediction.TagName];
        var visulization = parentObject.transform.GetChild(0).gameObject;
        var labelLine = parentObject.transform.GetChild(0).gameObject;
        UnsetWorldAnchor(parentObject);

        var labelText = visulization.GetComponentInChildren<TextMesh>();
        labelText.text = prediction.TagName;

        if (prediction.TagName.Contains("OK"))
        {
            labelText.color = goodColor;
            labelLine.GetComponent<MeshRenderer>().material.SetColor("_Color", goodColor);
        }
        else
        {
            labelText.color = errorColor;
            labelLine.GetComponent<MeshRenderer>().material.SetColor("_Color", errorColor);
        }

        var cameraToWorldMatrix = ImageCapture.Instance.worldMat;
        var projectionMatrix = ImageCapture.Instance.projectionMat;

        var imageCenter = prediction.BoundingBox.Center;
        var center = TargetToWorld(projectionMatrix, cameraToWorldMatrix, imageCenter);

        if (center.HasValue)
        {
            var parentLocation = center.Value;
            parentObject.transform.position = parentLocation;
        }

        SetWorldAnchor(parentObject);
    }

    private void SetWorldAnchor(GameObject gameObject)
    {
        gameObject.AddComponent<WorldAnchor>();
    }

    private void UnsetWorldAnchor(GameObject gameObject)
    {
        Destroy(gameObject.GetComponent<WorldAnchor>());
    }

    private Vector3? TargetToWorld(Matrix4x4 project, Matrix4x4 world, Vector2 target)
    {
        var cameraSpacePos = UnProjectVector(project, new Vector3(target.x, target.y, 1));
        var worldSpaceRayPoint1 = world.MultiplyPoint(Vector3.zero);     // camera location in world space
        var worldSpaceRayPoint2 = world.MultiplyPoint(cameraSpacePos);

        Debug.Log(string.Format("rayPoint1: {0}, rayPoint2: {1}", worldSpaceRayPoint1, worldSpaceRayPoint2));

        RaycastHit info;
        if (Physics.Raycast(worldSpaceRayPoint1, worldSpaceRayPoint2 - worldSpaceRayPoint1, out info, 30f, SpatialMapping.PhysicsRaycastMask))
        {
            Debug.Log("hit point: " + info.point);
            return info.point;
        }
        else
        {
            return null;
        }
    }

    private static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
    {
        Vector3 from = new Vector3(0, 0, 0);
        var axsX = proj.GetRow(0);
        var axsY = proj.GetRow(1);
        var axsZ = proj.GetRow(2);
        from.z = to.z / axsZ.z;
        from.y = (to.y - (from.z * axsY.z)) / axsY.y;
        from.x = (to.x - (from.z * axsX.z)) / axsX.x;
        return from;
    }
}
