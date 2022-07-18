protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 

XCOPY /Y Protocol.cs "../../../Server/Server/Packet"
XCOPY /Y Protocol.cs "../../../Server/Client/Packet"
XCOPY /Y Protocol.cs "../../../Client/Assets/Scripts/Packet"

START ../../../Server/PacketGenerator/bin/Debug/net6.0/PacketGenerator.exe ./Protocol.proto

XCOPY /Y ServerPacketManager.cs "../../../Server/Server/Packet"
REM XCOPY /Y ServerPacketHandler.cs "../../../Server/Server/Packet"

XCOPY /Y ClientPacketManager.cs "../../../Server/Client/Packet"
REM XCOPY /Y ClientPacketHandler.cs "../../../Server/Client/Packet"
XCOPY /Y ClientPacketManager.cs "../../../Client/Assets/Scripts/Managers/Core"
REM XCOPY /Y ClientPacketHandler.cs "../../../Client/Assets/Scripts/Packet"