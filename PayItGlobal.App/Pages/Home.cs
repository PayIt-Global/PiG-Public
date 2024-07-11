using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayItGlobal.App.Models;
using PayItGlobal.App.Resources.Styles;
using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Compatibility;
using Microsoft.Maui.Devices;

namespace PayItGlobal.App.Pages;
class Home : Component
{
    public override VisualNode Render()
    {
        return new Text("Home");
    }
}
