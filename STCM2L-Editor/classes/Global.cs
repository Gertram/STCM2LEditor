using STCM2LEditor.classes.Actions.Parameters;
using System.Collections.Generic;
using STCM2LEditor.classes.Actions;
using System.Linq;
using System;
namespace STCM2LEditor.classes
{
    class Global
    {
        public Dictionary<uint, List<GlobalParameter>> ParameterCalls { get; set; }
        public Dictionary<uint, List<IAction>> ActionCalls { get; set; }
        public void RecoverGlobalCalls(ICollection<IAction> actions)
        {
            foreach (var item in ParameterCalls)
            {
                try
                {
                    var action = actions.First(x => x.Address == item.Key);
                    foreach (var par in item.Value)
                    {
                        par.GlobalPointer = action;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Found global call without action");
                }
            }
            ParameterCalls.Clear();
            foreach (var item in ActionCalls)
            {
                try
                {
                    var action = actions.First(x => x.Address == item.Key);
                    foreach (var par in item.Value)
                    {
                        par.LocalCall = action;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Found global call without action");
                }
            }
            ActionCalls.Clear();
        }
        internal Global()
        {
            ParameterCalls = new Dictionary<uint, List<GlobalParameter>>();
            ActionCalls = new Dictionary<uint, List<IAction>>();
        }
    }
}
