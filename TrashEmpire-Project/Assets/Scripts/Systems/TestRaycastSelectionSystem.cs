using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace TMG.TrashEmpire
{
    //[AlwaysUpdateSystem]
    //[DisableAutoCreation]
    public class TestRaycastSelectionSystem : SystemBase
    {
        public bool EntitySelected => _entitySelected;

        private Camera _mainCamera;
        private bool _entitySelected;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        private SelectedEntityData _selectedEntityData;
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<UnitSelectStateTag>();
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            Debug.Log("TestRaycastSelectionSystem OnStartRunning()");
            
            _mainCamera = Camera.main;
            
            _selectedEntityData = GetSingleton<SelectedEntityData>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DeselectUnit();
                _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
                _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;

                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
                
                if (Raycast(rayStart, rayEnd, out var hit))
                {
                    var selectedEntity = _physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    if (EntityManager.HasComponent<SelectableUnitTag>(selectedEntity))
                    {
                        if (_selectedEntityData.SelectedUnit != selectedEntity)
                        {
                            SelectUnit(selectedEntity);
                        }
                    }
                }
            }
        }

        private void SelectUnit(Entity selectedEntity)
        {
            // Debug.Log($"Selecting Entity ID: {selectedEntity.Index}, Version: {selectedEntity.Version}");

            var ecb = _ecbSystem.CreateCommandBuffer();
            ecb.AddComponent<SelectedEntityTag>(selectedEntity);
            _selectedEntityData.SelectedUnit = selectedEntity;
            _entitySelected = true;
            var selectionUI = EntityManager.Instantiate(_selectedEntityData.SelectionUIPrefab);
            _selectedEntityData.SelectionUI = selectionUI;
            SetSingleton(_selectedEntityData);
            EntityManager.AddComponentData(selectionUI, new Parent {Value = selectedEntity});
            EntityManager.AddComponentData(selectionUI, new LocalToParent {Value = float4x4.zero});
        }

        private void DeselectUnit()
        {
            //Debug.Log("Deselecting Unit");
            EntityManager.RemoveComponent<SelectedEntityTag>(_selectedEntityData.SelectedUnit);
            
            _selectedEntityData.SelectedUnit = Entity.Null;
            EntityManager.DestroyEntity(_selectedEntityData.SelectionUI);
            _selectedEntityData.SelectionUI = Entity.Null;
            SetSingleton(_selectedEntityData);
            _entitySelected = false;
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
            Debug.Log("TestRaycastSelectionSystem OnStopRunning()");
        }
    }
}