using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public Material EffectMaterial;

    public Vector3 ScanOrigin;
    public float ScanDistance;
    public float MinScanDistance;
    public float MaxScanDistance;
    public float ScanGrowthFactor;

    private Camera mainCamera;
    private bool scanning;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            ScanDistance += ScanGrowthFactor * Time.deltaTime;
        else
            ScanDistance -= ScanGrowthFactor * Time.deltaTime;

        ScanDistance = Mathf.Clamp(ScanDistance, MinScanDistance, MaxScanDistance);
    }

    void OnEnable()
    {
        mainCamera = Camera.main;
        mainCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        EffectMaterial.SetVector("_WorldSpaceScannerPos", ScanOrigin);
        EffectMaterial.SetFloat("_ScanDistance", ScanDistance);
        RaycastCornerBlit(source, destination, EffectMaterial);
    }

    void RaycastCornerBlit(RenderTexture source, RenderTexture destination, Material mat)
    {
        float cameraFar = mainCamera.farClipPlane;
        float cameraFOV = mainCamera.fieldOfView;
        float cameraAspect = mainCamera.aspect;

        float cameraFOVHalf = cameraFOV * 0.5f;

        Vector3 toRight = mainCamera.transform.right * Mathf.Tan(cameraFOVHalf * Mathf.Deg2Rad) * cameraAspect;
        Vector3 toTop = mainCamera.transform.up * Mathf.Tan(cameraFOVHalf * Mathf.Deg2Rad);

        Vector3 topLeft = mainCamera.transform.forward - toRight + toTop;
        float cameraScale = topLeft.magnitude * cameraFar;

        topLeft.Normalize();
        topLeft *= cameraScale;

        Vector3 topRight = mainCamera.transform.forward + toRight + toTop;
        topRight.Normalize();
        topRight *= cameraScale;

        Vector3 bottomRight = mainCamera.transform.forward + toRight - toTop;
        bottomRight.Normalize();
        bottomRight *= cameraScale;

        Vector3 bottomLeft = mainCamera.transform.forward - toRight - toTop;
        bottomLeft.Normalize();
        bottomLeft *= cameraScale;

        RenderTexture.active = destination;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
