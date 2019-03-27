# PDF-Stitcher
Command line tool



To merge:

Merging will combine any supporting documents that have a doc_XX then place them into a merged folder.
All other documents will be renamed to be in the format doc_XX.pdf and also be moved to
the merged folder

PDFStitcherConsole.exe -M -F "C:\Users\ksingh\Desktop\Current Work\Newfolder\supp" 

To Append:

Append will merge the claim pdf with the corresponding supporting document pdf. This is based off of if file name MATCHES

PDFStitcherConsole.exe  -A -S "<additional doc>" -C "<main doc>"


** I will create a bat script that will make the command line arguments easier to use **
