using FTOptix.CoreBase;
using FTOptix.HMIProject;
using FTOptix.UI;
using UAManagedCore;

namespace FTOptixNetPlugin.Extensions
{
    public static class UAVariableExtensions
    {
        public static void AddDynamicLinkToVariableBit(this IUAVariable targetVariable , IUAVariable sourceVariable,uint bitIndex, DynamicLinkMode mode = DynamicLinkMode.Read)
        {
            targetVariable.SetDynamicLink(sourceVariable, mode);
            targetVariable.GetVariable("DynamicLink").Value += $".{bitIndex}";

        }

        public static void AddDynamicLinkToArrayElement(this IUAVariable targetVariable , IUAVariable sourceVariable,uint arrayIndex, DynamicLinkMode mode = DynamicLinkMode.Read)
        {
            targetVariable.SetDynamicLink(sourceVariable, arrayIndex, mode);

        }


        public static bool HasDynamicLink(this IUAVariable v)
        {
            if (v.Refs.GetNode(FTOptix.CoreBase.ReferenceTypes.HasDynamicLink) != null)
            {

                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool HasConverter(this IUAVariable v)
        {
            if (v.Refs.GetNode(FTOptix.CoreBase.ReferenceTypes.HasDynamicLink) != null)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
