protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 

XCOPY /Y Protocol.cs "../../../Server/Server/Packet"
XCOPY /Y Protocol.cs "../../../Server/Client/Packet"
XCOPY /Y Protocol.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../UnityServer/Assets/Scripts/Packet"

START ../../../Server/PacketGenerator/bin/Debug/net6.0/PacketGenerator.exe ./Protocol.proto

REM Login Server Packet Managements
XCOPY /Y LoginServerPacketManager.cs "../../../Server/Server/Packet"
REM XCOPY /Y ServerPacketHandler.cs "../../../Server/Server/Packet"

REM DummyClient Packet Managements
REM XCOPY /Y ClientPacketManager.cs "../../../Server/Client/Packet"
REM XCOPY /Y ClientPacketHandler.cs "../../../Server/Client/Packet"

REM UnityClient Packet Managments
XCOPY /Y ClientPacketManager.cs "../../../Client/Assets/Scripts/ServerCore"
REM XCOPY /Y ClientPacketHandler.cs "../../../Client/Assets/Scripts/Packet"

REM UnityServer Packet Managements
XCOPY /Y GameServerPacketManager.cs "../../../UnityServer/Assets/Scripts/Packet"