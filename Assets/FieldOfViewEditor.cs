using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc (fow.transform.position, Vector3.forward, Vector3.right, 360, fow.m_viewRadius);
        Vector2 viewAngleA = fow.dirFromAngle(-fow.m_viewAngle / 2, false);
        Vector2 viewAngleB = fow.dirFromAngle(fow.m_viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, (Vector2)fow.transform.position + viewAngleA * fow.m_viewRadius);
        Handles.DrawLine(fow.transform.position, (Vector2)fow.transform.position + viewAngleB * fow.m_viewRadius);
    }
}
