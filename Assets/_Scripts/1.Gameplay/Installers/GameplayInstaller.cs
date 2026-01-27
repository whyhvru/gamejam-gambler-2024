using Zenject;

namespace Module.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SurvivalService>()
                .AsSingle()
                .NonLazy();

            Container.Bind<MessageService>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<RocketGameService>()
                .AsSingle()
                .NonLazy();

            Container.Bind<SlotMachineService>()
                .AsSingle()
                .NonLazy();
        }
    }
}