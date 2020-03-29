using System;
using System.Collections.Generic;
using System.Text;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;

namespace ChatServerWeb.BusinessLogic
{


    public class ChatGroupLogic : BaseBusinessLogic<ChatGroup>, IDisposable
    {
        private IRepository _repository;
        public ChatGroupLogic(IRepository service) : base(service)
        {
            _repository = service;
        }

        /// <summary>
        /// To create chat group
        /// 1. Check if the user already belongs to a group, if true, return false
        /// 2. Check if the user already created a group
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public (bool,string) CreateNewChatGroup(string appId, string userEmail)
        {
            try
            {
                //Check if user belongs to a group already
                ChatApplicationUserLogic chatApplicationUserLogic = new  ChatApplicationUserLogic(_repository);
                var user = chatApplicationUserLogic.GetEntityBy(p => p.AppId == appId && p.Email == userEmail);
                if (user == null)
                    return (false, "User does not exist");
                if (user.GroupId != null)
                    return (false, "User already belongs to a group, please leave the group before you can create your own");     
                //check if group already exists
                ChatGroup group = GetEntityBy(p => p.CreatedByUser == userEmail && p.AppId == appId);
                if (group == null)
                {
                    group = new ChatGroup();
                    group.CreatedByUser = userEmail;
                    group.AppId = appId;
                    group.DateCreated = DateTime.UtcNow;
                    group.GroupName = userEmail + " Group";
                    group.NumberOfMembers = 1;
                    AddEntity(group);
                    Save();
                    return (true, "Group created successfully");
                }

                return (false, "Group already exists");
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
