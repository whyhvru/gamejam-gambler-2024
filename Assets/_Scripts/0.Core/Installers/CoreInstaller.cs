using UnityEngine;
using Zenject;

namespace Module.Core
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] private UnityTimer _timer;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<UnityTimer>()
                .FromInstance(_timer);

            Container.BindInterfacesAndSelfTo<GameClock>()
                .AsSingle()
                .NonLazy();
        }
    }
}