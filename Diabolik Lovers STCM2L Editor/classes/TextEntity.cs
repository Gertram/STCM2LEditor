using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Linq.Expressions;
using System.Xml;
using Microsoft.Win32;
using System.Xml.Linq;
using System.ComponentModel;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    public class TextEntity
    {
        public class MyString
        {
            public string Text { get; set; }
        }
        public TextEntity()
        {
        }

        public TextEntity(string name, BindingList<MyString> lines)
        {
            Name = name;
            Lines = lines;
        }
        public void AddLine(string text = "",int index = -1)
        {
            if (index == -1)
            {
                Lines.Add(new MyString { Text = text });
                return;
            }
            Lines.Insert(index, new MyString { Text = text });
        }
        public void DeleteLine(int index = -1)
        {
            if (index == -1)
            {
                Lines.RemoveAt(Lines.Count - 1);
                return;
            }
            Lines.RemoveAt(index);
        }

        public string Name { get; set; } = "";
        public BindingList<MyString> Lines { get; set; } = new BindingList<MyString>();
        /*
                public TextEntity(List<ImDefaultAction> actions, int actionsEnd, string name, bool newPage, bool before) {
                    Actions = actions;
                    ActionsEnd = actionsEnd;
                    AmountInserted = 0;

                    Actions.Insert(ActionsEnd, new DefaultAction(0, ActionHelpers.ACTION_DIVIDER, 0));
                    AmountInserted++;

                    if (name != null) {
                        Name = new Line(name);
                        NameAction = new DefaultAction(0, ActionHelpers.ACTION_NAME, 1);
                        Actions.Insert(ActionsEnd + AmountInserted, NameAction);
                        AmountInserted++;
                    }
                    else {
                        if (newPage) {
                            Actions.Insert(ActionsEnd + AmountInserted, new DefaultAction(0, ActionHelpers.ACTION_NEW_PAGE, 0));
                            AmountInserted++;
                        }

                        Actions.Insert(ActionsEnd + AmountInserted, new DefaultAction(0, ActionHelpers.ACTION_DIVIDER, 0));
                        AmountInserted++;
                    }

                    Lines = new ObservableCollection<Line>();
                    LineActions = new List<ImDefaultAction>();
                    PlaceActions = new List<ImDefaultAction>();
                    AddLine(true);

                    if (before) {
                        Actions.Insert(ActionsEnd + AmountInserted, new DefaultAction(0, ActionHelpers.ACTION_DIVIDER, 0));
                        AmountInserted++;
                    }

                    IsAnswer = false;
                    IsHighlighted = false;
                }*/
        /*
                public void SetConversation(ref int i, List<GeneralAction> actions) {
                    Actions = actions;

                    UInt32 opCode = actions[i].OpCode;
                    OldAddress = actions[i].OldAddress;

                    while (opCode == GeneralAction.ACTION_NAME || opCode == GeneralAction.ACTION_TEXT || opCode == GeneralAction.ACTION_PLACE) {
                        if (opCode == GeneralAction.ACTION_NAME) {
                            if (Name == null) {
                                Name = new Line(actions[i].GetStringFromParameter(0));
                                NameAction = actions[i];
                            }
                        }*//*
                        else if (opCode == Action.ACTION_PLACE)
                        {
                            string temp = actions[i].GetStringFromParameter(3);

                            Line line = new Line(temp);

                            Lines.Add(line);
                            PlaceActions.Add(actions[i]);
                        }*//*
                        else if(opCode == GeneralAction.ACTION_TEXT){
                            string temp = actions[i].GetStringFromParameter(0);

                            Line line = new Line(temp);

                            Lines.Add(line);
                            LineActions.Add(actions[i]);
                        }
                        i++;
                        ActionsEnd = i;
                        if (i >= actions.Count) {
                            break;
                        }

                        opCode = actions[i].OpCode;
                    }

                    i--;
                }

                public void SetAnswer(ref int i, List<GeneralAction> actions) {
                    string temp = actions[i].GetStringFromParameter(0);
                    Line line = new Line(temp);

                    Lines.Add(line);
                    LineActions.Add(actions[i]);
                    IsAnswer = true;
                }*/
        /*
                public void ResetText() {
                    ResetName();

                    foreach(Line line in Lines) {
                        line.Reset();
                    }
                }

                public void ResetName() {
                    if(Name != null) {
                        Name.Reset();
                    }
                }*/
        /*
                public void ReinsertLines () {
                    if(Name != null) {
                        NameAction.SetString(Name.LineText, 0);
                    }

                    if (PlaceActions.Count > 0) {
                        for (int i = 0; i < Lines.Count; i++) {
                            PlaceActions[i].SetString(Lines[i].LineText, 3);
                        }
                    }
                    else {
                        for (int i = 0; i < Lines.Count; i++) {
                            LineActions[i].SetString(Lines[i].LineText, 0);
                        }
                    }
                }*/
        /*
                public void AddLine(bool isNew = false, int index = -1) {
                    if (!IsAnswer && ((NameAction == null && Lines.Count < 10 )|| Lines.Count < 3)) {
                        var action = new DefaultAction(0, ActionHelpers.ACTION_TEXT, 1);
                        Line line = new Line("");

                        if(index == -1 || index == Lines.Count) {
                            Lines.Add(line);
                            LineActions.Add(action);
                            Actions.Insert(ActionsEnd + (isNew ? AmountInserted : 0), action);
                        }
                        else {
                            Lines.Insert(index, line);
                            LineActions.Insert(index, action);
                            Actions.Insert(ActionsEnd - (index == 0 ? 1 : index), action);
                        }

                        AmountInserted++;
                    }
                }*/
        /*
                public void DeleteLine(int index) {
                    Lines.Remove(Lines[index]);
                    Actions.Remove(LineActions[index]);
                    LineActions.Remove(LineActions[index]);
                }

                public void DeleteText() {
                    for(int i = 1; i <= AmountInserted; i++) {
                        Actions.Remove(Actions[ActionsEnd - i]);
                    }
                }*/
    }
}
