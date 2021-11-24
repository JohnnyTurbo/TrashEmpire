using Unity.Entities;
using UnityEngine;

namespace TMG.TrashEmpire
{
    public class UnitSelectedStateSystem : SystemBase
    {
        private Entity _gameStateController;
        
        protected override void OnStartRunning()
        {
            //Debug.Log("UnitSelectedStateSystem OnStartRunning");
            _gameStateController = GetSingletonEntity<GameStateControlTag>();
            
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<UnitSelectStateTag>();
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
            //Debug.Log("UnitSelectedStateSystem OnStopRunning");
        }

        private void ChangeToSetPatrolAreaState()
        {
            EntityManager.RemoveComponent<UnitSelectStateTag>(_gameStateController);
            EntityManager.AddComponent<SetPatrolAreaStateTag>(_gameStateController);
        }
    }
}