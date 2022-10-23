using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class AllConfigModel
    {
        public AllConfigModel(AuthConfig auth, UserConfig user)
        {
            AuthConfig = auth;
            UserConfig = user;
        }

        public AuthConfig AuthConfig { get; }

        public UserConfig UserConfig { get; }
    }
}
