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
    public class SetDropOffPointStateSystem : SystemBase
    {
        private Entity _gameStateController;
        private Entity _selectedUnit;
        private Camera _mainCamera;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SelectedEntityTag>();
            RequireSingletonForUpdate<SetDropOffPointStateTag>();
            _physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _mainCamera = Camera.main;
        }

        protected override void OnStartRunning()
        {
            //Debug.Log("SetDropOffPointStateSystem OnStartRunning()");

            _gameStateController = GetSingletonEntity<GameStateControlTag>();
            _selectedUnit = GetSingletonEntity<SelectedEntityTag>();
            
            //TODO: Highlight all available drop-off points & maybe make the rest of the world B&W
        }

        protected override void OnUpdate()
        {
            if(Input.GetMouseButtonDown(0))
            {
                _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
                if (Raycast(rayStart, rayEnd, out var hit))
                {
                    if (!HasComponent<DropOffPointData>(_selectedUnit))
                    {
                        EntityManager.AddComponent<DropOffPointData>(_selectedUnit);
                    }
                    EntityManager.SetComponentData(_selectedUnit, new DropOffPointData{Value = hit.Entity});
                    ChangeToUnitSelectedState();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                ChangeToUnitSelectedState();
            }
        }
        
        private void ChangeToUnitSelectedState()
        {
            //Debug.Log("Changing to unit selected state");
            EntityManager.RemoveComponent<SetDropOffPointStateTag>(_gameStateController);
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
                    CollidesWith = (uint) CollisionLayers.DropOff
                }
            };

            raycastHit = new RaycastHit();
            return _collisionWorld.CastRay(input, out raycastHit);
        }

        protected override void OnStopRunning()
        {
            //Debug.Log("SetDropOffPointStateSystem OnStopRunning()");
        }
    }
}