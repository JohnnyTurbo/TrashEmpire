using Reese.Nav;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;


namespace TMG.TrashEmpire
{
    public class SetPatrolAreaStateSystem : SystemBase
    {
        private Entity _gameStateController;
        private SelectedEntityData _selectedEntityData;
        private Camera _mainCamera;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        
        protected override void OnStartRunning()
        {
            //Debug.Log("SetPatrolAreaStateSystem OnStartRunning");
            
            _gameStateController = GetSingletonEntity<GameStateControlTag>();
            _selectedEntityData = GetSingleton<SelectedEntityData>();
            _mainCamera = Camera.main;
            _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
            
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<SetPatrolAreaStateTag>();
        }
        
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
                RaycastHit hit;
                if (Raycast(rayStart, rayEnd, out hit))
                {
                    if (HasComponent<NavDestination>(_selectedEntityData.SelectedUnit))
                    {
                        GetBuffer<NavPathBufferElement>(_selectedEntityData.SelectedUnit).Clear();
                        EntityManager.RemoveComponent<NavDestination>(_selectedEntityData.SelectedUnit);
                    }

                    EntityManager.AddComponentData(_selectedEntityData.SelectedUnit, new NavDestination
                    {
                        WorldPoint = hit.Position,
                        Teleport = false
                    });

                    ChangeToUnitSelectState();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeToUnitSelectState();
            }
        }

        private void ChangeToUnitSelectState()
        {
            EntityManager.RemoveComponent<SetPatrolAreaStateTag>(_gameStateController);
            EntityManager.AddComponent<UnitSelectStateTag>(_gameStateController);
        }
        
        private bool Raycast(float3 rayFrom, float3 rayTo, out RaycastHit raycastHit)
        {
            RaycastInput input = new RaycastInput()
            {
                Start = rayFrom,
                End = rayTo,
                Filter = CollisionFilter.Default
            };

            raycastHit = new RaycastHit();
            return _collisionWorld.CastRay(input, out raycastHit);
        }

        protected override void OnStopRunning()
        {
            //Debug.Log("SetPatrolAreaStateSystem OnStopRunning");
        }
    }
}