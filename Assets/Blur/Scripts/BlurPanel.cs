using UnityEngine;
using UnityEngine.UIElements;

namespace Blur.Scripts {
    [UxmlElement]
    public partial class BlurPanel : Image {
        private RenderTexture _renderTexture;

        [UxmlAttribute]
        public RenderTexture RenderTexture {
            get => _renderTexture;
            set {
                _renderTexture = value;
                image = _renderTexture;
            }
        }

        public BlurPanel() {
            RegisterCallback<GeometryChangedEvent>(UpdateImageRect);
            UpdateImageRect(null);
            scaleMode = ScaleMode.StretchToFill;
        }


        private void UpdateImageRect(GeometryChangedEvent ev) {
            if (!_renderTexture) return;

            var scaleX =_renderTexture.width / panel.visualTree.worldBound.width;
            var scaleY = _renderTexture.height / panel.visualTree.worldBound.height;
            sourceRect = new Rect(worldBound.x * scaleX, worldBound.y * scaleY, worldBound.width * scaleX, worldBound.height * scaleY);
            MarkDirtyRepaint();
        }
    }
}