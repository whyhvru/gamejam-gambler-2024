using Zenject;

namespace Module.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RocketGameService>()
                .AsSingle()
                .NonLazy();

            Container.Bind<SlotMachineService>()
                .AsSingle()
                .NonLazy();
        }
    }
}