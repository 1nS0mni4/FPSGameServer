using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator {
    public static class PacketFormat {

        #region PacketManager Formats
        /// <summary>
        /// {0} RegisterFormat
        /// </summary>
        public static string managerFormat =
@"using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

public class PacketManager {{
    #region Singleton
    public static PacketManager Instance {{ get; private set; }} = new PacketManager();
    #endregion
    public PacketManager() {{
        Register();
    }}

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _makeFunc = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    private Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<PacketSession, IMessage, ushort > CustomHandler {{ get; set; }} = null;

    private void Register() {{
{0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> segment) {{
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        ushort msgID = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if(_makeFunc.TryGetValue(msgID, out action)) {{
            action.Invoke(session, segment, msgID);
        }}
    }}

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> segment, ushort msgID) where T : IMessage, new() {{
        T pkt = new T();
        pkt.MergeFrom(segment.Array, segment.Offset + 4, segment.Count - 4);

        if(CustomHandler != null) {{
            CustomHandler.Invoke(session, pkt, msgID);
        }}
        else {{
            Action<PacketSession, IMessage> action = null;
            if(_handler.TryGetValue(msgID, out action))
                action.Invoke(session, pkt);
        }}
    }}

    public Action<PacketSession, IMessage> GetPacketHandler(ushort ID) {{
        Action<PacketSession, IMessage> action = null;

        if(_handler.TryGetValue(ID, out action))
            return action;
        return null;
    }}
}}";
        /// <summary>
        /// {0}: MsgID Without Underlines
        /// {1}: MsgID with Underlines
        /// </summary>
        public static string managerRegisterFormat =
@"      _makeFunc.Add((ushort)PacketID.{0}, MakePacket<{1}>);
    _handler.Add((ushort)PacketID.{0}, PacketHandler.{1}Handler);
";
        #endregion

        #region PacketHandler Formats
        /// <summary>
        /// {0} Handlers
        /// {1} UserType (Client | Server)
        /// </summary>
        public static string handlerFormat =
@"using Google.Protobuf;
using {0}.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {{
    {1}
}}";

        /// <summary>
        /// {0}: MsgID with Underlines
        /// {1}: UserType (Client | Server)
        /// </summary>
        public static string handlerFunctionFormat =
@"public static void {0}Handler(PacketSession s, IMessage packet) {{
        {0} parsedPacket = ({0})packet;
        {1}Session session = ({1}Session)s;

        throw new NotImplementedException();
    }}";

        #endregion
    }
}
