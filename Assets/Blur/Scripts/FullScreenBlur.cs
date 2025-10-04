using System;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Blur.Scripts {
#if UNITY_EDITOR
    [CustomPassDrawer(typeof(FullScreenBlur))]
    public class FullScreenBlurDrawer : CustomPassDrawer {
        protected override PassUIFlag commonPassUIFlags => PassUIFlag.Name;
    }
#endif
    [Serializable]
    public class FullScreenBlur : CustomPass {
        [SerializeField] private float radius = 8.0f;
        [SerializeField] private int sampleCount = 9;
        [SerializeField] private RenderTexture renderTexture;
        [Min(0.1f)] [SerializeField] private float outputTextureScale = .5f;
        private RTHandle _tempRenderTextureHandle;
        private RTHandle _halfResTarget;
        private RTHandle _renderTextureHandle;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd) {
            if (renderTexture == null) return;
            _halfResTarget = RTHandles.Alloc(
                Vector2.one * 0.5f,
                TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: GraphicsFormat.B10G11R11_UFloatPack32,
                useDynamicScale: true,
                name: "Half Res Custom Pass"
            );

            _tempRenderTextureHandle = RTHandles.Alloc(
                Vector2.one * outputTextureScale,
                TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: renderTexture.graphicsFormat,
                useDynamicScale: true,
                name: "Extra Buffer"
            );
            _renderTextureHandle = RTHandles.Alloc(renderTexture);
        }

        protected override void Execute(CustomPassContext ctx) {
            if (ctx.hdCamera.camera.cameraType == CameraType.SceneView || renderTexture == null) {
                return;
            }

            SyncRenderTextureAspect(renderTexture, ctx.hdCamera.camera);
            CustomPassUtils.GaussianBlur(ctx, ctx.cameraColorBuffer, _tempRenderTextureHandle, _halfResTarget, sampleCount, radius,
                downSample: true);
            CustomPassUtils.Copy(ctx, _tempRenderTextureHandle, _renderTextureHandle);
        }

        private void SyncRenderTextureAspect(RenderTexture rt, Camera camera) {
            var aspect = rt.width / (float)rt.height;
            if (Mathf.Approximately(aspect, camera.aspect) && Mathf.Approximately(rt.width, camera.pixelWidth * outputTextureScale)) return;
            rt.Release();
            rt.width = (int)(camera.pixelWidth * outputTextureScale);
            rt.height = (int)(camera.pixelHeight * outputTextureScale);
            rt.Create();
        }

        protected override void Cleanup() {
            _halfResTarget?.Release();
            _renderTextureHandle?.Release();
            _tempRenderTextureHandle?.Release();
        }
    }
}