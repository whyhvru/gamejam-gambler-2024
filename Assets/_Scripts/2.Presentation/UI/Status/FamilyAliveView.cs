using Module.Core;
using Module.Gameplay;
using UnityEngine;
using Zenject;

namespace Module.Presentation.UI
{
    public sealed class FamilyAliveView : MonoBehaviour
    {
        [SerializeField] private GameObject _mother;
        [SerializeField] private GameObject _wife;
        [SerializeField] private GameObject _child1;
        [SerializeField] private GameObject _child2;

        private IDataService _data;
        private SurvivalService _survival;

        [Inject]
        public void Construct(IDataService data, SurvivalService survival)
        {
            _data = data;
            _survival = survival;
        }

        private void OnEnable()
        {
            _survival.OnMemberDied += HandleMemberDied;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (_survival != null)
                _survival.OnMemberDied -= HandleMemberDied;
        }

        private void HandleMemberDied(EFamilyMember _) => RefreshAll();

        private void RefreshAll()
        {
            var s = _data.SaveData;

            SetActiveSafe(_mother, s.motherIsAlive);
            SetActiveSafe(_wife, s.wifeIsAlive);
            SetActiveSafe(_child1, s.childIsAlive);
            SetActiveSafe(_child2, s.child2IsAlive);
        }

        private static void SetActiveSafe(GameObject go, bool active)
        {
            if (go != null)
                go.SetActive(active);
        }
    }
}