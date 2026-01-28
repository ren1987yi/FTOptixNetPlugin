using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
namespace FTOptixNetPlugin.Extensions
{

    public class EvaluatorHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destVariable">目标变量</param>
        /// <param name="expression">表达式</param>
        /// <param name="srcVariables">源变量组</param>
        public static void CreateExpressionEvaluator(IUAVariable destVariable, string expression, IEnumerable<IUAVariable> srcVariables)
        {
            var exp = InformationModel.MakeObject<ExpressionEvaluator>("ExpressionEvaluator");

            exp.Expression = expression;
            var sources = new IUAVariable[srcVariables.Count()];

            var i = 0;
            foreach (var srcVar in srcVariables)
            {
                var s = InformationModel.MakeVariable($"Source{i}", OpcUa.DataTypes.BaseDataType);
                s.SetDynamicLink(srcVar);

                exp.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, s);
                i++;
            }



            destVariable.SetConverter(exp);

        }




        public static void CreateValueMapConverter(IUAVariable targetNode, IUAVariable sourceVariable, NodeId keyDataType, NodeId valueDataType, Tuple<object, object>[] keyValues)
        {
            ValueMapConverter newValueMapConverter = InformationModel.MakeObject<ValueMapConverter>("KeyValueConverter1", FTOptix.CoreBase.ObjectTypes.ValueMapConverter);
            // Create the pairs object
            IUAObject newPairs = InformationModel.MakeObject(new QualifiedName(FTOptix.CoreBase.ObjectTypes.ValueMapConverter.NamespaceIndex, "Pairs"));
            // For each pair, set properties
            for (int i = 0; i < keyValues.Length; i++)
            {
                var item = keyValues[i];
                // First element is the default one
                string pairBrowseName = "Pair";
                if (i > 0)
                    pairBrowseName += i.ToString();
                // Create the new pair
                IUAObject newPair = InformationModel.MakeObject(pairBrowseName, FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                IUAVariable newKey = newPair.GetVariable("Key");
                IUAVariable newValue = newPair.GetVariable("Value");
                // Set the proper datatype
                newKey.DataType = keyDataType;
                newValue.DataType = valueDataType;
                // Set the proper value
                newKey.Value = new UAValue(item.Item1);
                newValue.Value = new UAValue(item.Item2);
                // Add the pair to the map
                newPairs.Add(newPair);
            }
            // Add the map to the key value converter
            newValueMapConverter.Add(newPairs);
            newValueMapConverter.SourceVariable.SetDynamicLink(sourceVariable);
            // Add the key value converter to the variable
            targetNode.SetConverter(newValueMapConverter);
        }




        public static ValueMapConverterType CreateValueMapConverterType(IUANode parentNode, string name, NodeId keyDataType, NodeId valueDataType, Tuple<object, object>[] keyValues)
        {
            ValueMapConverterType newValueMapConverter = InformationModel.MakeObjectType<ValueMapConverterType>(name);
            // Create the pairs object
            IUAObject newPairs = InformationModel.MakeObject(new QualifiedName(FTOptix.CoreBase.ObjectTypes.ValueMapConverter.NamespaceIndex, "Pairs"));
            // For each pair, set properties
            for (int i = 0; i < keyValues.Length; i++)
            {
                var item = keyValues[i];
                // First element is the default one
                string pairBrowseName = "Pair";
                if (i > 0)
                    pairBrowseName += i.ToString();
                // Create the new pair
                IUAObject newPair = InformationModel.MakeObject(pairBrowseName, FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                IUAVariable newKey = newPair.GetVariable("Key");
                IUAVariable newValue = newPair.GetVariable("Value");
                // Set the proper datatype
                newKey.DataType = keyDataType;
                newValue.DataType = valueDataType;
                // Set the proper value
                newKey.Value = new UAValue(item.Item1);
                newValue.Value = new UAValue(item.Item2);
                // Add the pair to the map
                newPairs.Add(newPair);
            }
            // Add the map to the key value converter
            newValueMapConverter.Add(newPairs);
            parentNode.Add(newValueMapConverter);


            return newValueMapConverter;
        }

        public static void SetValueMapConverter(IUAVariable targetNode, IUAVariable sourceVariable, IUANode valueMapConverter)
        {
            var vmConverter = InformationModel.MakeObject("ValueMapConverter1", valueMapConverter.NodeId) as ValueMapConverter;

            if (vmConverter != null)
            {

                vmConverter.SourceVariable.SetDynamicLink(sourceVariable);
                targetNode.SetConverter(vmConverter);
            }
        }


        public static void CreateConditionConverter(IUAVariable targetNode, IUAVariable sourceVariable, NodeId dataTypeId, UAValue trueValue, UAValue falseValue)
        {
            // Create the conditional converter
            ConditionalConverter newConditionalConverter = InformationModel.MakeObject<ConditionalConverter>("ConditionalConverter1", FTOptix.CoreBase.ObjectTypes.ConditionalConverter);
            // Set the "false" condition
            newConditionalConverter.FalseValueVariable.DataType = dataTypeId;
            newConditionalConverter.FalseValueVariable.Value = falseValue;
            // Set the "true" condition
            newConditionalConverter.TrueValueVariable.DataType = dataTypeId;
            newConditionalConverter.TrueValueVariable.Value = trueValue;
            // Add the source variable
            newConditionalConverter.ConditionVariable.SetDynamicLink(sourceVariable);
            // Set the dynamic link to the object
            newConditionalConverter.Mode = DynamicLinkMode.Read;
            targetNode.SetConverter(newConditionalConverter);
        }

    }

}
