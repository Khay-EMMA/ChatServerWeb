using System;
using System.Collections.Generic;
using System.Text;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;

namespace ChatServerWeb.BusinessLogic
{

    public class ChatMessageLogic : BaseBusinessLogic<ChatMessage>, IDisposable
    {
        public ChatMessageLogic(IRepository service) : base(service)
        {

        }
    }
}
