using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace StarWarsArcade
{
class AIPath
{
    public Curve3D CurvePosition = new Curve3D();

    public Obstacle[] Obstacles = new Obstacle[60];

    // Specify the points the camera will pass through
    // and the points that the camera is looking at.
    // The number of points on the position and look-at curves
    // don't need to match, but the first and last points on the
    // curves should have the same time values if the curves oscillate.
    public void InitCurve()
    {
        float time = 0;
        float Y = 0;

        for (int i = 0; i < 60; i++)
        {
            switch ((int)Obstacles[i].World.Translation.Y)
            {
                case 0:
                    Y = 1.2f;
                    break;

                case 1:
                    Y = 0.0f;
                    break;

                case 2:
                    Y = 2.8f;
                    break;
                    
                case 3:
                    Y = 1.5f;
                    break;
            }

            CurvePosition.AddPoint(new Vector3(Obstacles[i].World.Translation.X, Y, Obstacles[i].World.Translation.Z), time);
            time += 1600;
        }

        CurvePosition.SetTangents();

    }

    public class Curve3D
    {

        public Curve curveX = new Curve();
        public Curve curveY = new Curve();
        public Curve curveZ = new Curve();


        public Curve3D()
        {
            //curveX.PostLoop = CurveLoopType.Oscillate;
            //curveY.PostLoop = CurveLoopType.Oscillate;
            //curveZ.PostLoop = CurveLoopType.Oscillate;

            //curveX.PreLoop = CurveLoopType.Oscillate;
            //curveY.PreLoop = CurveLoopType.Oscillate;
            //curveZ.PreLoop = CurveLoopType.Oscillate;
        }

        public void SetTangents()
        {
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;
            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == curveX.Keys.Count) nextIndex = i;

                prev = curveX.Keys[prevIndex];
                next = curveX.Keys[nextIndex];
                current = curveX.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveX.Keys[i] = current;
                prev = curveY.Keys[prevIndex];
                next = curveY.Keys[nextIndex];
                current = curveY.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveY.Keys[i] = current;

                prev = curveZ.Keys[prevIndex];
                next = curveZ.Keys[nextIndex];
                current = curveZ.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveZ.Keys[i] = current;
            }
        }

        static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur, ref CurveKey next)
        {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;
            if (Math.Abs(dv) < float.Epsilon)
            {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else
            {
                // The in and out tangents should be equal to the slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }

        public void AddPoint(Vector3 point, float time)
        {
            curveX.Keys.Add(new CurveKey(time, point.X));
            curveY.Keys.Add(new CurveKey(time, point.Y));
            curveZ.Keys.Add(new CurveKey(time, point.Z));
        }

        public Vector3 GetPointOnCurve(float time)
        {
            Vector3 point = new Vector3();
            point.X = curveX.Evaluate(time);
            point.Y = curveY.Evaluate(time);
            point.Z = curveZ.Evaluate(time);
            return point;
        }

    }
}
}