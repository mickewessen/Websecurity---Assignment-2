using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Websecurity___Essay_2.Models
{
    public class UserInput
    {
        public Guid Id { get; set; }

        [StringLength(500, MinimumLength = 1)]
        [Required]
        public string Content { get; set; }

        [StringLength(25, MinimumLength = 1)]
        [Required]
        public string Author { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
