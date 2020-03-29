using ChatServerWeb.SystemUtility;
using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ChatServerWeb.Test
{
    public class ChatMessageConverter
    {
        readonly ITestOutputHelper _output;

        public ChatMessageConverter(ITestOutputHelper output)
        {
            _output = output;
        }
        /// <summary>
        /// Compare the size of byte and strings to optimize storage of chat message
        /// </summary>
        [Fact]      
        public void CompareMemorySize()
        {
            long beforeStringAllocation = GC.GetTotalMemory(true);
            string input = "Hello my name is ugo. I love your chat service";
            long afterStringAllocation = GC.GetTotalMemory(true);

            long difference = afterStringAllocation - beforeStringAllocation;

            byte[] messageBytes = Encoding.ASCII.GetBytes(input);

            long afterBytesAllocation = GC.GetTotalMemory(true);

            long differenceAfterByteAllocation = afterBytesAllocation - afterStringAllocation;

            _output.WriteLine($"String difference {difference.ToString()}");
            _output.WriteLine($"Byte difference {differenceAfterByteAllocation.ToString()}");
        }


    }
}
