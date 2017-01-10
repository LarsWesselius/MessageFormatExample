using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleMessageSockets.Common.Messages;
using ExampleMessageSockets.Common.Serialization;

namespace ExampleMessageSockets.Server
{
    public class ServerListener
    {
        private ClassMappingManager _classMappingManager;

        public ServerListener()
        {
            _classMappingManager = new ClassMappingManager();
            _classMappingManager.Initialize();
        }

        // Now, let's skip over all the connection stuff - any type of connections can be used, I'm just showing how to do the messages bit

        public async Task SendMessageAsync(IMessage message)
        {
            // First we have to serialize the message.
            byte[] messageBuffer = await message.SerializeAsync();

            // So our packet structure is as follows;
            /*
             * _________________________________________________________
             * |                  |               |                     |
             * | ushort (2 bytes) | int (4 bytes) |   (Length) bytes    |
             * |____Class ID______|____Length_____|_____Message data____|
             * 
             * That's why we serialize first - so we know what the message size is.
             * For the record 'Length' does not include the 6 bytes for the 'header'.
            */

            int fullPacketLength = 2 + 4 + messageBuffer.Length;
            byte[] fullPacket = new byte[fullPacketLength];

            byte[] classIdBytes = BitConverter.GetBytes(ClassMappingManager.MappingsInverse[message.GetType()]);
            Buffer.BlockCopy(classIdBytes, 0, fullPacket, 0, 2); // From classIdBytes (start at index 0) to fullPacket (start at index 0), and write 2 bytes only.

            byte[] lengthBytes = BitConverter.GetBytes(messageBuffer.Length);
            Buffer.BlockCopy(lengthBytes, 0, fullPacket, 2, 4); // Again, but this time start at 2 (we've already written byte 0 and 1) and write 4 (int!).

            // Message itself.
            Buffer.BlockCopy(messageBuffer, 0, fullPacket, 4, messageBuffer.Length);

            // Our message is complete now. Write it to our stream..I've just mocked a stream here
            Stream stream = new MemoryStream(); // My network stream for a client, whatever it might be.

            // For simplicity we'll just write the entire packet in one Write call but I'd probably split it up into chunks either by creating a MemoryStream from the buffer
            //  or using a faster for loop with does something like for (int i = 0; i != (fullPacketLength % 1024) + 1; ++i) etcetera.
            await stream.WriteAsync(fullPacket, 0, fullPacketLength);
        }

        public async Task<IMessage> ReadMessageAsync(Stream clientStream)
        {
            // This'd probably be called from a loop (if bytes available, call this method with the stream). Or asynchronous from BeginRead.

            // Obviously we'd have to do some error checking in between these calls but the idea is there.
            byte[] classIdBuffer = new byte[2];
            await clientStream.ReadAsync(classIdBuffer, 0, 2);
            ushort classId = BitConverter.ToUInt16(classIdBuffer, 0);

            byte[] lengthBuffer = new byte[4];
            await clientStream.ReadAsync(lengthBuffer, 0, 4);
            int length = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] messageBuffer = new byte[length];
            await clientStream.ReadAsync(messageBuffer, 0, length);

            if (!ClassMappingManager.Mappings.ContainsKey(classId))
            {
                // We have a message we know nothing about. Discard with log message or something, as there is a mismatch between the client and server. Maybe even
                //  bail completely as now our entire message/class mapping cannot be trusted.
                return null;
            }

            Type messageType = ClassMappingManager.Mappings[classId];

            // Now, we use Activator here to create an instance of our class. It is certainly a bit slower than manually 'new'ing classes, but it gives you a lot
            //  of convenience just having to deal with IMessages.
            IMessage message = (IMessage)Activator.CreateInstance(messageType);
            await message.DeserializeAsync(messageBuffer);


            // You might be wondering what to do with the message after returning it - after all, you don't know which type it is yet as you just have the IMessage.
            // I'd probably have something like MessageHandlers which register to one or multiple ushort (class Ids) and call them if a class of such type was
            //  read.
            return message;
        }

    }
}
