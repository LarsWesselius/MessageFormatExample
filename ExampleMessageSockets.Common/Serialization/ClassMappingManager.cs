using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleMessageSockets.Common.Messages;

namespace ExampleMessageSockets.Common.Serialization
{
    public class ClassMappingManager
    {
        public static ConcurrentDictionary<ushort, Type> Mappings = new ConcurrentDictionary<ushort, Type>();

        // We do this purely for speed. Might be better ways to do something like this.
        public static ConcurrentDictionary<Type, ushort> MappingsInverse = new ConcurrentDictionary<Type, ushort>();

        private ushort _seed = 0; // ushort so we have more range but still only 2 bytes

        public void Initialize()
        {
            // You can either discover using reflection (it's fine, it's only once on startup!)
            // Or hardcode. I'd recommend discovering using reflection

            // Finds all classes that implement IMessage in ANY assembly.
            var type = typeof(IMessage);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && type.IsAssignableFrom(p));

            // We just assign a number to each message. We order by name because the client has to do the same thing
            //  and it's imperative the order in which messages are added is the same (otherwise the class ID won't be correct).
            foreach (var messageType in types.OrderBy(p => p.Name))
            {
                Mappings[_seed] = messageType;
                MappingsInverse[messageType] = _seed;

                ++_seed;
            }
        }
    }
}
