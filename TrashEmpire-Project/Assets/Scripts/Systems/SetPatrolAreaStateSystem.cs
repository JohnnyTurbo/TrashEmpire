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
    [UpdateAfter(typeof(UnitSelectedStateSystem))]
    public class SetPatrolAreaStateSystem : SystemBase
    {
        private Entity _gameStateController;
        private Entity _selectedUnit;
        private Camera _mainCamera;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        private PatrolAreaPrefabData _patrolAreaPrefabData;
        private Entity _patrolAreaSelection;

        protected override void OnCreate()
        { 
            // Debug.Log("SetPatrolAreaStateSystem OnCreate");
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<SetPatrolAreaStateTag>();
        }

        protected override void OnStartRunning()
        {
             //Debug.Log("SetPatrolAreaStateSystem OnStartRunning");
             
            _gameStateController = GetSingletonEntity<GameStateControlTag>();
            _selectedUnit = GetSingletonEntity<SelectedEntityTag>();
            _patrolAreaPrefabData = GetSingleton<PatrolAreaPrefabData>();
            _mainCamera = Camera.main;
            _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            
            _patrolAreaSelection = EntityManager.Instantiate(_patrolAreaPrefabData.PatrolAreaSelectionPrefab);
        }

        protected override void OnUpdate()
        {
            _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var rayStart = ray.origin;
            var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
            if (Raycast(rayStart, rayEnd, out var hit))
            {
                EntityManager.SetComponentData(_patrolAreaSelection, new Translation{Value = hit.Position});
                
                if (Input.GetMouseButtonDown(0))
                {
                    SetPatrolArea(hit.Position);

                    ChangeToUnitSelectedState();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                ChangeToUnitSelectedState();
            }
        }

        private void SetPatrolArea(float3 patrolOrigin)
        {
            Entity patrolArea;
            
            if (!HasComponent<PatrolAreaData>(_selectedUnit))
            {
                EntityManager.AddComponent<PatrolAreaData>(_selectedUnit);
                patrolArea = EntityManager.Instantiate(_patrolAreaPrefabData.PatrolAreaPrefab);
                SetComponent(_selectedUnit, new PatrolAreaData{Value = patrolArea});
            }
            else
            {
                patrolArea = GetComponent<PatrolAreaData>(_selectedUnit).Value;
            }
            EntityManager.SetComponentData(patrolArea, new Translation{Value = patrolOrigin});
        }

        private void ChangeToUnitSelectedState()
        {
            //Debug.Log("Changing to unit selected state");
            EntityManager.RemoveComponent<SetPatrolAreaStateTag>(_gameStateController);
            EntityManager.AddComponent<UnitSelectStateTag>(_gameStateController);
        }
        
        private bool Raycast(float3 rayFrom, float3 rayTo, out RaycastHit raycastHit)
        {
            RaycastInput input = new RaycastInput()
            {
                Start = rayFrom,
                End = rayTo,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint) CollisionLayers.Raycast,
                    CollidesWith = (uint) CollisionLayers.Ground
                }
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