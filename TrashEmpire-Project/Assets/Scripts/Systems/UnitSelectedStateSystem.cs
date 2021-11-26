using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace TMG.TrashEmpire
{
    [UpdateAfter(typeof(TestRaycastSelectionSystem))]
    public class UnitSelectedStateSystem : SystemBase
    {
        private Entity _gameStateController;
        private Entity _patrolAreaRenderEntity;
        private Entity _selectedUnit;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<UnitSelectStateTag>();
        }

        protected override void OnStartRunning()
        {
            Debug.Log("UnitSelectedStateSystem OnStartRunning");
            _gameStateController = GetSingletonEntity<GameStateControlTag>();
            
            _selectedUnit = GetSingletonEntity<SelectedEntityTag>();
            if (HasComponent<PatrolAreaData>(_selectedUnit))
            {
                var patrolAreaEntity = GetComponent<PatrolAreaData>(_selectedUnit).Value;
                _patrolAreaRenderEntity = GetBuffer<LinkedEntityGroup>(patrolAreaEntity)[1].Value;
                EntityManager.RemoveComponent<DisableRendering>(_patrolAreaRenderEntity);
            }
        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Set Patrol Area");
                ChangeToSetPatrolAreaState();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Set Drop-off Point");
            }
        }

        protected override void OnStopRunning()
        {
            Debug.Log("UnitSelectedStateSystem OnStopRunning");
            if (HasComponent<PatrolAreaData>(_selectedUnit))
            {
                EntityManager.AddComponent<DisableRendering>(_patrolAreaRenderEntity);
            }
        }

        private void ChangeToSetPatrolAreaState()
        {
            EntityManager.RemoveComponent<UnitSelectStateTag>(_gameStateController);
            EntityManager.AddComponent<SetPatrolAreaStateTag>(_gameStateController);
        }
    }
}