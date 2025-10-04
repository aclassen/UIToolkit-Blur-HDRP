using Blur.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

public class PingPongMover : MonoBehaviour {
    [SerializeField] private  UIDocument uiDocument;
    [SerializeField] private  AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private  float duration = 2f;
    [SerializeField] private float distance = 100f;
    private VisualElement _box;

    private void Start() {
        _box = uiDocument.rootVisualElement.Q<BlurPanel>();
    }

    private void Update() {
        var t = Mathf.PingPong(Time.time / duration, 1f);
        _box.style.left = -(distance * .5f) + curve.Evaluate(t) * distance;
    }
}