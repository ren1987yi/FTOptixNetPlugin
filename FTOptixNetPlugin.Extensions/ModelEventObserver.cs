using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAManagedCore;

namespace FTOptixNetPlugin.Extensions
{

    /// <summary>
    /// 模型事件观察者
    /// </summary>
    public class ModelEventObserver : IDisposable
    {

        /// <summary>
        /// 模型观察者
        /// </summary>
        ModelObjectObserver observer;
        /// <summary>
        /// 事件注册
        /// </summary>
        IEventRegistration eventRegistration;

        uint affinityId = 0;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="model">观察的模型</param>
        /// <param name="logicObject">logicObject</param>
        /// <param name="onAdded_callback">模型内children有added的回调函数</param>
        /// <param name="onRemoved_callback">模型内children有removed的回调函数</param> 
        public ModelEventObserver(
            IUANode model
            , IUAObject logicObject
            , Action<IUANode, IUANode, NodeId, ulong> onAdded_callback
            , Action<IUANode, IUANode, NodeId, ulong> onRemoved_callback

        )
        {
            var context = logicObject.Context;

            affinityId = context.AssignAffinityId();

            observer = new ModelObjectObserver(onAdded_callback, onRemoved_callback);

            eventRegistration = model.RegisterEventObserver(
                                 observer
                                , EventType.ForwardReferenceAdded | EventType.ForwardReferenceRemoved
                                , affinityId
                                );


           
        }


        public void Dispose()
        {
            eventRegistration?.Dispose();
            eventRegistration = null;

            observer = null;
        }
    }


    internal class ModelObjectObserver : IReferenceObserver
    {
        private Action<IUANode, IUANode, NodeId, ulong> _onAdded_callback;
        private Action<IUANode, IUANode, NodeId, ulong> _onRemoved_callback;
        public ModelObjectObserver(
                                    Action<IUANode, IUANode, NodeId, ulong> onAdded_callback
                                    , Action<IUANode, IUANode, NodeId, ulong> onRemoved_callback
                                    )
        {
            _onAdded_callback = onAdded_callback;
            _onRemoved_callback = onRemoved_callback;

        }


        public void OnReferenceAdded(IUANode sourceNode, IUANode targetNode, NodeId referenceTypeId, ulong senderId)
        {
            //Log.Info($"model added : sourceNode:{sourceNode.BrowseName}  targetNode:{targetNode.BrowseName}");
            if (_onAdded_callback != null)
            {
                _onAdded_callback.Invoke(sourceNode, targetNode, referenceTypeId, senderId);
            }
        }

        public void OnReferenceRemoved(IUANode sourceNode, IUANode targetNode, NodeId referenceTypeId, ulong senderId)
        {
            //Log.Info($"model removed : sourceNode:{sourceNode.BrowseName}  targetNode:{targetNode.BrowseName}");

            if (_onRemoved_callback != null)
            {
                _onRemoved_callback.Invoke(sourceNode, targetNode, referenceTypeId, senderId);
            }
        }
    }
}
