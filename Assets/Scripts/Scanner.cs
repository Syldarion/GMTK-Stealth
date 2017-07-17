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
    public float ScanGrowthSpeed;
    public float ScanDecaySpeed;
    public bool Scanning;
    public float ScanTime;
    public float ScanPingTime;
    public float ScanPingInterval;

    private Camera mainCamera;
    private IsoFollowCamera followCamera;
    private AudioSource scanSoundSource;

    void Awake()
    {
        followCamera = GetComponent<IsoFollowCamera>();
        scanSoundSource = GetComponent<AudioSource>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !Player.Instance.Hiding)
        {
            ScanTime += Time.deltaTime;
            ScanPingTime += Time.deltaTime;
            ScanDistance += ScanGrowthSpeed * Time.deltaTime;
            if (!Scanning)
            {
                scanSoundSource.loop = true;
                scanSoundSource.Play();
            }
            Scanning = true;

            CheckScanRange();
        }
        else
        {
            ScanPingTime = 0.0f;
            ScanTime = Mathf.Clamp(ScanTime - Time.deltaTime, 0.0f, ScanTime);
            ScanDistance -= ScanDecaySpeed * Time.deltaTime;
            if (Scanning)
            {
                scanSoundSource.loop = false;
            }
            Scanning = false;
        }

        ScanDistance = Mathf.Clamp(ScanDistance, MinScanDistance, MaxScanDistance);
        
        followCamera.FollowHeight = 4.0f + (ScanDistance - MinScanDistance) * 1.8f;
    }

    public void CheckScanRange()
    {
        if(ScanPingTime >= ScanPingInterval)
        {
            ScanPingTime = 0.0f;

            Collider[] enemies = Physics.OverlapSphere(ScanOrigin, ScanDistance, 1 << 8);

            foreach(Collider col in enemies)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if(enemy.Questioning)
                {
                    enemy.Alert(ScanOrigin);
                }
                else if(!enemy.Alerted)
                {
                    enemy.Question();
                }
            }
        }
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
        EffectMaterial.SetFloat("_ScanlineBoldness", 
            Player.Instance.Hiding ? 0.0f : Scanning ? 0.8f : 0.1f);
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
