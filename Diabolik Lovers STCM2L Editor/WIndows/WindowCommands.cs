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
using System.IO;
using System.ComponentModel;
using System.Configuration;

namespace STCM2L
{
    public class WindowCommands
    {
        static WindowCommands()
        {
            Import = new RoutedCommand("Import", typeof(MainWindow));
            ImportFromXML = new RoutedCommand("ImportFromXML", typeof(MainWindow));
            ImportFromText = new RoutedCommand("ImportFromText", typeof(MainWindow));
            NewLine = new RoutedCommand("NewLine", typeof(MainWindow));
            Pack = new RoutedCommand("Pack", typeof(MainWindow));
            ActionsView = new RoutedCommand("ActionsView", typeof(MainWindow));
            TextView = new RoutedCommand("TextView", typeof(MainWindow));
            PlaceView = new RoutedCommand("PlaceView", typeof(MainWindow));
            NameView = new RoutedCommand("NameView", typeof(MainWindow));
            Find = new RoutedCommand("Find", typeof(MainWindow));
        }
        public static RoutedCommand Import { get; set; }
        public static RoutedCommand ImportFromXML { get; set; }
        public static RoutedCommand ImportFromText { get; set; }
        public static RoutedCommand ActionsView { get; set; }
        public static RoutedCommand NewLine  { get; set; }
        public static RoutedCommand PlaceView { get; set; }
        public static RoutedCommand NameView { get; set; }
        public static RoutedCommand TextView { get; set; }
        public static RoutedCommand Find { get; set; }
        public static RoutedCommand Pack { get; set; }
    }
}
