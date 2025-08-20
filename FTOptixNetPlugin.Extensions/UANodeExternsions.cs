using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using FTOptix.OPCUAClient;
using FTOptix.UI;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using UAManagedCore;
using OpcUA = UAManagedCore.OpcUa;

namespace FTOptixNetPlugin.Extensions
{
    public static class UANodeExternsions
    {

        #region Public Methods
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

        public static T GetVariableValue<T>(this IUANode node, string variable_name, T defaultValue)
        {
            if (node == null)
            {
                return defaultValue;
            }
            else
            {
                var v = node.GetVariable(variable_name);
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

        public static bool SetVariableValue<T>(this IUANode node, string variable_name, T value)
        {
            if (node == null)
            {
                return false;
            }
            else
            {
                var v = node.GetVariable(variable_name);
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

        public static string GetCurrentProjectBrowsePath(this IUANode node)
        {
            if (node.Owner == Project.Current || node.BrowseName == "Root")
                return node.BrowseName;
            return GetCurrentProjectBrowsePath(node.Owner) + "/" + node.BrowseName;
        }



        public static bool IsInStartedSubtree(this IUANode node)
        {
            if (node.Status == NodeStatus.Detached)
            {
                return false;
            }
            while (node != null)
            {
                if (node.Status == NodeStatus.Started)
                {
                    return true;
                }
                node = node.Owner;
            }
            return false;
        }


        /// <summary>
        /// UANode to Json
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string JsonSerialize(this IUANode node)
        {
            var dc = new Dictionary<string, object>();
            if(node is IUAVariable)
            {
                dc.Add(node.BrowseName, InternalVariableJsonSerialize((IUAVariable)node));
            }
            else
            {

                InternalJsonSerialize(node, ref dc);
            }

            return System.Text.Json.JsonSerializer.Serialize(dc);
        }


        /// <summary>
        /// Json Deserial to UANode
        /// </summary>
        /// <param name="node"></param>
        /// <param name="browseName"></param>
        /// <param name="jsonContent"></param>
        public static void JsonDeserialize(this IUANode node,string browseName,string jsonContent)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Dynamic.ExpandoObject>(jsonContent);

            if(data == null)
            {
                return;
            }

            var child = InformationModel.MakeObject(browseName);
            node.Add(child);

            foreach (var item in data)
            {
                if (item.Value is JsonElement)
                {
                   
                    var el = (JsonElement)item.Value;


                    InternalJsonDeserialize(item.Key, el, child);

                    

                }
            }


        }


#endregion

        #region Private Methods

        private static void InternalJsonSerialize(IUANode node,ref Dictionary<string,object> dc)
        {

            foreach (var child in node.Children)
            {
                if (child is IUAVariable)
                {
                    var vv = child as IUAVariable;
                    if(vv == null)
                    {
                        dc.Add(child.BrowseName, null);
                    }
                    else
                    {
                        dc.Add(child.BrowseName, InternalVariableJsonSerialize(vv));

                    }

                }
                else if (child is Alias)
                {
                    var _node = node.GetAlias(child.BrowseName);

                    if (_node != null)
                    {
                        if(_node is IUAVariable)
                        {
                            dc.Add(child.BrowseName, InternalVariableJsonSerialize((IUAVariable)_node));
                        }
                        else
                        {
                            var _dc = new Dictionary<string, object>();
                            dc.Add(child.BrowseName, _dc);
                            InternalJsonSerialize(_node, ref _dc);
                        }
                    }
                    else
                    {
                        dc.Add(child.BrowseName, null);
                    }
                }
                else
                {
                    var _dc = new Dictionary<string, object>();
                    dc.Add(child.BrowseName, _dc);
                    InternalJsonSerialize(child, ref _dc);
                }
            }
            

        }

        private static object InternalVariableJsonSerialize(IUAVariable v)
        {
            return v.Value.Value;
        }



        private static void InternalJsonDeserialize(string name,JsonElement el, IUANode root)
        {

            switch (el.ValueKind)
            {
                case JsonValueKind.True:
                case JsonValueKind.False:
                    var bv = InformationModel.MakeVariable(name, OpcUA.DataTypes.Boolean);
                    bv.Value = el.GetBoolean();
                    root.Add(bv);
                    break;
                case JsonValueKind.String:
                    var sv = InformationModel.MakeVariable(name, OpcUA.DataTypes.String);
                    sv.Value = el.GetString();
                    root.Add(sv);
                    break;
                case JsonValueKind.Number:
                    var nv = InformationModel.MakeVariable(name, OpcUA.DataTypes.Float);
                    nv.Value = el.GetSingle();
                    root.Add(nv);
                    break;
                case JsonValueKind.Object:
                    var ov = InformationModel.MakeObject(name);
                    root.Add(ov);
                    foreach(var item in el.EnumerateObject())
                    {
                        InternalJsonDeserialize(item.Name, item.Value, ov);
                    }
                    break;
                case JsonValueKind.Array:
                    var fel = el.EnumerateArray().FirstOrDefault();
                    NodeId datatype = NodeId.Empty;
                    switch(fel.ValueKind)
                    {
                        case JsonValueKind.String:
                            datatype = OpcUA.DataTypes.String;
                            break;
                        case JsonValueKind.Number:
                            datatype = OpcUA.DataTypes.Float;
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            datatype = OpcUA.DataTypes.Boolean;
                            break;
                        default:
                            return;
                    }

                    var av = InformationModel.MakeVariable(name, datatype, new uint[] { (uint)el.GetArrayLength() });
                    var values = el.EnumerateArray().Select(e => GetJsonValue(e)).ToArray();

                    av.Value = new UAValue(values);
                    root.Add(av);
                    break;
                default:
                    break;
            }
        }

        private static object GetJsonValue(JsonElement el)
        {

            switch (el.ValueKind)
            {
                case JsonValueKind.True:
                case JsonValueKind.False:
                  
                    return el.GetBoolean();
                    
                    
                case JsonValueKind.String:
                    
                    return el.GetString();
                    
                    
                case JsonValueKind.Number:
                    
                    return el.GetSingle();
                default:
                    return null;
            }
        }
        #endregion
    }
}
