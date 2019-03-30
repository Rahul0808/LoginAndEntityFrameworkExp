using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppFinal.Common.Entities
{
    public class BaseEntity
    {
     
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the created user.
        /// </summary>
        /// <value>
        /// The created user.
        /// </value>

        public string CreatedUser { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        /// 

        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the updated user.
        /// </summary>
        /// <value>
        /// The updated user.
        /// </value>
        
        public string UpdatedUser { get; set; }

        /// <summary>
        /// Gets or sets the updated time.
        /// </summary>
        /// <value>
        /// The updated time.
        /// </value>
        
        public DateTime? UpdatedTime { get; set; }
    }
}
