using System;
using System.Collections.Generic;

using AE.Net.Mail;

namespace EmailSample
{
    public class MailMessege
    {
        public string From { get; set; }

        public string To { get; set; }
        public string subject { get; set; }
        public string sender { get; set; }
        public string UID { get; set; }
        public int MessegeNo { get; set; }
        public DateTime sendDate { get; set; }

        public string Body { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }
}
