using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMessageSockets.Common.Messages
{
    public interface IMessage
    {
        Task<byte[]> SerializeAsync();

        Task DeserializeAsync(byte[] message);
    }
}
