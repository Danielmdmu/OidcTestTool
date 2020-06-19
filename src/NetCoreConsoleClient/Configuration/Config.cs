using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreConsoleClient
{
    public class Config
    {
        public string ClientId { get; set; }
        public string IssuerUrl { get; set; }
        public string Scope { get; set; }
    }
}