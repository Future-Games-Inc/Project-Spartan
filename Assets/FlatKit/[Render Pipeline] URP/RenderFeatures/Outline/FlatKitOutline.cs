using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FlatKit {
public class FlatKitOutline : ScriptableRendererFeature {
    [Tooltip("To create new settings use 'Create > FlatKit > Outline Settings'.")]
    public OutlineSettings settings;

    [SerializeField]
    [HideInInspector]
    // ReSharper disable once InconsistentNaming
    private Material _effectMaterial;

    private DustyroomRenderPass _fullScreenPass;
    private bool _requiresColor;
    private bool _injectedBeforeTransparents;
    private ScriptableRenderPassInput _requirements = ScriptableRenderPassInput.Color;

    private static readonly string OutlineShaderName = "Hidden/FlatKit/OutlineWrap";
    private static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
    private static readonly int Thickness = Shader.PropertyToID("_Thickness");
    private static readonly int DepthThresholdMin = Shader.PropertyToID("_DepthThresholdMin");
    private static readonly int DepthThresholdMax = Shader.PropertyToID("_DepthThresholdMax");
    private static readonly int NormalThresholdMin = Shader.PropertyToID("_NormalThresholdMin");
    private static readonly int NormalThresholdMax = Shader.PropertyToID("_NormalThresholdMax");
    private static readonly int ColorThresholdMin = Shader.PropertyToID("_ColorThresholdMin");
    private static readonly int ColorThresholdMax = Shader.PropertyToID("_ColorThresholdMax");

    public override void Create() {
        // Settings.
        {
            if (settings == null) return;
            settings.onSettingsChanged = null;
            settings.onReset = null;
            settings.onSettingsChanged += SetMaterialProperties;
            settings.onReset += CreateMaterial;
        }

        // Material.
        if (_effectMaterial == null) {
            CreateMaterial();
        }

        SetMaterialProperties();

        {
            _fullScreenPass = new DustyroomRenderPass {
                renderPassEvent = settings.renderEvent,
            };

            _requirements = ScriptableRenderPassInput.Color; // Needed for the full-screen blit.
            if (settings.useDepth) _requirements |= ScriptableRenderPassInput.Depth;
            if (settings.useNormals) _requirements |= ScriptableRenderPassInput.Normal;
            ScriptableRenderPassInput modifiedRequirements = _requirements;

            _requiresColor = (_requirements & ScriptableRenderPassInput.Color) != 0;
            _injectedBeforeTransparents = settings.renderEvent <= RenderPassEvent.BeforeRenderingTransparents;

            if (_requiresColor && !_injectedBeforeTransparents) {
                modifiedRequirements ^= ScriptableRenderPassInput.Color;
            }

            _fullScreenPass.ConfigureInput(modifiedRequirements);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (!settings.applyInSceneView && renderingData.cameraData.isSceneViewCamera) return;
        if (renderingData.cameraData.isPreviewCamera) return;
        if (_effectMaterial == null) return;

        _fullScreenPass.Setup(_effectMaterial, _requiresColor, _injectedBeforeTransparents, "Flat Kit Outline",
            renderingData);
        renderer.EnqueuePass(_fullScreenPass);
    }

    protected override void Dispose(bool disposing) {
        _fullScreenPass.Dispose();
    }

    private void CreateMaterial() {
        var effectShader = Shader.Find(OutlineShaderName);

        // This may happen on project load.
        if (effectShader == null) return;

        _effectMaterial = CoreUtils.CreateEngineMaterial(effectShader);
#if UNITY_EDITOR
        AlwaysIncludedShaders.Add(OutlineShaderName);
#endif
    }

    private void SetMaterialProperties() {
        if (_effectMaterial == null) return;

        const string depthKeyword = "OUTLINE_USE_DEPTH";
        SetKeyword(_effectMaterial, depthKeyword, settings.useDepth);

        const string normalsKeyword = "OUTLINE_USE_NORMALS";
        SetKeyword(_effectMaterial, normalsKeyword, settings.useNormals);

        const string colorKeyword = "OUTLINE_USE_COLOR";
        SetKeyword(_effectMaterial, colorKeyword, settings.useColor);

        const string outlineOnlyKeyword = "OUTLINE_ONLY";
        SetKeyword(_effectMaterial, outlineOnlyKeyword, settings.outlineOnly);

        const string resolutionInvariantKeyword = "RESOLUTION_INVARIANT_THICKNESS";
        SetKeyword(_effectMaterial, resolutionInvariantKeyword, settings.resolutionInvariant);

        _effectMaterial.SetColor(EdgeColor, settings.edgeColor);
        _effectMaterial.SetFloat(Thickness, settings.thickness);

        _effectMaterial.SetFloat(DepthThresholdMin, settings.minDepthThreshold);
        _effectMaterial.SetFloat(DepthThresholdMax, settings.maxDepthThreshold);

        _effectMaterial.SetFloat(NormalThresholdMin, settings.minNormalsThreshold);
        _effectMaterial.SetFloat(NormalThresholdMax, settings.maxNormalsThreshold);

        _effectMaterial.SetFloat(ColorThresholdMin, settings.minColorThreshold);
        _effectMaterial.SetFloat(ColorThresholdMax, settings.maxColorThreshold);
    }

    private static void SetKeyword(Material material, string keyword, bool enabled) {
        if (enabled) {
            material.EnableKeyword(keyword);
        } else {
            material.DisableKeyword(keyword);
        }
    }
}
}