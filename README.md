MsCompoundDocumentWrapper
=================
This is a port from my old blog.

I had to read the contents of several Microsoft Compound Documents from disk. After doing some searching I could not find any .NET implementations to tackle this for me so I decided to build a library in C# and publish it for everyone else to use. During my searching I did come across a great document from OpenOffice.org which details the structure of Microsoft Compound Document File Format with extreme clarity and accuracy. Using this document I created a C# library which can read from an input stream the Compound Document and return a set of directory objects which contain the meta-data and the underlying stream object which contains the data that was stored inside the compound document.

The code is self-explanatory and I also added documentation in the code. Together with the documentation I believe any .NET developer should be able to easily reuse this library. Please double check your results, I did not have as much time as I would have liked to thoroughly test the output. Also I did not implement reading of the time stamps in the directory entries.


For more information about Microsoft's Compound Document visit these sites.
* [Wiki site](http://en.wikipedia.org/wiki/Compound_File_Binary_Format)
* [OpenOffice.org's Documentation of the Microsoft Compound Document File Format](http://sc.openoffice.org/compdocfileformat.pdf)
* [Windows Compound Binary File Format Specification](http://download.microsoft.com/download/0/B/E/0BE8BDD7-E5E8-422A-ABFD-4342ED7AD886/WindowsCompoundBinaryFileFormatSpecification.pdf)