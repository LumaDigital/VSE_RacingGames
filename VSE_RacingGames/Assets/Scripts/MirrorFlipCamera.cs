using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class MirrorFlipCamera : MonoBehaviour
{
    public Camera GameCamera
    {
        get
        {
            if (gameCamera == null)
            {
                gameCamera = this.GetComponent<Camera>();
            }
            return gameCamera;
        }
    }
    private Camera gameCamera;

    public bool FlipHorizontal
    {
        get => flipHorizontal;
        set
        {
            flipHorizontal = value;
            UpdateCameraMatrix();
        }
    }
    [SerializeField]
    private bool flipHorizontal;

    private float aspectRatio = 0;

    void Awake()
    {
        RenderPipelineManager.beginCameraRendering += beginCameraRendering;
        RenderPipelineManager.endCameraRendering += endCameraRendering;
    }

    void beginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        GL.invertCulling = flipHorizontal;

        // Update if aspect ratio changed
        if (Mathf.Abs(aspectRatio - GameCamera.aspect) > 0.01f)
        {
            aspectRatio = GameCamera.aspect;
            UpdateCameraMatrix();
        }
    }

    void endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        GL.invertCulling = false;
    }

    public void UpdateCameraMatrix()
    {
        GameCamera.ResetWorldToCameraMatrix();
        GameCamera.ResetProjectionMatrix();
        Vector3 scale = new Vector3(flipHorizontal ? -1 : 1, 1, 1);
        GameCamera.projectionMatrix = GameCamera.projectionMatrix * Matrix4x4.Scale(scale);
    }

    void OnValidate()
    {
        if (GameCamera == null)
            return;

        UpdateCameraMatrix();

#if UNITY_EDITOR
        RenderPipelineManager.beginCameraRendering -= beginCameraRendering;
        RenderPipelineManager.endCameraRendering -= endCameraRendering;
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            RenderPipelineManager.beginCameraRendering += beginCameraRendering;
            RenderPipelineManager.endCameraRendering += endCameraRendering;
        }
#endif
    }
}