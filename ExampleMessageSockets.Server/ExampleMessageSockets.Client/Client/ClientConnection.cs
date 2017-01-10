using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleMessageSockets.Common.Serialization;

namespace ExampleMessageSockets.Client
{
    public class ClientConnection
    {
        private ClassMappingManager _classMappingManager;

        public ClientConnection()
        {
            _classMappingManager = new ClassMappingManager();
            _classMappingManager.Initialize();
        }
    }
}
