using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader.Services
{
    public interface IAppVersion
    {
        string GetVersion();
        int GetBuild();
    }
}
