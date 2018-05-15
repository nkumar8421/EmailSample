using AE.Net.Mail;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailSample
{
    public class Email
    {
        public Email(string userName,string password)
        {
            UserName = userName;
            Password = password;
        }
        public string UserName { get; set; }
        public string Password { get; set; }


        public List<MailMessege> ReceiveMails()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();
            model.Inbox = new List<MailMessege>();

            string accountType ="gmail";

            try
            {
                EmailConfiguration email = new EmailConfiguration ();
                switch(accountType)
                {
                    case "gmail":
                        // type your popserver
                        email.POPServer = "imap.gmail.com";
                        break;
                    case "Outlook":
                        // type your popserver
                        email.POPServer = "outlook.office365.com";
                        break;
                }
                 
                email.POPUsername = UserName; // type your username credential
                email.POPpassword = Password; // type your password credential
                email.IncomingPort = "993";
                email.IsPOPssl = true;
                               
                int success = 0;
                int fail = 0;
                ImapClient ic = new ImapClient(email.POPServer, email.POPUsername, email.POPpassword, AuthMethods.Login, Convert.ToInt32(email.IncomingPort), (bool)email.IsPOPssl);
                // Select a mailbox. Case-insensitive
                ic.SelectMailbox("INBOX");
                int i = 1;
                int msgcount = ic.GetMessageCount("INBOX");
                int end = msgcount - 1;
                int start = msgcount - msgcount;
                // Note that you must specify that headersonly = false
                // when using GetMesssages().
                MailMessage[] mm = ic.GetMessages(start, end, false);
                   // var messages = ic.GetMessages(start, end, true);
                
                foreach (var item in mm)
                {
                    MailMessege obj = new MailMessege();
                    try
                    {
                        obj.Body = item.Body;
                        obj.UID = item.Uid;
                        obj.subject = item.Subject;
                        obj.sender = item.From.ToString();
                        obj.sendDate = item.Date;
                        obj.To = item.RawHeaders;
                        if (item.Attachments == null) { }
                        else obj.Attachments = item.Attachments;

                        model.Inbox.Add(obj);
                        success++;
                    }
                    catch (Exception e)
                    {
                        fail++;
                    }
                    i++;

                }
                ic.Dispose();
                model.Inbox = model.Inbox.OrderByDescending(m => m.sendDate).ToList();
                model.mess = "Mail received!\nSuccesses: " + success + "\nFailed: " + fail + "\nMessage fetching done";

                if (fail > 0)
                {
                    model.mess = "Since some of the emails were not parsed correctly (exceptions were thrown)\r\n" +
                                    "please consider sending your log file to the developer for fixing.\r\n" +
                                    "If you are able to include any extra information, please do so.";
                }
            }

            catch (Exception e)
            {
                
                model.mess = "Error occurred retrieving mail. " + e.Message;
            }
            finally
            {
                
            }
            return model.Inbox;
        }

        public void DeleteAllMessages()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();
            model.Inbox = new List<MailMessege>();
            string accountType = "gmail";

            try
            {
                EmailConfiguration email = new EmailConfiguration();
                switch (accountType)
                {
                    case "Gmail":
                        // type your popserver
                        email.POPServer = "imap.gmail.com";
                        break;
                    case "Outlook":
                        // type your popserver
                        email.POPServer = "outlook.office365.com";
                        break;
                }
                email.POPUsername = UserName; // type your username credential
                email.POPpassword = Password; // type your password credential
                email.IncomingPort = "993";
                email.IsPOPssl = true;

               
                ImapClient ic = new ImapClient(email.POPServer, email.POPUsername, email.POPpassword, AuthMethods.Login, Convert.ToInt32(email.IncomingPort), (bool)email.IsPOPssl);
                // Select a mailbox. Case-insensitive
                ic.SelectMailbox("INBOX");
                int i = 1;
                int msgcount = ic.GetMessageCount("INBOX");
                int end = msgcount - 1;
                int start = msgcount - msgcount;
                // Note that you must specify that headersonly = false
                // when using GetMesssages().
                MailMessage[] mm = ic.GetMessages(start, end, false);
                // var messages = ic.GetMessages(start, end, true);
                foreach (var it1 in mm)
                {
                    ic.DeleteMessage(it1);
                }


                ic.Dispose();



            }

            catch (Exception e)
            {

                model.mess = "Error occurred retrieving mail. " + e.Message;
            }
            finally
            {
               
            }
            //return model.Inbox;
        }

        //public string

        public bool VerifyNotificationEmail(List<MailMessege> mailMessages, Dictionary<string, string> verifyValues)
        {
            bool result = true;
            if(mailMessages.Count==0)
            {
                Console.WriteLine("No email found for verification");
                return false;
            }
            foreach (MailMessege item in mailMessages)
            {
                foreach (var verifyValue in verifyValues.ToArray())
                {
                    bool currentResult = false;
                    String exepected = verifyValue.Value;
                    String actual = null;
                    String name = null;

                    switch (verifyValue.Key.ToString())
                    {
                        case "From":
                            currentResult = item.sender.ToLower().Contains(verifyValue.Value);
                            exepected = verifyValue.Value;
                            actual = item.From;
                            name = "From Address";
                            
                            break;
                        case "To":
                            currentResult = item.To.ToLower().Contains(verifyValue.Value.ToLower());
                            exepected = verifyValue.Value;
                            actual = item.To;
                            name = "To Address";
                            
                            break;
                        case "Subject":
                            currentResult = item.subject.Contains(verifyValue.Value);
                            exepected = verifyValue.Value;
                            actual = item.subject;
                            name = "Subject";
                            
                            break;
                        case "Body":
                            currentResult = item.Body.Contains(verifyValue.Value);
                            exepected = verifyValue.Value;
                            actual = item.Body;
                            name = "Body";
                            break;
                        case "PhoneNumber":
                            currentResult = item.Body.Contains(verifyValue.Value);
                            exepected = verifyValue.Value;
                            actual = item.Body;
                            name = "Body";
                            break;

                        default:
                            name = $"Check Key {verifyValue.Key}";
                            currentResult = false;
                            exepected = "Found";
                            actual = "Not Found";
                            break;

                    }
                    if (currentResult)
                        Console.WriteLine($"Value for [{name}]({exepected}) == '{actual}' Ok.");
                    else
                        Console.WriteLine($"Value for [{name}]({exepected}) != '{actual}' Failed. *ERROR*");

                    result &= currentResult;
                }
            }
            return result;
        }
    }
}
