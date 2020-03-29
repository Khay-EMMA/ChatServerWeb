using System;
using System.Collections.Generic;
using System.Text;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;

namespace ChatServerWeb.BusinessLogic
{
    public class ChatApplicationsLogic : BaseBusinessLogic<ChatApplication>, IDisposable
    {
        public ChatApplicationsLogic(IRepository service) : base(service)
        {

        }
    }
}
