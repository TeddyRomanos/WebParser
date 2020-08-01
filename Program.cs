using System;
using System.IO;
using System.Net.Http;
using System.Linq;

namespace WebParser
{
    class Program
    {
        static void Main (string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                var client = new HttpClient();

                var website = "https://www.bensound.com/";
                var pathToParse = "royalty-free-music/cinematic";
                var htmlAtStart = @"href=""";
                var htmlAtEnd = @""" class=""bouton_pop"">Download</a>";
                var currDir = Directory.GetCurrentDirectory() + "\\";
                var bypassString = string.Empty;
                var filesAreAtRoot = true;
                if(args?.Count() > 3){
                    website = args[0];
                    pathToParse = args[1];
                    htmlAtStart = args[2];
                    htmlAtEnd = args[3];
                    if(args.Count() > 4)
                        currDir = args[4];
                }else{
                    Console.WriteLine("Welcome to WebParser!" + Environment.NewLine +
                        "We hope to help you parse a website." + Environment.NewLine + 
                        "The end result should be downloading the files you need from that website." + Environment.NewLine
                    );
                    Console.WriteLine("Lets start!");
                    Console.WriteLine("Enter the website URL: example (" + website +")");
                    website = Console.ReadLine();
                    Console.WriteLine("Enter the website path to parse: example (" + pathToParse +")");
                    pathToParse = Console.ReadLine();
                    Console.WriteLine("Enter the start HTML to look for: example (" + htmlAtStart + ")");
                    htmlAtStart = Console.ReadLine();
                    Console.WriteLine("Enter the end HTML to look for: example (" + htmlAtEnd + ")");
                    htmlAtEnd = Console.ReadLine();
                    Console.WriteLine("Enter what to bypass: example (Parent Directory)");
                    bypassString = Console.ReadLine();
                    Console.WriteLine("Files are at root: (y or n)");
                    filesAreAtRoot  = Console.ReadLine().ToLower() == "y";
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[INFO] Reading website: " + website + pathToParse); 

                var res =  client.GetAsync(website + pathToParse).Result;
                var html = res.Content.ReadAsStringAsync().Result;
                var sections = html.Split(new string[] {htmlAtEnd}, StringSplitOptions.None).ToList();
                Console.WriteLine("[INFO] Reading website done."); 
                foreach(var section in sections)
                {
                    var downloadLink = website;               
                    try
                    {
                        downloadLink = website +  (filesAreAtRoot ? string.Empty : pathToParse) + section.Substring(section.LastIndexOf(htmlAtStart) + htmlAtStart.Length);
                        if(!string.IsNullOrEmpty(bypassString) && downloadLink.Contains(bypassString))
                        {   
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[WARNING] DownloadLink: " + downloadLink + "Bypassed!"); 
                            continue;
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[INFO] Downloading file at: " + downloadLink); 

                        var fileName = downloadLink.Substring(downloadLink.LastIndexOf("/") + 1);
                        var fileMessage = client.GetAsync(downloadLink).Result;
                        var fileBytes = fileMessage.Content.ReadAsByteArrayAsync().Result;
                        File.WriteAllBytes(currDir +fileName,fileBytes);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[INFO] File downloaded: " + currDir +fileName); 
                    }
                    catch(Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] Error downloading: " + downloadLink + Environment.NewLine + "Exception: " + ex.ToString()); 
                    }
                }
            }
            catch(Exception ex){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Fatal Exception: " + ex.ToString());

            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[INFO] Done"); 
            Console.Read();

        }
    }
}
