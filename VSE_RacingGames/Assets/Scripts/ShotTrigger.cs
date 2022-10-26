using UnityEngine;

public class ShotTrigger : MonoBehaviour
{
    private int oldChildCount;

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
        if (ShotManager == null || ShotManager.ToggleShotTriggerDisplay == false)
        {
            foreach (Transform child in this.transform.parent)
                DestroyImmediate(child.gameObject);
        }
        else if (this.transform.parent.childCount > ShotManager.ListOfShots.Count)
        {
            oldChildCount = this.transform.parent.childCount;

            for (int i = ShotManager.ListOfShots.Count; i < oldChildCount; i++)
            {
                try
                {
                    DestroyImmediate(this.transform.parent.GetChild(i).gameObject);
                }
                catch
                {
                    break;
                }
            }
        }
        else
        {
            Gizmos.color = new Color(r: 0, g: 0.7f, b: 0, a: 0.5f);
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, new Vector3(x: 7, y: 3, z: 0.1f));
        }
    }
}
