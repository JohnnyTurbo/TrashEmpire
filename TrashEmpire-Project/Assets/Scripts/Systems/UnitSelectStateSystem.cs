using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace TMG.TrashEmpire
{
    //[AlwaysUpdateSystem]
    //[DisableAutoCreation]
    public class UnitSelectStateSystem : SystemBase
    {
        public bool UnitSelected => _unitSelected;

        private Camera _mainCamera;
        private bool _unitSelected;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        private SelectionUIData _selectionUIData;
        private Entity _selectedUnit;
        private Entity _patrolAreaRenderEntity;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<UnitSelectStateTag>();
        }

        protected override void OnStartRunning()
        {
            //Debug.Log("UnitSelectStateSystem OnStartRunning()");
            
            _mainCamera = Camera.main;
            
            _selectionUIData = GetSingleton<SelectionUIData>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //DeselectUnit();
                _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
                _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;

                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
                
                if (Raycast(rayStart, rayEnd, out var hit))
                {
                    var selectedEntity = _physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    Debug.Log(selectedEntity.Index + "Hit");
                    if (EntityManager.HasComponent<SelectableUnitTag>(selectedEntity))
                    {
                        if (_selectedUnit != selectedEntity)
                        {
                            DeselectUnit();
                            SelectUnit(selectedEntity);
                        }
                    }
                    else
                    {
                        // Clicked on non-selectable object
                        DeselectUnit();
                    }
                }
                else
                {
                    // Raycast did not hit anything
                    DeselectUnit();
                }
            }
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
                    CollidesWith = (uint) (CollisionLayers.Ground | CollisionLayers.Units)
                }
            };

            raycastHit = new RaycastHit();
            return _collisionWorld.CastRay(input, out raycastHit);
        }

        private void SelectUnit(Entity selectedUnit)
        {
            Debug.Log($"Selecting Entity ID: {selectedUnit.Index}, Version: {selectedUnit.Version}");
            _unitSelected = true;

            EntityManager.AddComponent<SelectedEntityTag>(selectedUnit);
            _selectedUnit = selectedUnit;
            var selectionUI = EntityManager.Instantiate(_selectionUIData.SelectionUIPrefab);
            _selectionUIData.SelectionUI = selectionUI;
            SetSingleton(_selectionUIData);
            EntityManager.AddComponentData(selectionUI, new Parent {Value = selectedUnit});
            EntityManager.AddComponentData(selectionUI, new LocalToParent {Value = float4x4.zero});
            ShowHidePatrolArea(true);
        }

        private void DeselectUnit()
        {
            Debug.Log("Deselecting Unit");
            _unitSelected = false;
            
            EntityManager.RemoveComponent<SelectedEntityTag>(_selectedUnit);
            ShowHidePatrolArea(false);

            _selectedUnit = Entity.Null;
            EntityManager.DestroyEntity(_selectionUIData.SelectionUI);
            _selectionUIData.SelectionUI = Entity.Null;
            SetSingleton(_selectionUIData);
        }

        private void ShowHidePatrolArea(bool shouldShow)
        {
            if (!HasComponent<PatrolAreaData>(_selectedUnit)) return;
            
            var patrolAreaEntity = GetComponent<PatrolAreaData>(_selectedUnit).Value;
            _patrolAreaRenderEntity = GetBuffer<LinkedEntityGroup>(patrolAreaEntity)[1].Value;
            if (shouldShow)
            {
                EntityManager.RemoveComponent<DisableRendering>(_patrolAreaRenderEntity);
            }
            else
            {
                EntityManager.AddComponent<DisableRendering>(_patrolAreaRenderEntity);
            }
        }
        
        protected override void OnStopRunning()
        {
            //Debug.Log("UnitSelectStateSystem OnStopRunning()");
        }
    }
}