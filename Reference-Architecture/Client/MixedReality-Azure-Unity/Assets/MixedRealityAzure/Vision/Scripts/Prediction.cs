using UnityEngine;

//Class to store each prediction of a PredictionResult in a nice format
public class Prediction
{
    public string name { get; set; }
    public float confidence { get; set; }
    public float[] boundingBoxInfo = null;// holds info about the bounding box, in fractions of the image (Left, Top, Width, Height)
}
