using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WePair.Models.Requests.Messages
{
    public class MessageAddRequest
    {
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Body { get; set; }

#nullable enable
        public string? Subject { get; set; }
#nullable disable

        [Required]
        public int RecipientId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Sent")]
        public DateTime DateSent { get; set; }


    }
}
