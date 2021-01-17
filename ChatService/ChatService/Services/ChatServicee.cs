using ChatService.Protos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Services
{
    public class ChatServicee : ChatRoomService.ChatRoomServiceBase
    {
        private readonly ChatRoom _chatRoom;

        public ChatServicee(ChatRoom chatRoom)
        {
            _chatRoom = chatRoom;
        }

        public override async Task Join(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;
            do
            {
                _chatRoom.Join(requestStream.Current.User, responseStream);
                await _chatRoom.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());
                _chatRoom.Remove(context.Peer);
            
        }
    }
}
