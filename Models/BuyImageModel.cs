using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class BuyImageModel
    {
        public int UserId { get; set; }
        public int PublicationId { get; set; }
        public CardModel Card { get; set; }
       
    }
}