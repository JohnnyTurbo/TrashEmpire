using System;
using Reese.Nav;
using Unity.Entities;
using UnityEngine;

namespace TMG.TrashEmpire
{
    public class PointAndClickTest : MonoBehaviour
    {
        private Camera _mainCamera;
        private EntityManager _entityManager;
        private Entity _playerEntity;
        
        private void Start()
        {
            _mainCamera = Camera.main;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = _entityManager.CreateEntityQuery(typeof(PlayerTag));
            _playerEntity = playerQuery.GetSingletonEntity();
            Debug.Log(_playerEntity.Index);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (_entityManager.HasComponent<NavDestination>(_playerEntity))
                    {
                        _entityManager.GetBuffer<NavPathBufferElement>(_playerEntity).Clear();
                        _entityManager.RemoveComponent<NavDestination>(_playerEntity);
                    }
                    
                    _entityManager.AddComponentData(_playerEntity, new NavDestination
                    {
                        WorldPoint = hit.point,
                        Teleport = false
                    });
                }
            }
        }
    }
}