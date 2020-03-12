using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[JsonObject]
public class PredictionResults
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "project")]
    public Guid Project { get; set; }

    [JsonProperty(PropertyName = "iteration")]
    public Guid Iteration { get; set; }

    [JsonProperty(PropertyName = "created")]
    public DateTime Created { get; set; }

    [JsonProperty(PropertyName = "predictions")]
    public List<Prediction> Predictions { get; set; }
}

[JsonObject]
public class Prediction
{
    [JsonProperty(PropertyName = "tagId")]
    public string TagId { get; set; }

    [JsonProperty(PropertyName = "tagName")]
    public string TagName { get; set; }

    [JsonProperty(PropertyName = "probability")]
    public float Probability { get; set; }

    [JsonProperty(PropertyName = "boundingBox")]
    public BoundingBox BoundingBox { get; set; }
}
