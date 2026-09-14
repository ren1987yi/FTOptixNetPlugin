using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService
{
    internal class ControlActionDelegate
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Func<string,object> Func { get; set; }
    }



    public class ControlActionDelegateManager
    {
        private List<ControlActionDelegate> delegates = new List<ControlActionDelegate>();
        public void Register(string controllerName, string actionName, Func<string, object> func)
        {
            var existing = delegates.FirstOrDefault(d => d.ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase) && d.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Func = func; // Update the existing delegate
            }
            else
            {
                delegates.Add(new ControlActionDelegate
                {
                    ControllerName = controllerName,
                    ActionName = actionName,
                    Func = func
                });
            }
        }
        public Func<string, object> GetDelegate(string controllerName, string actionName)
        {
            var del = delegates.FirstOrDefault(d => d.ControllerName.Equals(controllerName,StringComparison.OrdinalIgnoreCase) && d.ActionName.Equals(actionName,StringComparison.OrdinalIgnoreCase));
            return del?.Func;
        }
    }
}
