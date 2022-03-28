using System.Windows.Input;

namespace STCM2LEditor
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
            GamePreset = new RoutedCommand("GamePreset", typeof(MainWindow));
            PlaceView = new RoutedCommand("PlaceView", typeof(MainWindow));
            NameView = new RoutedCommand("NameView", typeof(MainWindow));
            Find = new RoutedCommand("Find", typeof(MainWindow));
            Goto = new RoutedCommand("Goto", typeof(MainWindow));
        }
        public static RoutedCommand Import { get; set; }
        public static RoutedCommand Goto { get; set; }
        public static RoutedCommand ImportFromXML { get; set; }
        public static RoutedCommand ImportFromText { get; set; }
        public static RoutedCommand GamePreset { get; set; }
        public static RoutedCommand ActionsView { get; set; }
        public static RoutedCommand NewLine { get; set; }
        public static RoutedCommand PlaceView { get; set; }
        public static RoutedCommand NameView { get; set; }
        public static RoutedCommand TextView { get; set; }
        public static RoutedCommand Find { get; set; }
        public static RoutedCommand Pack { get; set; }
    }
}
