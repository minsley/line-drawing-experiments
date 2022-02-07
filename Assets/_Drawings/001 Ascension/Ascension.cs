using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Freya;
using Shapes;
using UnityEngine;

[ExecuteAlways]
public class Ascension : ImmediateModeShapeDrawer
{
    public float lineLength = 0.5f;
    public float skew = 2;
    public float duration = 30;
    public float floatHeight = 10;
    public float spacing = 1.5f;
    public Vector3Int field = new Vector3Int(6, 8, 6);

    public Vector2[] opacityMap =
    {
        new(0f, 0),
        new(0.1f, 1),
        new(0.7f, 1),
        new(1f, 0)
    };

    public Vector2[] bezier =
    {
        Vector2.zero,
        new (0.44f, -1.41f),
        new (0f, 0.46f),
        new (0.5f, 0.5f)
    };

    private readonly List<Box> _boxes = new List<Box>();
    private BezierCubic2D _horizOffsetCurve;

    public override void OnEnable()
    {
        base.OnEnable();
        Init();
    }

    private void OnValidate()
    {
        Init();
    }

    // private void Update()
    // {
    //     throw new NotImplementedException();
    // }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.LineGeometry = LineGeometry.Billboard;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = 0.005f;
            Draw.Matrix = transform.localToWorldMatrix;
            Draw.Color = Color.black;

            for (int layer = 0; layer < field.y; layer++)
            {
                foreach (var box in _boxes)
                {
                    DrawBoxAtLayer(box, layer);
                }
            }
        }
    }
    
    private void DrawBoxAtLayer(Box box, int layer)
    {
        var t = (Time.time - box.timeOffset - duration*layer/field.y) % duration / duration;

        if (t <= 0) return;
        
        Draw.Opacity = MultiLerp(t, opacityMap);
        Draw.Line(box.GetCorner(0, t), box.GetCorner(1, t));
        Draw.Line(box.GetCorner(1, t), box.GetCorner(2, t));
        Draw.Line(box.GetCorner(2, t), box.GetCorner(3, t));
        Draw.Line(box.GetCorner(3, t), box.GetCorner(0, t));
        Draw.Line(box.GetCorner(4, t), box.GetCorner(5, t));
        Draw.Line(box.GetCorner(5, t), box.GetCorner(6, t));
        Draw.Line(box.GetCorner(6, t), box.GetCorner(7, t));
        Draw.Line(box.GetCorner(7, t), box.GetCorner(4, t));
        Draw.Line(box.GetCorner(0, t), box.GetCorner(4, t));
        Draw.Line(box.GetCorner(1, t), box.GetCorner(5, t));
        Draw.Line(box.GetCorner(2, t), box.GetCorner(6, t));
        Draw.Line(box.GetCorner(3, t), box.GetCorner(7, t));
    }

    private void Init()
    {
        _horizOffsetCurve = new(bezier[0], bezier[1], bezier[2], bezier[3]);
        
        _boxes.Clear();
        
        for (int i = 0; i < field.x; i++)
        {
            for (int j = 0; j < field.z; j++)
            {
                _boxes.Add(new Box(
                    lineLength, 
                    duration, 
                    floatHeight,
                    skew,
                    Time.time, 
                    new Vector3(i - (field.x-1)/2f, 0, j - (field.z-1)/2f) * spacing,
                    _horizOffsetCurve));
            }
        }
    }

    private float MultiLerp(float t, params Vector2[] breakpoints)
    {
        switch (breakpoints.Length)
        {
            case 0: return t;
            case 1: return breakpoints[0].y;
            default:
                int next = 0;
                for (int i = 0; i < breakpoints.Length; i++)
                {
                    if (breakpoints[i].x <= t) continue;

                    next = i;
                    break;
                }

                if (next == 0) return breakpoints[next].y;

                var a = breakpoints[next - 1];
                var b = breakpoints[next];
                
                return Mathf.Lerp(a.y, b.y, Mathf.InverseLerp(a.x, b.x, t));
        }
    }

    struct Box
    {
        public float length;
        public Vector3[] cornerStarts;
        public Vector3[] cornerEnds;
        public float duration;
        public float floatHeight;
        public float skew;
        public float timeOffset;
        public Vector3 posOffset;
        public BezierCubic2D horizOffsetCurve;

        public Box(float length, float duration, float floatHeight, float skew, float timeOffset, Vector3 posOffset, BezierCubic2D horizOffsetCurve)
        {
            this.length = length;
            this.duration = duration;
            this.floatHeight = floatHeight;
            this.skew = skew;
            this.timeOffset = timeOffset;
            this.posOffset = posOffset;
            this.horizOffsetCurve = horizOffsetCurve;

            var halfLen = length / 2;
            cornerStarts = new[]
            {
                new Vector3(-halfLen, 0, -halfLen),
                new Vector3(-halfLen, 0, halfLen),
                new Vector3(halfLen, 0, halfLen),
                new Vector3(halfLen, 0, -halfLen),
                new Vector3(-halfLen, length, -halfLen),
                new Vector3(-halfLen, length, halfLen),
                new Vector3(halfLen, length, halfLen),
                new Vector3(halfLen, length, -halfLen),
            };

            cornerEnds = cornerStarts.ToArray();
            for(var i=0; i<cornerEnds.Length; i++)
            {
                cornerEnds[i] = UnityEngine.Random.insideUnitSphere * length * skew;
            }
        }

        public Vector3 GetCorner(int i, float t)
        {
            var start = cornerStarts[i];
            var end = cornerEnds[i];
            var heightOffset = new Vector3(0, floatHeight, 0);
            var offset = horizOffsetCurve.GetPointY(t);
            var horizOffset = new Vector3(start.x.Sign() * offset, 0, start.z.Sign() * offset);

            start += posOffset;
            end += posOffset + heightOffset + horizOffset;
            
            return Vector3.Lerp(start, end, t);
        }
    }
}
