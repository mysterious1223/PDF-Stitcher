using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;
namespace PDFStitcherConsole
{
    class PDFStitcher
    {
        // arguments
        public List<string> arguments = new List<string>();
        List<string> arg_list = new List<string>();
        // - M or - m is merge for supporting documents

        private string merge_directory="";
        
        
        // - A or - a append supporting documents to our claim

        private string folder_supp="", folder_claims="";

        // - K or - k Special string

        private string special_String="";

        // states
        private enum State
        {
            None,Merge, Append
        };

        State mode;

        public PDFStitcher (string[] args)
        {


            arg_list.Add("-M");

            arg_list.Add("-F");

            arg_list.Add("-A");

            arg_list.Add("-S");

            arg_list.Add("-C");

            arg_list.Add("-K");




            if (handle_arguments (args))
            {
                Console.WriteLine("Command looks good. Proceeding...");

                if (mode == State.Append)
                {
                    //append
                    if (!Append_mode_process())
                    {
                        Console.WriteLine("Append failed!");
                        
                    }
                    else
                    {
                        Console.WriteLine("Append Done!");

                    }
                }

                if (mode == State.Merge)
                {
                    //merge
                    if (!Merge_mode_process('_'))
                    {
                        Console.WriteLine("Merge failed!");

                    }
                    else
                    {
                        Console.WriteLine("Merge Done!");

                    }
                }
            }
            else
            {
                print_error();
            }

            Console.WriteLine("exiting...");

        }
        private bool Merge_mode_process (char delimeter)
        {
            // This function will merge any duplicate supp docs then rename and place in a Merged folder
            // This function isnt modular
            

            Console.WriteLine("Starting Merge...");

            if (!Directory.Exists(merge_directory + @"\Merged\"))
            {
                Console.WriteLine("!Merge directory doesnt exist. Creating...");

                try
                {
                    Directory.CreateDirectory(merge_directory + @"\Merged\");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to create merge directory "+ ex.ToString());
                    return false;
                }
                Console.WriteLine("!Merge directory Created!");

            }


            // get all supporting document files

            List<String> suppDocs = Directory.GetFiles(merge_directory, "*.pdf").ToList<String>();

            foreach (object ai in suppDocs)
            {

                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                string[] strings = ai.ToString().Split(delimeter);

                string stringtocheck = "";
                for (int i = 0; i < strings.Length; i++)
                {
                    if (i < 2)
                    {
                        stringtocheck += strings[i] + delimeter;
                    }
                }

                Console.WriteLine("[processing] " + stringtocheck);

                var newlist = suppDocs.Where(
                    a => a.Contains(stringtocheck)
                    );


                if (newlist.Count() > 1)
                {
                    Console.WriteLine("FOUND A DUPE. Merging and moving...");

                    try
                    {
                        PDFManipulator.Merge(newlist.ToList(),
                         merge_directory + @"\Merged\" +
                                Path.GetFileName(stringtocheck.Substring(0, stringtocheck.Length - 1)) + ".pdf");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Err : " + ex.ToString());

                        return false;
                    }

                }
                else
                {
                    Console.WriteLine("NOT A DUPE. moving...");


                    Console.WriteLine("Moving "+ ai.ToString()+" to " + merge_directory + @"\Merged\" + 
                        Path.GetFileName(stringtocheck.Substring(0,stringtocheck.Length-1)) + ".pdf");
                    try
                    {
                        File.Copy(ai.ToString(), merge_directory + @"\Merged\" +
                            Path.GetFileName(stringtocheck.Substring(0, stringtocheck.Length - 1)) + ".pdf");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Err : " + ex.ToString());

                        return false;
                    }
                }
                // with new list we can merge these documents and move to the new folder

                foreach (object a in newlist)
                {
                    Console.WriteLine(a);
                }

                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }


            






            return true;
        }
        private bool Append_mode_process()
        {
            // how will we go about this?

            // WE NEED TO CHECK IF THE MERGE DIRECTORY EXIST, IF NOT CREATE

            if (!Directory.Exists(folder_claims+@"\Merged\"))
            {
                Console.WriteLine("!Merge directory doesnt exist. Creating...");

                try
                {
                    Directory.CreateDirectory(folder_claims + @"\Merged\");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to create merge directory");
                    return false;
                }
                Console.WriteLine("!Merge directory Created!");

            }

            List<String> filesClaims = Directory.GetFiles(folder_claims, "*.pdf").ToList<String>();
            List<String> suppDocs = Directory.GetFiles(folder_supp, "*.pdf").ToList<String>();
          

            if (filesClaims.Count != suppDocs.Count)
            {
                Console.WriteLine("The count of claims and count of supporting documents DO NOT MATCH!");

                return false;
            }

            // We need to the supporting documents to have the same file name as the claim pdfs

            foreach (object claim in filesClaims)
            {
                // process a merge
                Console.WriteLine("[PROCESSING] Claim ->>>>>  " + Path.GetFileNameWithoutExtension(claim.ToString()));
                string temp="";
                foreach (object supp in suppDocs)
                {
                    if (Path.GetFileName(supp.ToString()) == Path.GetFileName(claim.ToString()))
                    {
                        temp = supp.ToString();
                        break;
                    }
                }

                if (temp == "")
                {
                    Console.WriteLine("Couldnt locate "+claim.ToString());
                    return false;


                }

                if (File.Exists(claim.ToString()))
                {
                    Console.WriteLine("Found "+claim.ToString());
                }
                if (File.Exists(temp))
                {
                    Console.WriteLine("Found " + temp);
                }

                try
                {
                    //Console.WriteLine("Trying to merge : " + claim.ToString() + " with :" + temp + " Mergeout : "+ folder_claims + @"\Merged\" + Path.GetFileName(claim.ToString()) );
                    PDFManipulator.Merge(new List<string> {
                        claim.ToString(),temp
                    }, 
                     folder_claims+@"\Merged\"+ Path.GetFileName(claim.ToString()));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("err : "+ex.ToString());
                    Console.WriteLine("File with error : " + claim.ToString());

                    return false;
                }
                Console.WriteLine("[PROCESS COMPLETE]");
            }
                

            return true;
        }

        public bool handle_arguments (string[] args)
        {
            //arguments = args.ToList<String>();

            string prev ="";

            
            if (args.Length < 1)
            {
                Console.WriteLine("Length error!");
                return false;
            }


            // check for errors
            
            // supplied parameters
            foreach (object a in args)
            {
                if (a.ToString().ToUpper().Contains("-"))
                {
                    string temp = a.ToString().ToUpper();

                    

                    if (!arg_list.Contains(temp))
                    {
                        Console.WriteLine("Argument not supported : "+temp);

                        return false;
                    }
                    
                }
            }

            // process
            foreach (object a in args)
            {
                //Console.WriteLine(a);

                // remove special leading string*

                
                // states
                // -M means we are merging
                // -A is appending

                if (a.ToString().ToUpper() == "-M")
                {
                    mode = State.Merge;
                }
                if (a.ToString().ToUpper() == "-A")
                {
                    mode = State.Append;
                }

                if (a.ToString().ToUpper() == "-K")
                {
                    // allows our trigger to work
                    prev = "-K";
                }

                if (a.ToString().ToUpper() == "-F")
                {
                    // allows our trigger to work
                    prev = "-F";
                }

                if (a.ToString().ToUpper() == "-S")
                {
                    // allows our trigger to work
                    prev = "-S";
                }
                if (a.ToString().ToUpper() == "-C")
                {
                    // allows our trigger to work
                    prev = "-C";
                }


                if (prev == "-K")
                {
                    special_String = a.ToString();
                }
                if (prev == "-F")
                {
                    merge_directory = a.ToString();
                }
                if (prev == "-S")
                {
                    folder_supp = a.ToString();
                }
                if (prev == "-C")
                {
                    folder_claims = a.ToString();
                }
            }


            if (mode == State.None)
            {
                Console.WriteLine("Please specify a mode -A or -M");
                return false;
            }

            // check our argument values like -A has the right arguments

            if (!check_mode())
            {
                return false;
            }
           

            return true;
        }
        private bool check_mode ()
        {
            /*
            private string merge_directory;
            private string folder_supp, folder_claims;
            private string special_String;
            private enum State
            {
                None, Merge, Append
            };
            */

            if (mode == State.Merge)
            {
                // need to have merge_dir populated and M

                if (merge_directory.Length == 0)
                {
                    // something not there

                    Console.WriteLine("Wrong parameters");
                    return false;

                }

                if (folder_supp.Length > 0)
                {
                    Console.WriteLine("Wrong parameters");
                    return false;
                }
                if (folder_claims.Length > 0)
                {
                    Console.WriteLine("Wrong parameters");
                    return false;
                }

            }

            if (mode == State.Append)
            {
                if (folder_claims.Length == 0)
                {
                    // something not there

                    Console.WriteLine("Wrong parameters");
                    return false;

                }
                if (folder_supp.Length == 0)
                {
                    // something not there

                    Console.WriteLine("Wrong parameters");
                    return false;

                }
                if (merge_directory.Length > 0)
                {
                    Console.WriteLine("Wrong parameters");
                    return false;
                }


            }




            Console.WriteLine("Running ...\nMode : "+ mode.ToString());
                return true;
        }

        public void debug()
        {
            Console.WriteLine("special character : " + special_String);
            Console.WriteLine("Mode : " + mode.ToString());
            Console.WriteLine("Merge Directory : " + merge_directory);
            Console.WriteLine("folder Supp : " + folder_supp);
            Console.WriteLine("folder claims : " + folder_claims);
        }
        public void print_error()
        {
            Console.WriteLine();
            Console.WriteLine("Format : PDFStitcher -k 'Claimnumber_' -m -f <folder docments to merge>");
            Console.WriteLine("PDFStitcher -k 'Claimnumber_' -A -S <folder of documents to append> -C <append to these>");
            Console.WriteLine();
        }
    }
}
