using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages.Components
{
    public interface IMainState
    {
        PageEnum CurrentPage { get; set; }
        bool IsAuthenticated { get; set; }
        bool Loading { get; set; }
        bool LoadedDirectly { get; set; }
    }
}
