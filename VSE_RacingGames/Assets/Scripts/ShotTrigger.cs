using UnityEditor;
using UnityEngine;

public class ShotTrigger : MonoBehaviour
{
    private Color triggerColor = new Color(r: 0, g: 0.7f, b: 0, a: 0.5f);
    private Vector3 triggerShape = new Vector3(x: 7, y: 3, z: 0.1f);

    public ShotManager ShotManager
    {
        get
        {
            if (shotManager == null)
                shotManager = this.transform.parent.GetComponent<ShotManager>();
            return shotManager;
        }
    }
    private ShotManager shotManager;

    private void OnDrawGizmos()
    {
        if (ShotManager == null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Gizmos.color = triggerColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, triggerShape);

            Handles.Label(transform.position, name, VSEUtility.TriggerLabelStyle);
        }
    }
}
