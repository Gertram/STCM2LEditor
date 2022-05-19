using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2LEditor.classes
{
    public interface IGameEncoding
    {
        Encoding Encoding { get; }
    }
    public interface IGameSettings:IGameEncoding
    {
        uint ACTION_NAME { get; }
        uint ACTION_TEXT { get; }
        uint ACTION_CHOICE { get; }
        uint ACTION_DIVIDER { get; }
        uint ACTION_NEW_PAGE { get; }
        uint ACTION_PLACE { get; }
        uint ACTION_SHOW_PLACE { get; }
    }
}
