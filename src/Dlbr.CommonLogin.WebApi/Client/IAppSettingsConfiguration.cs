using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public interface IAppSettingsConfiguration
    {
        string Prefix { get; }
    }
}
