using Module.Presentation.Audio;
using UnityEngine;
using Zenject;

namespace Module.Application
{
    public sealed class ApplicationInstaller : MonoInstaller
    {
        [Header("Audio")]
        [SerializeField] private AudioConfigSO _audioConfig;

        public override void InstallBindings()
        {
            Container.Bind<AudioPlayer>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.BindInstance(_audioConfig)
                .AsSingle();

            Container.BindInterfacesTo<DataService>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<AppFlowService>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<AudioService>()
                .AsSingle()
                .NonLazy();
        }
    }
}