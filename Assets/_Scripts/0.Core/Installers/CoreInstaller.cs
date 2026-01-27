using Zenject;

namespace Module.Core
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameClock>()
                .AsSingle()
                .NonLazy();
        }
    }
}