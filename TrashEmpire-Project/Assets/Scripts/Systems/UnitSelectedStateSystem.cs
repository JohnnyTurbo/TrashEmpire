using Unity.Entities;
using UnityEngine;

namespace TMG.TrashEmpire
{
    [UpdateAfter(typeof(UnitSelectStateSystem))]
    public class UnitSelectedStateSystem : SystemBase
    {
        private Entity _gameStateController;

        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<UnitSelectStateTag>();
            
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            //Debug.Log("UnitSelectedStateSystem OnStartRunning");
            _gameStateController = GetSingletonEntity<GameStateControlTag>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Debug.Log("Set Patrol Area");
                ChangeToSetPatrolAreaState();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Set Drop-off Point");
                ChangeToSetDropOffPointState();
            }
        }

        private void ChangeToSetPatrolAreaState()
        {
            EntityManager.RemoveComponent<UnitSelectStateTag>(_gameStateController);
            EntityManager.AddComponent<SetPatrolAreaStateTag>(_gameStateController);
        }

        private void ChangeToSetDropOffPointState()
        {
            EntityManager.RemoveComponent<UnitSelectStateTag>(_gameStateController);
            EntityManager.AddComponent<SetDropOffPointStateTag>(_gameStateController);
        }

        protected override void OnStopRunning()
        {
            //Debug.Log("UnitSelectedStateSystem OnStopRunning");
        }
    }
}