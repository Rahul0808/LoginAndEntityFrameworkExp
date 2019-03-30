using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppFinal.Common.Entities
{
    public class UserAccount : BaseEntity
    {
        public int AccountId { get; set; }

        public decimal Balance { get; set; }
    }
}
