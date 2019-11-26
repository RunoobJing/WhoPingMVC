using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhoPingMVC.Models {
    public class BaseCommunicationReponse {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class PostActivityResponse : BaseCommunicationReponse {
    }
}