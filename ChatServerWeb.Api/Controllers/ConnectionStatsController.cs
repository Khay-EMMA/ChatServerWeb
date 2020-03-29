using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerWeb.BusinessLogic.Service;
using ChatServerWeb.BusinessLogic.TCPServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatServerWeb.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionStatsController : ControllerBase
    {
        private readonly ChatMessageSingletonService _chatMessageSingletonService;
        public ConnectionStatsController(ChatMessageSingletonService chatMessageSingletonService)
        {
            _chatMessageSingletonService = chatMessageSingletonService;
        }
        // GET api/values
        [HttpGet("ActiveConnections")]
        public IActionResult GetAcriveConnections()
        {
            int activeConnections = ServerTcp.ActiveConnection;
            return Ok(activeConnections);
        }

        [HttpGet("ChatPool")]
        public IActionResult GetChatPoolCount()
        {
            int generalChats = _chatMessageSingletonService.GeneralChatMessagesByChatApplication.Count;
            return Ok($"General Chat Pool: {generalChats}");
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
