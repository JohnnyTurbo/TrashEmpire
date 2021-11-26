using Unity.Entities;
using Unity.Physics.Stateful;

namespace TMG.TrashEmpire
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(TriggerEventConversionSystem))]
    public class PatrolAreaTrashTriggerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer, ref DynamicBuffer<TrashBufferElement> trashBuffer) =>
            {
                foreach (var triggerEvent in triggerEventBuffer)
                {
                    var otherEntity = triggerEvent.GetOtherEntity(e);
                    
                    if(triggerEvent.State == EventOverlapState.Stay){continue;}

                    if (triggerEvent.State == EventOverlapState.Enter)
                    {
                        trashBuffer.Add(otherEntity);
                    }
                    else
                    {
                        
                        for (var i = 0; i < trashBuffer.Length; i++)
                        {
                            if (trashBuffer[i] == otherEntity)
                            {
                                trashBuffer.RemoveAtSwapBack(i);
                                break;
                            }
                        }
                    }
                }
            }).Run();
        }
    }
}