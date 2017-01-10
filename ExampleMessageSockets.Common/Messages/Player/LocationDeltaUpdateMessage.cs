using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMessageSockets.Common.Messages.Player
{
    public class LocationDeltaUpdateMessage : IMessage
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int PlayerID { get; set; }

        public async Task<byte[]> SerializeAsync()
        {
            // Now obviously you could make a generic serializer that simply takes an IMessage and automatically tries to serialize.
            // This would also be the way to do member classes but it's beyond the scope of this example.
            byte[] messageBuffer = new byte[8 + 8 + 8 + 4]; // 3 floats, one int

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, messageBuffer, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, messageBuffer, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, messageBuffer, 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(PlayerID), 0, messageBuffer, 24, 4);

            return messageBuffer;
        }

        public async Task DeserializeAsync(byte[] message)
        {
            X = BitConverter.ToSingle(message, 0);
            Y = BitConverter.ToSingle(message, 8);
            Z = BitConverter.ToSingle(message, 16);
            PlayerID = BitConverter.ToInt32(message, 24);
        }
    }
}
