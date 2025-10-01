using Module.Core;
using Module.Gameplay;
using UnityEngine;
using UnityEngine.Audio;

namespace Module.Application
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;

        private IGameFlowService _gameFlowService;

        private void Awake()
        {
            InitializeServices();
            LoadMainMenu();
        }

        private void InitializeServices()
        {
            DataService dataService = new();
            ServiceRegistry.Register<IDataService>(dataService);

            SceneLoaderService sceneLoaderService = new();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoaderService);

            SettingsService settingsService = new(_audioMixer);
            ServiceRegistry.Register<ISettingsService>(settingsService);

            _gameFlowService = new GameFlowService(dataService, sceneLoaderService);
            ServiceRegistry.Register<IGameFlowService>(_gameFlowService);

            GameTimeService gameTimeService = new(dataService);
            ServiceRegistry.Register<IGameTimeService>(gameTimeService);

            BalanceService balanceService = new(dataService);
            ServiceRegistry.Register(balanceService);

            ServiceRegistry.Register<DebtService>(new DebtService(dataService, balanceService));
        }

        private void LoadMainMenu()
        {
            _gameFlowService.LoadMainMenu();
        }
    }
}