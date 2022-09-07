using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB {
    [Table("UserAccount")]
    public class UserAccount {
        public int UserAccountID { get; set; }
        public string ID { get; set; }
        public string Password { get; set; }
        public uint AuthCode { get; set; }
    }
}
