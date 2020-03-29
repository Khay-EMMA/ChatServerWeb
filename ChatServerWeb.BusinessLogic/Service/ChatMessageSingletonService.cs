using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.SystemUtility;
using Microsoft.Win32.SafeHandles;

namespace ChatServerWeb.BusinessLogic.Service
{
    public class ChatMessageSingletonService:IDisposable
    {
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);


        public Dictionary<string,List<ChatMessage>> GeneralChatMessagesByChatApplication { get; set; }
        public Dictionary<string,Dictionary<string,List<ChatMessage>>> GroupChatMessagesByChatApplication { get; set; }
        


        //Holds all chat messages, both private and group chats
        //Chat history is trimmed to avoid overflow
        public Dictionary<string, List<ChatMessage>> ChatMessageHistoryByChatApplication { get; set; }

        public ChatMessageSingletonService()
        {
            GeneralChatMessagesByChatApplication = new Dictionary<string, List<ChatMessage>>();
            GroupChatMessagesByChatApplication = new Dictionary<string, Dictionary<string, List<ChatMessage>>>();
            ChatMessageHistoryByChatApplication = new Dictionary<string, List<ChatMessage>>();
        }

        public void AddChatMessage(ChatMessage message, GroupType groupType)
        {
            switch (groupType)
            {
                case GroupType.General:
                    GeneralChatMessagesByChatApplication.TryGetValue(message.AppId, out var generalChatMessages);
                    generalChatMessages?.Add(message);
                    break;
                case GroupType.Private:
                    break;
            }

            //add to chat history singleton, all chats must be added to chat history
            ChatMessageHistoryByChatApplication.TryGetValue(message.AppId, out var chatHistory);
            chatHistory?.Add(message);
        }

        /// <summary>
        /// Filter message history by application id
        /// UserEmail not in use for now.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ChatMessage> GetChatMessageHistoryByChatApplication(string appId)
        {
            ChatMessageHistoryByChatApplication.TryGetValue(appId, out var chatMessageHistory);

            return chatMessageHistory;
        }
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void ClearGeneralChatMessages(Dictionary<string, List<int>> generalChatMessagesByChatApplicationToRemove)
        { 
            foreach (var item in generalChatMessagesByChatApplicationToRemove)
            {
                //get the current general chat messages based on the application id
                GeneralChatMessagesByChatApplication.TryGetValue(item.Key, out var currentGeneralChatMessages);

                //loop through the messages and remove at indexes in the list of items to remove
                for (int i = 0; i < item.Value.Count; i++)
                {
                    currentGeneralChatMessages?.RemoveAt(item.Value[i]);
                }
            }
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.

            disposed = true;
        }
    }
}
