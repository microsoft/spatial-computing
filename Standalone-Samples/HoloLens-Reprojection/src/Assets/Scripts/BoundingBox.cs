using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoundingBox
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Vector2 Center {
        get
        {
            float x = (float)(Left + (Width / 2.0f)) * 2.0f - 1.0f;
            float y = (float)(1.0f - (Top + Height / 2.0f)) * 2.0f - 1.0f;
            return new Vector2(x, y);
        }
    }
}
