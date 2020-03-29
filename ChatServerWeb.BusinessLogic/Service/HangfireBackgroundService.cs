using System;
using System.Collections.Generic;
using System.Text;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using Hangfire;
using Hangfire.SqlServer;

namespace ChatServerWeb.BusinessLogic.Service
{
    /// <summary>
    /// Handles all the background jobs required in the application
    /// </summary>
    public class HangfireBackgroundService
    {
        private readonly IRepository _repository;
        private readonly ChatMessageSingletonService _chatMessageSingletonService;
        private readonly ChatMessageLogic _chatMessageLogic;
        private readonly ChatApplicationUserLogic _chatApplicationUserLogic;
        public HangfireBackgroundService(IRepository repository, ChatMessageSingletonService chatMessageSingletonService, 
            ChatMessageLogic chatMessageLogic, ChatApplicationUserLogic chatApplicationUserLogic)
        {
            _repository = repository;
            _chatMessageSingletonService = chatMessageSingletonService;
            _chatMessageLogic = chatMessageLogic;
            _chatApplicationUserLogic = chatApplicationUserLogic;

        }
        /// <summary>
        /// 1. Gets the current general chat messages from the singleton and saves them in the database
        /// 2. Sets the chat message dictionary of the singleton to empty after saving to database
        /// NOTE: To set the chat message dictionary of the singleton to empty, it has to be done carefully,
        /// by getting the index of each chat object according to the chat application ID, then removing by index
        /// </summary>
        public void SaveGeneralChatMessageFromSingleton()
        {
            try
            {
                Dictionary<string, List<ChatMessage>> generalChatMessagesByChatApplication = _chatMessageSingletonService.GeneralChatMessagesByChatApplication;

                bool entityAdded = false;
                Dictionary<string, List<int>> messageObjectsToRemoveByApplication = new Dictionary<string, List<int>>();
                foreach (var item in generalChatMessagesByChatApplication)
                {
                    List<int> itemsToRemove = new List<int>();

                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        ChatMessage chatMessage = new ChatMessage();
                        chatMessage.AppId = item.Value[i].AppId;
                        chatMessage.DateCreated = item.Value[i].DateCreated;
                        chatMessage.GroupId = item.Value[i].GroupId;
                        chatMessage.GroupType = item.Value[i].GroupType;
                        chatMessage.Message = item.Value[i].Message;
                        chatMessage.Status = item.Value[i].Status;
                        chatMessage.UserEmail = item.Value[i].UserEmail;
                        chatMessage.Username = item.Value[i].Username;
                        _chatMessageLogic.AddEntity(chatMessage);
                        itemsToRemove.Add(i);
                        entityAdded = true;
                    }

                    messageObjectsToRemoveByApplication.Add(item.Key, itemsToRemove);
                }

                if (entityAdded)
                {
                    //save to database
                    _repository.Save();
                    //clear singleton data
                    _chatMessageSingletonService.ClearGeneralChatMessages(messageObjectsToRemoveByApplication);
                }

                
            }
            catch (Exception e)
            {
                //todo: Log exception to db
                Console.WriteLine(e);
            }
          

        }
        /// <summary>
        /// 1. Gets the current private chat messages from the singleton and saves them in the database
        /// 2. Sets the chat message dictionary to empty after saving to database
        /// </summary>
        public void SaveGroupChatMessageFromSingleton()
        {

        }


        /// <summary>
        /// 1. Get the chat history from the database and update the singleton
        /// 2. Time interval should be 1 day
        /// </summary>
        public void GetChatHistoryFromDatabase()
        {

        }
        public void EnqueueChatMessageJobs()
        {

            RecurringJob.AddOrUpdate("SaveGeneralChatMessageFromSingleton", () => SaveGeneralChatMessageFromSingleton(), Cron.MinuteInterval(2));
        }

    }
}
