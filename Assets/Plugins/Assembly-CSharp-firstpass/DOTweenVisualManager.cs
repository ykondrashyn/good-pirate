using DG.Tweening.Core;
using UnityEngine;

namespace DG.Tweening
{
    // Small lifecycle adapter for the five recovered visual-manager records.
    public sealed class DOTweenVisualManager : MonoBehaviour
    {
        public VisualManagerPreset preset;
        public OnEnableBehaviour onEnableBehaviour;
        public OnDisableBehaviour onDisableBehaviour;
        private void OnEnable()
        {
            switch (onEnableBehaviour)
            {
                case OnEnableBehaviour.Play: DOTween.Play(gameObject); break;
                case OnEnableBehaviour.Restart: DOTween.Restart(gameObject); break;
                case OnEnableBehaviour.RestartFromSpawnPoint:
                    foreach (var animation in GetComponents<ABSAnimationComponent>()) animation.DORestart(true);
                    break;
            }
        }
        private void OnDisable()
        {
            switch (onDisableBehaviour)
            {
                case OnDisableBehaviour.Pause: DOTween.Pause(gameObject); break;
                case OnDisableBehaviour.Rewind: DOTween.Rewind(gameObject); break;
                case OnDisableBehaviour.Kill: DOTween.Kill(gameObject); break;
                case OnDisableBehaviour.KillAndComplete: DOTween.Kill(gameObject, true); break;
                case OnDisableBehaviour.DestroyGameObject: Destroy(gameObject); break;
            }
        }
    }
}
