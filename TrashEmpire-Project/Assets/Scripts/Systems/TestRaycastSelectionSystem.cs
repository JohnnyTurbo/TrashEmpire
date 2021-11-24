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
    //[AlwaysUpdateSystem]
    [DisableAutoCreation]
    public class TestRaycastSelectionSystem : SystemBase
    {
        public bool EntitySelected => _entitySelected;

        private Camera _mainCamera;
        private bool _entitySelected;
        private bool _justSelected;
        private BuildPhysicsWorld _physicsWorldSystem;
        private CollisionWorld _collisionWorld;
        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
            _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
                RaycastHit hit;
                if (Raycast(rayStart, rayEnd, out hit) && !_entitySelected)
                {
                    var selectedEntity = _physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    if (EntityManager.HasComponent<SelectableTag>(selectedEntity))
                    {
                        EntityManager.AddComponent<SelectedEntityTag>(selectedEntity);
                        _entitySelected = true;
                        _justSelected = true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_justSelected)
                {
                    _justSelected = false;
                }
                else
                {
                    var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(_mainCamera.farClipPlane);
                RaycastHit hit;
                if (Raycast(rayStart, rayEnd, out hit) && _entitySelected)
                {

                    _entitySelected = false;
                    Entities.WithAll<SelectedEntityTag>().ForEach((Entity e) =>
                    {
                        if (HasComponent<NavDestination>(e))
                        {
                            GetBuffer<NavPathBufferElement>(e).Clear();
                            EntityManager.RemoveComponent<NavDestination>(e);
                        }

                        EntityManager.AddComponentData(e, new NavDestination
                        {
                            WorldPoint = hit.Position,
                            Teleport = false
                        });
                    }).WithStructuralChanges().WithoutBurst().Run();
                }

                /*else
                {
                    var selectedEntity = _physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    if (EntityManager.HasComponent<SelectableTag>(selectedEntity))
                    {
                        EntityManager.AddComponent<SelectedEntityTag>(selectedEntity);
                        _entitySelected = true;
                    }
                }*/
                }
            }
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
    }
}