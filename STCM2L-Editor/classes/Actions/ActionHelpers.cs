using System;

namespace STCM2LEditor.classes.Actions
{
    internal static partial class ActionHelpers
    {
        public const int HEADER_LENGTH = sizeof(uint) * 4;
        public static uint ACTION_NAME = 0xd4;
        // public const uint ACTION_NEW_TEXT = 0x7a;
        public static UInt32 ACTION_TEXT = 0xd2;
        public static UInt32 ACTION_CHOICE = 0xe7;
        public static UInt32 ACTION_DIVIDER = 0xd3;
        public static UInt32 ACTION_NEW_PAGE = 0x1c1;
        public static UInt32 ACTION_PLACE = 0x79d0;
        public static UInt32 ACTION_SHOW_PLACE = 0x227c;
    }
}