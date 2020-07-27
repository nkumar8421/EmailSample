using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSample
{
    class Program
    {
        static void Main(string[] args)
        {
			-- This is main method
            Email email = new Email("SampleEmail78945", "infy@123$%^");
            var allemails = email.ReceiveMails();

            foreach(MailMessege item in allemails)
            {
                Console.WriteLine("From address :" + item.From + "\n");
                Console.WriteLine("TO address :" + item.To + '\n');
            }
            Console.ReadLine();
        }
    }
}
