using System;
using System.Collections.Generic;
using System.Text;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.SystemUtility;

namespace ChatServerWeb.BusinessLogic
{

    public class ChatApplicationUserLogic : BaseBusinessLogic<ChatApplicationUser>, IDisposable
    {
        private IRepository _repository;

        public ChatApplicationUserLogic(IRepository service) : base(service)
        {
            _repository = service;
        }

        /// <summary>
        /// 1. Check if user exists before creating
        /// 2. If exists, do nothing
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userEmail"></param>
        /// <param name="username"></param>
        internal void CreateUser(string appId, string userEmail, string username)
        {
            ChatApplicationUser existingUser = GetEntityBy(p => p.AppId == appId && p.Email == userEmail);
            if (existingUser == null)
            {
                ChatApplicationUser user = new ChatApplicationUser
                {
                    AppId = appId,
                    Email = userEmail,
                    DateCreated = DateTime.UtcNow,
                    Username = username,
                    Status = (int)UserStatus.Active
                };

                AddEntity(user);
                Save();
            }

        }

        /// <summary>
        /// To add user to group
        /// 1. Check if the user exists, if true, return false and nice error message
        /// 2. Check if the group exists by checking with the groupcreator's email,if true, return false and nice error message
        /// 3. Check if the user already belongs to a group, if true, return false and nice error message
        /// 4. Add the user to the group if all conditions above are satisfied
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userEmail"></param>
        /// <param name="groupCreatorEmail"></param>
        /// <returns></returns>
        internal (bool, string) AddUserToGroup(string appId, string userEmail, string groupCreatorEmail)
        {
            try
            {
                ChatGroupLogic chatGroupLogic = new ChatGroupLogic(_repository);
                var user = GetEntityBy(p => p.AppId == appId && p.Email == userEmail);
                if (user == null)
                    return (false, "User does not exist");
                var group = chatGroupLogic.GetEntityBy(p => p.AppId == appId && p.CreatedByUser == groupCreatorEmail);
                if (group == null)
                    return (false, "Group does not exist");
                if (user.GroupId != null)
                    return (false, "User already belongs to a group, please leave the group before you can join another group");

                user.GroupId = group.GroupId;
                Save();
                return (true, "You just joined successfully");
            }
            catch (Exception e)
            {
                throw e;

            }

        }
    }
}
