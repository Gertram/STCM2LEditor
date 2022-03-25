using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.ComponentModel;
using System.Configuration;

using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor
{
    public class TextTranslate
    {
        public TranslateData Name { get; set; } = new NullTranslateData();
        public BindingList<TranslateData> Lines { get; set; } = new BindingList<TranslateData>();
        public void AddLine(STCM2L file, int index = -1)
        {
            var translateData = new StringData("");
            var originalData = new StringData("");
            var proxy = new ProxyData(originalData, translateData);
            var action = new DefaultAction(ActionHelpers.ACTION_TEXT, 0, 1);
            action.Parameters[0] = new LocalParameter(proxy);
            var actionProxy = new ActionProxy { Action = action,Proxy=proxy };
            var translate = new TranslateData(translateData, new List<ActionProxy> { actionProxy });
            file.Translates.Add(translate);
            var ind = file.Actions.IndexOf(Lines.Last().Actions[0].Action);
            file.Actions.Insert(ind+1,action);
            if(index == -1)
            {
                index = Lines.Count;
            }
            Lines.Insert(index, translate);
        }
        public void DeleteLine(STCM2L file, int index = -1)
        {
            if (index == -1)
            {
                index = Lines.Count-1;
            }
            var translate = Lines[index];
            foreach (var item in translate.Actions)
            {
                file.Actions.Remove(item.Action);
                file.Translates.Remove(translate);
            }
            Lines.RemoveAt(index);
        }
        public TextTranslate Insert(STCM2L file,bool before = true)
        {
            var actions = file.Actions;
            bool newPage = false;
            TranslateData name = Name; ;

            if (name is NullTranslateData)
            {
                string messageBoxCaption = "New page";
                string messageBoxText = "Do you want to create a new page?";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage image = MessageBoxImage.Question;

                MessageBoxResult result = MessageBox.Show(messageBoxText, messageBoxCaption, button, image);

                newPage = result == MessageBoxResult.Yes;
            }

            var translate = new TextTranslate
            {
                Name = name
            };
            int actionInd = 0;
            if (before)
            {
                if (name is NullTranslateData)
                {
                    actionInd = actions.IndexOf(Lines[0].Actions[0].Action);
                    if (newPage)
                    {
                        actions.Insert(actionInd, new DefaultAction(ActionHelpers.ACTION_NEW_PAGE, 0,  0));
                        actionInd++;
                        actions.Insert(actionInd, new DefaultAction(ActionHelpers.ACTION_DIVIDER, 0, 0));
                        actionInd++;
                    }
                }
                else
                {
                    actionInd = actions.IndexOf(Name.Actions[0].Action);
                    var action = new DefaultAction(ActionHelpers.ACTION_NAME, 0, 1);
                    var original = new StringData(name.OriginalText);
                    var proxy = new ProxyData(original, name.Data);
                    name.Actions.Add(new ActionProxy { Action = action, Proxy = proxy });
                    action.Parameters[0] = new LocalParameter(proxy);
                    actions.Insert(actionInd, action);
                    actionInd++;
                }

                actions.Insert(actionInd, new DefaultAction(ActionHelpers.ACTION_DIVIDER,0, 0));
                //actionInd++;
            }
            else
            {
                actionInd = actions.IndexOf(Lines.Last().Actions[0].Action) + 1;

                actions.Insert(actionInd, new DefaultAction(ActionHelpers.ACTION_DIVIDER,0, 0));
                actionInd++;
                if (name is NullTranslateData)
                {
                    if (newPage)
                    {
                        actions.Insert(actionInd, new DefaultAction(ActionHelpers.ACTION_NEW_PAGE,0, 0));
                        actionInd++;
                        actions.Insert(actionInd, new DefaultAction(ActionHelpers.ACTION_DIVIDER,0, 0));
                        actionInd++;
                    }
                }
                else
                {
                    var action = new DefaultAction(ActionHelpers.ACTION_NAME, 0, 1);
                    var original = new StringData(name.OriginalText);
                    var proxy = new ProxyData(original, name.Data);
                    name.Actions.Add(new ActionProxy { Action = action, Proxy = proxy });
                    action.Parameters[0] = new LocalParameter(proxy);
                    actions.Insert(actionInd, action);
                    actionInd++;
                }
            }
            var _action = new DefaultAction(ActionHelpers.ACTION_TEXT, 0,  1);
            var data = new StringData("");
            _action.Parameters[0] = new LocalParameter(data);
            actions.Insert(actionInd, _action);
            actionInd++;
            var translateData = MakeTranslate(file,_action, true);
            translate.Lines.Add(translateData);
            return translate;
        }
        private TranslateData MakeTranslate(STCM2L file,DefaultAction action, bool insert = false)
        {
            foreach (var translate in file.Translates)
            {
                foreach (var _action in translate.Actions)
                {
                    if (_action.Action == action)
                    {
                        return translate;
                    }
                }
            }
            var param = action.Parameters[0] as LocalParameter;
            var data2 = new StringData("");
            var data = new ProxyData(new StringData(param.ParameterData), data2);
            param.ParameterData = data;
            var _actions = new List<ActionProxy>
            {
                new ActionProxy { Action = action, Proxy = data }
            };
            var _translate = new TranslateData(data2, _actions);
            if (insert)
                file.Translates.Add(_translate);
            return _translate;
        }
        public void DeleteFrom(STCM2L file)
        {
            if(!(Name is NullTranslateData))
            {
                var ind = file.Actions.IndexOf(Lines.First().Actions[0].Action);
                var nameAction = file.Actions[ind-1];
                Name.Actions.Remove(Name.Actions.First(x => x.Action == nameAction));
                file.Actions.Remove(nameAction);
                if(Name.Actions.Count == 0)
                    file.Translates.Remove(Name);
            }
            var index = file.Actions.IndexOf(Lines.Last().Actions[0].Action);
            var action = file.Actions[index+1];
            if(action.OpCode == ActionHelpers.ACTION_DIVIDER)
            {
                file.Actions.Remove(action);
            }
            foreach (var line in Lines)
            {
                line.DeleteFrom(file);
            }
        }
    }
}
