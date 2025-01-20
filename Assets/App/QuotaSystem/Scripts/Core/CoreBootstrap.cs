using DeepGame.Quota;
using UnityEditor;
using UnityEngine;

namespace VRSim.Core
{
    public class CoreBootstrap : MonoBehaviour
    {
        [SerializeField]
        private QuotaManager _quotaManager;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!EditorApplication.isPlaying)
            {
                ServiceLocator.Clear();
                SetupServices();
            }
        }
#endif

        private void Awake()
        {
            ServiceLocator.Clear();
            SetupServices();

            _quotaManager.InitializeQuota();
        }

        private void OnDestroy()
        {
            ServiceLocator.Dispose();
        }

        private void SetupServices()
        {
            ServiceLocator.Register(_quotaManager);
        }
    }
}
