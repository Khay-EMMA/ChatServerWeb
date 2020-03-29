using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.SystemUtility;
using Microsoft.EntityFrameworkCore.Internal;

namespace ChatServerWeb.BusinessLogic.Service
{
    /// <summary>
    /// This class initialized required objects before
    /// the rest of the application starts
    /// This initializes objects that have to deal with database calls or makes use of D.I
    /// </summary>
    public class ApplicationInitializer
    {
        private readonly ChatMessageSingletonService _chatMessageSingletonService;
        private readonly ChatMessageLogic _chatMessageLogic;
        private readonly ChatApplicationsLogic _chatApplicationsLogic;
        private readonly HangfireBackgroundService _hangfireBackgroundService;
        private readonly IRepository _repository;
        private List<ChatApplication> _chatApplications;

        public ApplicationInitializer(ChatMessageSingletonService chatMessageSingletonService, 
            IRepository repository, HangfireBackgroundService hangfireBackgroundService)
        {
            _chatMessageSingletonService = chatMessageSingletonService;
            _repository = repository;
            _hangfireBackgroundService = hangfireBackgroundService;
            _chatMessageLogic = new ChatMessageLogic(_repository);
            _chatApplicationsLogic = new ChatApplicationsLogic(_repository);
            _chatApplications = _chatApplicationsLogic.GetEntitiesBy(p => p.IsActive);
        }

 

        public void InitializeSingleton()
        {
            if (_chatMessageSingletonService.GeneralChatMessagesByChatApplication.Count <= 0)
            {
                List<ChatMessage> messages = _chatMessageLogic.GetAllEntities();
                PopulateChatSingleton(messages);

            }
        }


        /// <summary>
        /// On application start, set the general application singleton to empty
        /// </summary>
        /// <param name="chatMessages"></param>
        /// <param name="applicationId"></param>
        public void InitializeGeneralChatByChatApplication(List<ChatMessage> chatMessages, string applicationId)
        {
            Dictionary<string, List<ChatMessage>> generalChatMessagesByChatApplication = new Dictionary<string, List<ChatMessage>>();
            generalChatMessagesByChatApplication.Add(applicationId, new List<ChatMessage>());
            _chatMessageSingletonService.GeneralChatMessagesByChatApplication = generalChatMessagesByChatApplication;

        }
        public void PopulateChatMessageHistoryByChatApplication(List<ChatMessage> chatMessages, string applicationId)
        {
            Dictionary<string, List<ChatMessage>> ChatMessageHistoryByChatApplication = new Dictionary<string, List<ChatMessage>>();
            ChatMessageHistoryByChatApplication.Add(applicationId, chatMessages);
            _chatMessageSingletonService.ChatMessageHistoryByChatApplication = ChatMessageHistoryByChatApplication;
        }

        public void PopulateChatSingleton(List<ChatMessage> chatMessages)
        {
            for (int i = 0; i < _chatApplications.Count; i++)
            {
                string applicationId = _chatApplications[i].AppId;
                List<ChatMessage> chatMessagesByApplication = chatMessages.Where(p=>p.AppId == applicationId).ToList();
                //populate general chat message by application
                InitializeGeneralChatByChatApplication(chatMessagesByApplication, applicationId);
                //populate overall chat message history by application
                PopulateChatMessageHistoryByChatApplication(chatMessagesByApplication, applicationId);

                //todo: populate GroupChatMessagesByChatApplication

            }
        }
    }
}
