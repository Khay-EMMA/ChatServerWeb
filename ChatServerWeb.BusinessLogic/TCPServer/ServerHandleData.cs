using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using ChatServerWeb.BusinessLogic.Service;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.Model.ViewModel;
using ChatServerWeb.SystemUtility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ChatServerWeb.BusinessLogic.TCPServer
{
    /* ----------------------------------------
 * |ServerHandleData.cs Class by Ugochukwu Aronu © 2018|
 * ----------------------------------------
 * This class is needed to handle all info
 * which gets sent by the client to the server.
 * It checks which packet got sent, reads it
 * out and will execute the assigned code for it.
 */
    public class ServerHandleData
    {
        private readonly ChatMessageSingletonService _chatMessageSingletonService;
        private readonly ChatMessageLogic _chatMessageLogic;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        public ServerHandleData(ChatMessageSingletonService chatMessageSingletonService, IMapper mapper, 
            IServiceProvider serviceProvider)
        {
            _chatMessageSingletonService = chatMessageSingletonService;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        //makes sure that you need a index and packet(byte[]) to read out the dictionary
        public delegate void PacketDelegate(int connectionId, byte[] data);

        //dictionary filled with packets to listen to.
        private static Dictionary<int, PacketDelegate> _packetDictionary;

        public static int PacketLength;

        //creates a new dictionary of packets to listen to so it executes the correct
        //method when the packet is arriving at the server.
        public void InitPacketsFromClient()
        {
            _packetDictionary = new Dictionary<int, PacketDelegate>
            {
                //Add your packets in here, so the server knows which method to execute.
                //Add your packets in here, so the client knows which method to execute.
                { (int)ClientPackets.ClientChatMessage,HandleChatMessageFromClient},
                { (int)ClientPackets.ClientChatHistory,HandleChatHistoryFromClient},
                { (int)ClientPackets.ClientCreateGroup,HandleClientCreateGroup},
                { (int)ClientPackets.ClientCreateChatUser,HandleClientCreateChatUser},
                { (int)ClientPackets.ClientJoinGroup,HandleClientJoinGroup},
            };
            Text.WriteLine("Client Packets Initialized...", TextType.DEBUG);

        }

    

        /// <summary>
        /// Accurately extract the data and assign
        /// the corresponding bytes to the Client's ByteBuffer
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="newBytesRead"></param>
        internal static void HandleDataFromClient(int connectionId, byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();

            if (ServerTcp.Clients[connectionId].ByteBuffer == null)
            {
                //creating a new instance of 'Bytebuffer' to read out the packet.
                ServerTcp.Clients[connectionId].ByteBuffer = new ByteBuffer();
            }
            //writing incoming packet to the buffer.
            ServerTcp.Clients[connectionId].ByteBuffer.WriteBytes(buffer);

            if (ServerTcp.Clients[connectionId].ByteBuffer.Count() == 0)
            {
                ServerTcp.Clients[connectionId].ByteBuffer.Clear();
                return;
            }

            //Check if the byte buffer has data as big as an int size
            if (ServerTcp.Clients[connectionId].ByteBuffer.Count() >= Constants.INT_SIZE)
            {
                PacketLength = ServerTcp.Clients[connectionId].ByteBuffer.ReadInteger(false);
                if (PacketLength <= 0)
                {
                    //no packet exists in the bytebuffer
                    ServerTcp.Clients[connectionId].ByteBuffer.Clear();
                    return;
                }
            }

            while (PacketLength > 0 && PacketLength <= ServerTcp.Clients[connectionId].ByteBuffer.Length() - Constants.INT_SIZE)
            {
                if (PacketLength <= ServerTcp.Clients[connectionId].ByteBuffer.Length() - Constants.INT_SIZE)
                {
                    ServerTcp.Clients[connectionId].ByteBuffer.ReadInteger();

                    data = ServerTcp.Clients[connectionId].ByteBuffer.ReadBytes(PacketLength);

                    HandleDataPacketsFromClient(connectionId, data);
                }


                PacketLength = 0;
                if (ServerTcp.Clients[connectionId].ByteBuffer.Length() >= Constants.INT_SIZE)
                {

                    PacketLength = ServerTcp.Clients[connectionId].ByteBuffer.ReadInteger(false);

                    if (PacketLength <= 0)
                    {
                        ServerTcp.Clients[connectionId].ByteBuffer.Clear();
                        return;
                    }

                }

                if (PacketLength <= 1)
                {
                    ServerTcp.Clients[connectionId].ByteBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// Correctly check the packet sent and invoke the
        /// correct function(delegate)
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        private  static void HandleDataPacketsFromClient(int connectionId, byte[] data)
        {
            //creating a new instance of 'Bytebuffer' to read out the packet.
            ByteBuffer byteBuffer = new ByteBuffer();
            //writing incoming packet to the buffer.
            byteBuffer.WriteBytes(data);
            //reads out the packet to see which packet we got.
            int packet = byteBuffer.ReadInteger();
            //closes the buffer.
            byteBuffer.Dispose();
            //checking if we are listening to that packet in the _packets Dictionary.
            if (_packetDictionary.TryGetValue(packet, out PacketDelegate packetDelegate))
            {
                //checks which Method is assigned to the packet and executes it,
                //index: the socket which sends the data
                //data: the packet byte [] with the information.
                packetDelegate.Invoke(connectionId, data);
            }
        }

        private void HandleChatMessageFromClient(int connectionId, byte[] data)
        {
            try
            {
                //Creates a new instance of the buffer to read out the packet.
                ByteBuffer buffer = new ByteBuffer();
                //writes the packet into a list to make it avaiable to read it out.
                buffer.WriteBytes(data);
                //Todo INFO: You always have to read out the data as you sent it. 
                //In this case you always have to first to read out the packet identifier.
                int packetIdentify = buffer.ReadInteger();
                //In the server side you now send a string as next so you have to read out the string as next.
                string chatJsonString = buffer.ReadString();

                //todo: convert to chat model and Save this chat in the singleton

                ChatMessageViewModel chatMessage = JsonConvert.DeserializeObject<ChatMessageViewModel>(chatJsonString);


                //todo: remove this message later
                string messageToDisplay = $"App ID: {chatMessage.AppId} Message: {chatMessage.Message} " +
                                          $"User ID: {chatMessage.UserEmail} " +
                                          $"Group Type: {Enum.GetName(typeof(GroupType), chatMessage.GroupType) } " +
                                          $"Date: {DateTime.UtcNow}";

                string messageJsonString = JsonConvert.SerializeObject(chatMessage);
                //Send message to other clients
                ServerTcp.SendChatMessageToClient(connectionId, messageJsonString);

                //Save Chat Message
                SaveChatMessage(chatMessage);

                //print out the string msg you did send from the server.
                Text.WriteLine($"Pckt ID: {packetIdentify} - Message: {chatMessage} ", TextType.INFO);
            }
            catch (Exception e)
            {
                Text.WriteLine($"Error occured in ServerHandleData:HandleChatMessageFromClient  with message {e.Message}", TextType.ERROR);
            }
        }

        /// <summary>
        /// Create a group if the group does not exist
        /// </summary>
        /// <param name="connectionid"></param>
        /// <param name="data"></param>
        private void HandleClientCreateGroup(int connectionid, byte[] data)
        {
            try
            {
                IRepository repository = _serviceProvider.GetRequiredService<IRepository>();
                ChatGroupLogic chatGroupLogic = new ChatGroupLogic(repository);

                //Creates a new instance of the buffer to read out the packet.
                ByteBuffer buffer = new ByteBuffer();
                //writes the packet into a list to make it available to read it out.
                buffer.WriteBytes(data);
                //Todo INFO: You always have to read out the data as you sent it. 
                //In this case you always have to first to read out the packet identifier.
                int packetIdentify = buffer.ReadInteger();
                //In the server side you now send a string as next so you have to read out the string as next.
                string appId = buffer.ReadString();
                string userEmail = buffer.ReadString();
                (bool,string) response = chatGroupLogic.CreateNewChatGroup(appId, userEmail);
               
                int responseStatus = response.Item1 ? (int)BooleanStatus.True : (int)BooleanStatus.False;
                //Send create group response to the client
                ServerTcp.SendCreateGroupResponse(connectionid, responseStatus, response.Item2);
            }
            catch (Exception e)
            {
                //todo: Log error
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// Gets chat history from singleton and sends it to the client
        /// </summary>
        /// <param name="connectionid"></param>
        /// <param name="data"></param>
        private void HandleChatHistoryFromClient(int connectionid, byte[] data)
        {
            //Creates a new instance of the buffer to read out the packet.
            ByteBuffer buffer = new ByteBuffer();
            //writes the packet into a list to make it avaiable to read it out.
            buffer.WriteBytes(data);
            //Todo INFO: You always have to read out the data as you sent it. 
            //In this case you always have to first to read out the packet identifier.
            int packetIdentify = buffer.ReadInteger();
            //In the server side you now send a string as next so you have to read out the string as next.
            string appId = buffer.ReadString();

            //get the chat message history by application Id
            List<ChatMessage> chatMessageHistory = _chatMessageSingletonService.GetChatMessageHistoryByChatApplication(appId);


            //todo: use auto mapper to convert this to a viewmodel
            List<ChatMessageViewModel> chatMessageViewModels = _mapper.Map<List<ChatMessageViewModel>>(chatMessageHistory);
            string chatMessageHistoryJsonString = JsonConvert.SerializeObject(chatMessageViewModels);
            //Send chat history to client with connection id
            ServerTcp.SendChatMessageHistoryToClient(connectionid, chatMessageHistoryJsonString);
        }
        /// <summary>
        /// Create a new user if the user does not exist.
        /// Nothing will be returned to the client for now
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        private void HandleClientCreateChatUser(int connectionId, byte[] data)
        {
            IRepository repository = _serviceProvider.GetRequiredService<IRepository>();
            ChatApplicationUserLogic chatApplicationUserLogic = new ChatApplicationUserLogic(repository);

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetIdentify = buffer.ReadInteger();
            //In the server side you now send a string as next so you have to read out the string as next.
            string appId = buffer.ReadString();
            string userEmail = buffer.ReadString();
            string username = buffer.ReadString();

            chatApplicationUserLogic.CreateUser(appId, userEmail, username);
        }

        private void HandleClientJoinGroup(int connectionId, byte[] data)
        {
            try
            {
                IRepository repository = _serviceProvider.GetRequiredService<IRepository>();
                ChatApplicationUserLogic chatApplicationUserLogic = new ChatApplicationUserLogic(repository);

                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteBytes(data);
                int packetIdentify = buffer.ReadInteger();
                //In the server side you now send a string as next so you have to read out the string as next.
                string appId = buffer.ReadString();
                string userEmail = buffer.ReadString();
                string groupCreatorEmail = buffer.ReadString();

                (bool, string) response = chatApplicationUserLogic.AddUserToGroup(appId, userEmail, groupCreatorEmail);

                ServerTcp.SendJoinGroupResponseToClient(response.Item1, response.Item2);
            }
            catch (Exception e)
            {
                //todo: log error
                Console.WriteLine(e);
            }
         


        }
        /// <summary>
        /// Saves the chat message to singleton which later syncs with database
        /// </summary>
        /// <param name="chatMessageModel"></param>     
        public void SaveChatMessage(ChatMessageViewModel chatMessageModel)
        {
            //Assign chat message to entity
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.AppId = chatMessageModel.AppId;
            chatMessage.DateCreated = DateTime.UtcNow;
            if (chatMessageModel.GroupType == (int)GroupType.Private)
            {
                chatMessage.GroupId = chatMessageModel.GroupId;
            }
            chatMessage.Status = (int) ChatMessageStatus.Active;
            chatMessage.GroupType = chatMessageModel.GroupType;
            chatMessage.Message = chatMessageModel.Message;
            chatMessage.UserEmail = chatMessageModel.UserEmail;
            chatMessage.Username = chatMessageModel.Username;
            //Add entity to singleton 
           
            if (chatMessageModel.GroupType == (int)GroupType.General)
            {
                //Add to general chat singleton
                _chatMessageSingletonService.AddChatMessage(chatMessage,GroupType.General);
                
                //Save to database directly
                //_chatMessageLogic.AddEntity(chatMessage);
                //_chatMessageLogic.Save();
            }
            else
            {
                //Add to private chat
            }
        }


     
        //todo: bypass singleton and hangfire operations, lets deal with the database directly and monitor performance
    }
}
