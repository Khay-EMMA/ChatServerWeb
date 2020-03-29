using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerWeb.Api.Infrastructure.Configuration
{
    public class JWTConfig
    {
        public string ServerSecret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ExpiresIn { get; set; }
    }

}
