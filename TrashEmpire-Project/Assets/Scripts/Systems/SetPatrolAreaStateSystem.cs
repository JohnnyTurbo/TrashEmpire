using Reese.Nav;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;


namespace TMG.TrashEmpire
{
    //[DisableAutoCreation]
    public class SetPatrolAreaStateSystem : SystemBase
    {
        private Entity _gameStateController;
        private SelectedEntityData _selectedEntityData;
        private Camera _mainCamera;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        private PatrolAreaPrefabData _patrolAreaPrefabData;
        private Entity _patrolAreaSelection;
        // private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        { 
            // _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            // This should be OnCreate() eventually
             Debug.Log("SetPatrolAreaStateSystem OnStartRunning");
            // Debug.Log("SetPatrolAreaStateSystem OnCreate");

            _gameStateController = GetSingletonEntity<GameStateControlTag>();
            _selectedEntityData = GetSingleton<SelectedEntityData>();
            _patrolAreaPrefabData = GetSingleton<PatrolAreaPrefabData>();
            _mainCamera = Camera.main;
            _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
            
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<SetPatrolAreaStateTag>();
            
            // This stuff should actually be in OnStartRunning()
            //var ecb = _ecbSystem.CreateCommandBuffer();
            //ecb.Instantiate(_patrolAreaData.PatrolAreaPrefab);
            _patrolAreaSelection = EntityManager.Instantiate(_patrolAreaPrefabData.PatrolAreaSelectionPrefab);
        }

        protected override void OnUpdate()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var rayStart = ray.origin;
            var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
            if (Raycast(rayStart, rayEnd, out var hit))
            {
                EntityManager.SetComponentData(_patrolAreaSelection, new Translation{Value = hit.Position});
                
                if (Input.GetMouseButtonDown(0))
                {
                    SetPatrolArea(hit.Position);

                    ChangeToUnitSelectState();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                ChangeToUnitSelectState();
            }
        }

        private void SetPatrolArea(float3 patrolOrigin)
        {
            var patrolArea = EntityManager.Instantiate(_patrolAreaPrefabData.PatrolAreaPrefab);
            EntityManager.SetComponentData(patrolArea, new Translation{Value = patrolOrigin});

            if (!HasComponent<PatrolAreaData>(_selectedEntityData.SelectedUnit))
            {
                EntityManager.AddComponent<PatrolAreaData>(_selectedEntityData.SelectedUnit);
            }
            
            var patrolAreaData = GetComponent<PatrolAreaData>(_selectedEntityData.SelectedUnit);
            patrolAreaData.Value = patrolArea;
            SetComponent(_selectedEntityData.SelectedUnit, patrolAreaData);

            
            
            /*if (HasComponent<NavDestination>(_selectedEntityData.SelectedUnit))
            {
                GetBuffer<NavPathBufferElement>(_selectedEntityData.SelectedUnit).Clear();
                EntityManager.RemoveComponent<NavDestination>(_selectedEntityData.SelectedUnit);
            }

            EntityManager.AddComponentData(_selectedEntityData.SelectedUnit, new NavDestination
            {
                WorldPoint = patrolOrigin,
                Teleport = false
            });*/
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
             Debug.Log("SetPatrolAreaStateSystem OnStopRunning");
            
            EntityManager.DestroyEntity(_patrolAreaSelection);
        }
    }
}