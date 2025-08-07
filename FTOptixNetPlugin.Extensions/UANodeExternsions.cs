using FTOptix.HMIProject;
using UAManagedCore;

namespace FTOptixNetPlugin.Extensions
{
    public static class UANodeExternsions
    {
        /// <summary>
        /// 注册 内元素 加入 和 移除的事件观察
        /// </summary>
        /// <param name="model">父模型</param>
        /// <param name="logicObject">脚本对象</param>
        /// <param name="onAdded_callback">加入事件响应</param>
        /// <param name="onRemoved_callback">移除事件响应</param>
        /// <returns></returns>
        public static ModelEventObserver RegisterAddAndRemoveObserver(this IUANode model
                                        , IUAObject logicObject
                                        , Action<IUANode, IUANode, NodeId, ulong> onAdded_callback
                                        , Action<IUANode, IUANode, NodeId, ulong> onRemoved_callback
                                        )
        {

        
            
            return new ModelEventObserver(model, logicObject, onAdded_callback, onRemoved_callback);



        }

        /// <summary>
        /// 获取对象的类型 NodeId
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static NodeId GetTypeNodeId(this IUANode node)
        {
            var t = node.GetType();
            var s = t.Assembly?.FullName?.Split(",").FirstOrDefault();

            var aaaa = $"{s}.ObjectTypes";
            var tObjectTypes = t.Assembly?.GetType(aaaa);
            if (tObjectTypes != null)
            {
                var fInfo = tObjectTypes.GetField(t.Name);
                var val = fInfo.GetValue(null);
                if (val != null)
                {
                    return val as NodeId;
                }

            }

            return null;
        }

        public static T GetVariableValue<T>(this IUANode node, string browseName, T defaultValue)
        {
            if (node == null)
            {
                return defaultValue;
            }
            else
            {
                var v = node.GetVariable(browseName);
                if (v == null)
                {
                    return defaultValue;
                }
                else
                {
                    return (T)v.Value.Value;
                }
            }
        }

        public static bool SetVariableValue<T>(this IUANode node, string browseName, T value)
        {
            if (node == null)
            {
                return false;
            }
            else
            {
                var v = node.GetVariable(browseName);
                if (v == null)
                {
                    return false;
                }
                else
                {
                    v.Value = new UAValue(value);
                    return true;
                }
            }
        }

        public static void ClearAll(this IUANode node)
        {
            foreach (var item in node.Children)
            {
                item.Delete();
            }
        }


        public static T GetVariableValue<T>(this IUANode node, string variable_name)
        {
            var v = node.GetVariable(variable_name);
            if (v != null)
            {
                return (T)v.Value.Value;
            }
            else
            {
                return default(T);
            }
        }



        /// <summary>
        /// 深度克隆
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <exception cref="Exception"></exception>
        public static void DeepClone(this IUANode source, IUANode target)
        {
            //var root = InformationModel.MakeObject(obj.BrowseName);
            foreach (var item in source.Children)
            {
                if (item.GetType() == typeof(IUAVariable))
                {
                    var v = InformationModel.MakeVariable(item.BrowseName, (item as IUAVariable).DataType);
                    v.Value = (item as IUAVariable)?.Value;
                    target.Add(v);
                }
                else if (item.GetType() == typeof(IUAObject))
                {
                    var o = InformationModel.MakeObject(item.BrowseName);
                    // DeepClone(item as IUAObject,o);
                    item.DeepClone(o);
                    target.Add(o);
                }
                else
                {
                    throw new Exception("不知道啥类型了");
                }
            }
        }

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        /// <exception cref="Exception"></exception>
        public static void DeepCopy(this IUANode obj, IUANode target)
        {
            //var root = InformationModel.MakeObject(obj.BrowseName);
            foreach (var item in obj.Children)
            {
                if (item.GetType() == typeof(UAVariable))
                {
                    var v = target.GetVariable(item.BrowseName);
                    if (v != null)
                    {
                        v.Value = (item as IUAVariable)?.Value;

                    }

                }
                else if (item.GetType() == typeof(UAObject))
                {
                    var node = target.GetObject(item.BrowseName);
                    if (node != null)
                    {

                        item.DeepCopy(node);
                    }

                }
                else
                {
                    throw new Exception("不知道啥类型了");
                }
            }
        }
    }
}
