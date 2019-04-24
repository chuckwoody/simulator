﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    [HideInInspector]
    public Color laneColor = Color.cyan;
    [HideInInspector]
    public Color whiteLineColor = Color.white;
    [HideInInspector]
    public Color yellowLineColor = Color.yellow;
    [HideInInspector]
    public Color stopLineColor = Color.red;
    [HideInInspector]
    public Color stopSignColor = Color.red;
    [HideInInspector]
    public Color junctionColor = Color.gray;
    [HideInInspector]
    public Color poleColor = Color.white;
    [HideInInspector]
    public Color speedBumpColor = Color.yellow;
    [HideInInspector]
    public Color curbColor = Color.blue;
    [HideInInspector]
    public Color pedestrianColor = Color.green;


    public enum LaneTurnType
    {
        NO_TURN = 1,
        LEFT_TURN = 2,
        RIGHT_TURN = 3,
        U_TURN = 4
    };

    public enum LaneBoundaryType
    {
        UNKNOWN = 0,
        DOTTED_YELLOW = 1,
        DOTTED_WHITE = 2,
        SOLID_YELLOW = 3,
        SOLID_WHITE = 4,
        DOUBLE_YELLOW = 5,
        CURB = 6
    };

    public enum LineType
    {
        UNKNOWN = -1, // TODO why is this -1 not 0?
        SOLID_WHITE = 0,
        SOLID_YELLOW = 1,
        DOTTED_WHITE = 2,
        DOTTED_YELLOW = 3,
        DOUBLE_WHITE = 4,
        DOUBLE_YELLOW = 5,
        CURB = 6,
        STOP = 7
    };

    public enum SignType
    {
        STOP = 0,
        YIELD = 1,
        // TODO all the signs!
    }
    
    public static class AnnotationGizmos
    {
        public static void DrawArrowHead(Vector3 start, Vector3 end, Color color, float arrowHeadScale = 1.0f, float arrowHeadLength = 0.02f, float arrowHeadAngle = 13.0f, float arrowPositionRatio = 0.5f)
        {
            var originColor = Gizmos.color;
            Gizmos.color = color;

            var lineVector = end - start;
            var arrowFwdVec = lineVector.normalized * arrowPositionRatio * lineVector.magnitude;

            //Draw arrow head
            Vector3 right = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
            Vector3 left = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
            Vector3 up = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
            Vector3 down = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;

            Vector3 arrowTip = start + (arrowFwdVec);

            Gizmos.DrawLine(arrowTip, arrowTip + right * arrowHeadScale);
            Gizmos.DrawLine(arrowTip, arrowTip + left * arrowHeadScale);
            Gizmos.DrawLine(arrowTip, arrowTip + up * arrowHeadScale);
            Gizmos.DrawLine(arrowTip, arrowTip + down * arrowHeadScale);

            Gizmos.color = originColor;
        }

        public static void DrawWaypoint(Vector3 point, float pointRadius, Color surfaceColor, Color lineColor)
        {
            Gizmos.color = surfaceColor;
            Gizmos.DrawSphere(point, pointRadius);
            Gizmos.color = lineColor;
            Gizmos.DrawWireSphere(point, pointRadius);
        }

        public static void DrawArrowHeads(Transform mainTrans, List<Vector3> localPoints, Color lineColor)
        {
            for (int i = 0; i < localPoints.Count - 1; i++)
            {
                var start = mainTrans.TransformPoint(localPoints[i]);
                var end = mainTrans.TransformPoint(localPoints[i + 1]);
                DrawArrowHead(start, end, lineColor, arrowHeadScale: MapAnnotationTool.ARROWSIZE * 1f, arrowPositionRatio: 0.5f); // TODO why reference map annotation tool?
            }
        }

        public static void DrawLines(Transform mainTrans, List<Vector3> localPoints, Color lineColor)
        {
            var pointCount = localPoints.Count;
            for (int i = 0; i < pointCount - 1; i++)
            {
                var start = mainTrans.TransformPoint(localPoints[i]);
                var end = mainTrans.TransformPoint(localPoints[i + 1]);
                Gizmos.color = lineColor;
                Gizmos.DrawLine(start, end);
            }
        }

        public static void DrawWaypoints(Transform mainTrans, List<Vector3> localPoints, float pointRadius, Color surfaceColor, Color lineColor)
        {
            var pointCount = localPoints.Count;
            for (int i = 0; i < pointCount - 1; i++)
            {
                var start = mainTrans.TransformPoint(localPoints[i]);
                DrawWaypoint(start, pointRadius, surfaceColor, lineColor);
            }

            var last = mainTrans.TransformPoint(localPoints[pointCount - 1]);
            DrawWaypoint(last, pointRadius, surfaceColor, lineColor);
        }
    }

#if UNITY_EDITOR
    public class Handles
    {
        public void DrawArrow(Vector3 start, Vector3 end, Color color, float arrowHeadScale = 1.0f, float arrowHeadLength = 0.02f, float arrowHeadAngle = 20.0f, float arrowPositionRatio = 0.5f)
        {
            var originColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = color;

            //Draw base line
            UnityEditor.Handles.DrawLine(start, end);

            var lineVector = end - start;
            var arrowFwdVec = lineVector.normalized * arrowPositionRatio * lineVector.magnitude;

            //Draw arrow head
            Vector3 right = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
            Vector3 left = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
            Vector3 up = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
            Vector3 down = (Quaternion.LookRotation(arrowFwdVec) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;

            Vector3 arrowTip = start + (arrowFwdVec);

            UnityEditor.Handles.DrawLine(arrowTip, arrowTip + right * arrowHeadScale);
            UnityEditor.Handles.DrawLine(arrowTip, arrowTip + left * arrowHeadScale);
            UnityEditor.Handles.DrawLine(arrowTip, arrowTip + up * arrowHeadScale);
            UnityEditor.Handles.DrawLine(arrowTip, arrowTip + down * arrowHeadScale);

            UnityEditor.Handles.color = originColor;
        }
    }
#endif
}
