using SNPM.Core.Interfaces;
using System;


namespace SNPM.Core
{
    internal class ServerConnection : IServerConnection
    {
        public string servAddr { get { return "dupa"; } set => throw new NotImplementedException(); }
    }
}