using STCM2LEditor.classes.Action.Parameters;
using System.Collections.Generic;
using STCM2LEditor.classes.Action;
using System.Linq;
using System;
namespace STCM2LEditor.classes
{
    static class Global
    {
        public static Dictionary<uint, List<GlobalParameter>> ParameterCalls { get; set; }
        public static Dictionary<uint, List<IAction>> ActionCalls { get; set; }
        public static void RecoverGlobalCalls(ICollection<IAction> actions)
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
            Global.ParameterCalls.Clear();
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
            Global.ActionCalls.Clear();
        }
        static Global()
        {
            ParameterCalls = new Dictionary<uint, List<GlobalParameter>>();
            ActionCalls = new Dictionary<uint, List<IAction>>();
        }
    }
}
