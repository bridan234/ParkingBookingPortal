using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmailSender
{
    public  interface IEmail
    {
        [Required, Display(Name = "From"), DataType(DataType.EmailAddress)]
        public string FromAddress { get; set; }

        [Required, Display(Name = "To"), DataType(DataType.EmailAddress)]
        public string ToAddress { get; set; }

        [Required, Display(Name = "Email Subject"), DataType(DataType.Text)]
        public string Subject { get; set; }

        [Display(Name = "Message Body"), DataType(DataType.MultilineText)]
        public string MessageBody { get; set; }

        [Required, Display(Name = "Body HTML"), DataType(DataType.Text)]
        public string BodyHtml { get; set; }


    }
}
