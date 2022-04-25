using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugExtensions
{
    public static void DrawCircle(Vector3 position, float radius, Color debugColor, float maxAngle = 360, int steps = 20) {
        float angle = 0;
        Vector3 begin = position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized * radius;
        Vector3 end;
        for (int j = 0; j <= steps; j++) {
            angle = (float)j / (float)steps * maxAngle * Mathf.Deg2Rad;
            end = position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized * radius;
            Debug.DrawLine(begin, end, debugColor);
            begin = end;
        }
    }
}
