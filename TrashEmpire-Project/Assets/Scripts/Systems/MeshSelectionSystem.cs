using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace TMG.TrashEmpire
{
    //[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public class MeshSelectionSystem : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private BeginInitializationEntityCommandBufferSystem ecbSystem;
        
        protected override void OnCreate()
        {
            _stepPhysicsWorld = EntityManager.World.GetOrCreateSystem<StepPhysicsWorld>();
            _buildPhysicsWorld = EntityManager.World.GetOrCreateSystem<BuildPhysicsWorld>();
            RequireSingletonForUpdate<SelectionTag>();
            ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        
        protected override void OnUpdate()
        {
            var ecb = ecbSystem.CreateCommandBuffer();
            var seq = EntityManager.CreateEntityQuery(typeof(SelectionTag));
            var teq = EntityManager.CreateEntityQuery(typeof(TrashData));
            
            Debug.Log($"{seq.CalculateEntityCount()} selections and {teq.CalculateEntityCount()} trashes and {seq.ToEntityArray(Allocator.Temp)[0].Index}");
            //EditorApplication.isPaused = true;
            
            var jobHandle = new SelectionJob
            {
                selectionGroup = GetComponentDataFromEntity<SelectionTag>(),
                trashGroup = GetComponentDataFromEntity<TrashData>()
            }.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, Dependency);
            
            jobHandle.Complete();
            
            //EditorApplication.isPaused = true;
            EntityManager.DestroyEntity(seq);
            //ecb.DestroyEntity(seq);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EntityManager.DestroyEntity(seq);
            }
            
        }
        
        struct SelectionJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<SelectionTag> selectionGroup;
            public ComponentDataFromEntity<TrashData> trashGroup;
            public DynamicBuffer<TrashBufferElement> trashToPickUp;
            
            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.EntityA;
                var entityB = triggerEvent.EntityB;

                var isBodyASelection = selectionGroup.HasComponent(entityA);
                var isBodyBSelection = selectionGroup.HasComponent(entityB);
            
                if(isBodyASelection && isBodyBSelection){return;}

                var isBodyATrash = trashGroup.HasComponent(entityA);
                var isBodyBTrash = trashGroup.HasComponent(entityB);
            
                if((isBodyASelection && !isBodyBTrash) || (isBodyBSelection && !isBodyATrash)){return;}

                //var selectionEntity = isBodyASelection ? entityA : entityB;
                var trashEntity = isBodyASelection ? entityB : entityA;

                trashToPickUp.Add(trashEntity);
            }
        }
    }
}