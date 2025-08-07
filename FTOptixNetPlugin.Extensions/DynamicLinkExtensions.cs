using FTOptix.CoreBase;
using UAManagedCore;

namespace FTOptixNetPlugin.Extensions
{
    public static class DynamicLinkExtensions
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
    }
}
