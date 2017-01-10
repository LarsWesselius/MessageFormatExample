# Message Format Example
## Description
This is an example project to show how you could create a message format for use with TCP sockets in C#. It is intended to only show the message serialization and not anything else.

## Why do you need a message format?
When sending stuff through TCP you need to somehow specify a start and an end of a message. Most, somewhat older protocols like IRC simply rely on newlines (CRLF) to split messages. While this is very simple and effective, in large projects such as games it will become unmanageable. Therefore, it is useful to be able to convert the messages to objects on either end of the line.

## Structure
The idea is to map Types to ushort IDs. When writing a message, the ushort ID of the message to be sent is written first. Then the length of the message itself, and then the data of the message itself.

To visualize;

| 2 bytes ClassID | 4 bytes Length | Message Data |

The Message serializes itself. When reading, the class ID is read, the length is read and then the message. The Message is created based on the Type found in the mapping and then deserializes itself to create a complete POCO. This POCO is then returned and..something could happen with it.

### Disclaimer
This code does not have a lot of error checking and utility classes (like an automatic Serializer to serialize any type without needing to do it yourself. You could even use a BinaryFormatter for the last part of the message). A lot can be improved but this is intended as an example.
 
