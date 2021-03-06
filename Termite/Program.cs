﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Extract_Blocks
{
    class Program
    {
        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
       
        private static bool GetBoolean(string input)
        {
            return (input.ToLower() == "true");
        }

        static void Main(string[] args)
        {
            StreamWriter sw = new StreamWriter(@".\Files.txt");
            sw.AutoFlush = true;
            Console.SetOut(sw);
            // use cache extractSWF();

            string[] itemIds = File.ReadAllLines(@"decompiled/scripts/items/ItemId.as");

            var variables = new Dictionary<string, int>();
            foreach (string lineX in itemIds)
            {
                var lineTrimmed = lineX.TrimStart();
                var lineWords = lineTrimmed.Split(Convert.ToChar(" "));

                

                if ((lineWords[0] == "public") || (lineWords[0] == "private") || (lineWords[0] == "protected"))
                {
                    // could be a class, interface or variable
                    int i = 0;
                    if (lineWords[1] == "static")
                    {
                        i++;
                    }

                    if (lineWords[1+i] == "const")
                    {
                        i++;
                    } else
                    {
                        if (lineWords[1+i] == "function")
                        {
                            /// do function stuff
                        }
                    }

                    if (lineWords[1+i].Contains(":"))
                    {
                        // this is a variable
                        i++;
                        var variable = lineWords[i];
                        var variableClean = (lineWords[i].Split(Convert.ToChar(":")))[0];
                        var val = lineWords[i + 2];
                        variables.Add(variableClean, Convert.ToInt32(val.Replace(";", "")));
                        //Console.WriteLine(variableClean + " = " + val.Substring(0,(val.Length - 2)));
                    }

                }

                //Console.WriteLine(lineX);
            }


            Dictionary<int, string> verification = new Dictionary<int, string>();
            var blocks = new List<Block>();
            string[] lines = File.ReadAllLines(@"decompiled/scripts/items/ItemManager.as");

            string curPackage = "";
            foreach (string line in lines)
            {
                if (line.Contains("_loc") && line.Contains(","))
                {
                    if (line.Contains("addBrick") || line.Contains("ItemBrickPackage"))
                    {
                        if (line.Contains("addBrick"))
                        {
                            //ItemManager.as
                            var parts = line.Split(Convert.ToChar(","));
                            Block block = new Block();
                            block.id = parts[0];
                            block.layer = parts[1];
                            block.bmd = parts[2];
                            block.shop_name = parts[3];
                            block.description = parts[4];
                            block.item_tab = parts[5];
                            block.bool_1 = GetBoolean(parts[6]);
                            block.bool_2 = GetBoolean(parts[7]); ;
                            block.sprite_offset = GetNumbers(parts[8]);
                            try
                            {
                                block.color = parts[9].Replace("));", "");
                                //http://stackoverflow.com/questions/17587588/
                                block.color = Convert.ToString(
                                    Decimal.Parse(
                                        block.color,
                                        NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));


                            }
                            catch (Exception)
                            {
                                block.color = "0"; // just black (empty)
                            }

                            Color color = UIntToColor(Convert.ToUInt32(block.color));
                            var hex = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

                            block.color = hex;
                            block.block_package = curPackage;
                            blocks.Add(block);
                        }
                        else
                        {
                            string[] parts = line.Split(Convert.ToChar("("));
                            parts = parts[1].Split(Convert.ToChar(","));

                            string parts_final = Convert.ToString(parts[0]).Replace("\"", "");
                            curPackage = parts_final;
                        }
                    }
                }

                lines = System.IO.File.ReadAllLines(@"decompiled/scripts/items/ItemId.as");
                /*var layerConverterDict = new Dictionary<string, int>();
                foreach (string pkg_line in lines)
                {
                    if (pkg_line.Contains("public static const") && pkg_line.Contains("int"))
                    {
                        var the_line = pkg_line.Replace("public static const ", "");

                        var package_and_id = (the_line.Split(Convert.ToChar("=")));
                        var package = package_and_id[0].Replace(":int", ""); // GLOWY_LINE_BLUE_STRAIGHT
                        var id = GetNumbers(package_and_id[1]); // 376

                        layerConverterDict.Add(package.Replace(" ", ""), Convert.ToInt32(id));
                    }
                }*/

                foreach (var block in blocks)
                {
                    // do id resolution (ItemId.* to int) (ItemId.as)
                    if (block.id.Contains("ItemId."))
                    {
                        var id = block.id;
                        var sample = id.Split(Convert.ToChar("("))[2];
                        id = sample.Replace("ItemId.", "");
                        try
                        {
                            block.id = Convert.ToString(variables[id]);
                        }
                        catch (Exception e)
                        {
                        }

                    }
                }



                foreach (var block in blocks)
                {
                    if (block.id.Contains("addBrick"))
                    {
                        block.id = block.id.Split(Convert.ToChar("("))[2];
                    }

                    // verification (debugging only)
                    //EnsureInterblockConsistency(verification, block);

                    // sample code
                    /*if (block.id == "182")
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(block));
                    }*/
                }
            }

            Console.WriteLine(JsonConvert.SerializeObject(blocks));
            Console.ReadKey(false);
        }

        private static void extractSWF()
        {
            // https://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput.aspx
            Process process = new Process();
            process.StartInfo.FileName = "java.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = "-jar decompiler/ffdec.jar -export script decompiled/ freegame.swf";
            process.Start();

            // Synchronously read the standard output of the spawned process. 
            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();

            // Write the redirected output to this application's window.
            Console.WriteLine(output);

            process.WaitForExit();
            process.Close();
        }

        private static void EnsureInterblockConsistency(Dictionary<int, string> verification, Block block)
        {
            string value = null;
            verification.TryGetValue(Convert.ToInt32(block.id), out value);

            if (value != null)
            {
                if (!(value == block.color))
                {
                    Console.WriteLine("ERROR: " + block.color + " and " + value + " do not match.");
                }
            }
            else
            {
                verification.Add(Convert.ToInt32(block.id), block.color);
            }
        }
    }
}
