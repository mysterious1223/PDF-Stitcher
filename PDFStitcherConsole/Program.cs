using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFStitcherConsole
{
    class Program
    {




        static void Main(string[] args)
        {
           
            /* APPEND
             -k ClaimNumber_ -A -S "C:\Users\ksingh\Desktop\Current Work\Newfolder\supp" -C "C:\Users\ksingh\Desktop\Current Work\Newfolder\claims"
              MERGE
              -M -F "C:\Users\ksingh\Desktop\Current Work\Newfolder\supp" 
             */

            PDFStitcher prog = new PDFStitcher(args);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~");
            prog.debug();

            Console.ReadKey();
        }
    }
}
