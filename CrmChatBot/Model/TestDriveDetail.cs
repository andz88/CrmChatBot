using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmChatBot.Model
{
    [Serializable]
    public class TestDriveDetail
    {
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public string RequestedTime { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
    }
}