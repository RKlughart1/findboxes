using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Text;
using System.Drawing.Text;
//Change again1111333344
namespace findboxes
{
    class Program
    {
        static void Main(string[] args)
        {
            
            bool generated = true;
            if (generated == true) {
                Console.WriteLine("Number of Images to generate: ");
                string line1 = Console.ReadLine();
                int numImages = 1;
                Console.WriteLine("Number of lines per image: ");
                string line2 = Console.ReadLine();
                int numlines = 1;

                string font = "";

                if (Int32.TryParse(line1, out numImages) && Int32.TryParse(line2, out numlines))
                {
                    numImages = numImages;
                    numlines = numlines;
                    InstalledFontCollection col = new InstalledFontCollection();
                    ArrayList allFonts = new ArrayList();
                    foreach (FontFamily fa in col.Families)
                    {
                        Console.WriteLine(fa.Name);
                        allFonts.Add((string)fa.Name);
                    }
                    bool flag = false;
                    do
                    {
                        Console.WriteLine("Choose font: ");
                        font = Console.ReadLine();
                        if (allFonts.Contains(font))
                            flag = true;
                        else
                        {
                            Console.WriteLine("font not in list");
                        }

                    } while (flag == false);


                    Console.WriteLine("Starting ...");
                }
                else
                {
                    Console.WriteLine("Cannot convert to int");
                    System.Environment.Exit(0);
                    // cannot parse it as an integer
                }
                bool allChars = false;
                String[] text = new String[numImages];
                int height = 0, width = 0;
                IDictionary<int, Image> images = DrawRandomText(4, 4, new Font(font, 60), numImages, ref text, allChars, ref height, ref width, numlines: numlines);
                //IDictionary<int, Image> images = GetFromFolder(args[0]);
                string allText = "";
                ArrayList PageNumber = new ArrayList();
                ArrayList Heights = new ArrayList();
                //Get all text into one string and merge all individual image dictionaries into one dictionay
                IDictionary<int, int[]> AllBox = PrepareBOXF(images, ref allText, ref PageNumber,ref Heights, text, args);
                SaveBOX(allText, AllBox, PageNumber, Heights, args[2] + "testing.box");
            }
            else
            {
                //bool allChars = false;
                string img1text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-''*!@#$%^&()_+1234567890";
                string img2text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+=-?'':;.,<>1234567890";
                String[] text = new String[4] { img1text , img1text , img2text , img2text };
                //int height = 0, width = 0;
                //IDictionary<int, Image> images = DrawRandomText(4, 4, new Font(font, 60), numImages, ref text, allChars, ref height, ref width, numlines: numlines);
                IDictionary<int, Image> images = GetFromFolder(args[0]);
                string allText = "";
                ArrayList PageNumber = new ArrayList();
                ArrayList Heights = new ArrayList();
                //Get all text into one string and merge all individual image dictionaries into one dictionay
                IDictionary<int, int[]> AllBox = PrepareBOXF(images, ref allText, ref PageNumber,ref Heights, text, args);
                //Console.WriteLine(AllBox.Count + " *");
                //Console.WriteLine(allText.Length + " %");
                SaveBOX(allText, AllBox, PageNumber, Heights, args[2] + "testing.box");
            }
            
         

            Console.WriteLine("Done");
           
        }



     public static IDictionary<int,Image> GetFromFolder(string folder)
        {
            IDictionary<int, Image> images = new Dictionary<int, Image>();
            string[] files = Directory.GetFiles(folder); //, "*ProfileHandler.cs", SearchOption.AllDirectories);
            int counter = 0;
            foreach(string file in files)
            {
                Image img = Bitmap.FromFile(file);
                img = whiteBackground((Bitmap)img);
                images.Add(counter,img);
                counter++;
            }
            return images;
        }

        /// <returns>
        /// AllBox is the same as the other dictionaries but holds the x1,x2,y1,y2 character positions for all images ||
        /// allText is the sequence of characters in order of the images ||
        /// PageNumber is the page number for each character
        /// </returns>
        
       public static IDictionary<int, int[]>  PrepareBOXF(IDictionary<int, Image> images,ref string allText, ref ArrayList PageNumber,ref ArrayList Heights,string[] text,string[] args)
        {
            
            int counter = 0;
            IDictionary<int, int[]> AllBox = new Dictionary<int, int[]>();
            int missingCount = 0;
            foreach (KeyValuePair<int, Image> kvp in images)
            {
                
                
                
                


                IDictionary<int, int[]> Boxess = BoxesForAllChar(kvp.Value);
              
                
                if (text[kvp.Key].Length != Boxess.Count)// && allText.Length>127-36)
                {
                   // Console.WriteLine(text[kvp.Key].Length);
                    //Console.WriteLine(Boxess.Count);
                    Console.WriteLine("Not adding to training");
                    missingCount++;
                    //System.Environment.Exit(0);
                }
                else {
                    allText += text[kvp.Key];
                    Image imgBoxess = DrawBoxes(Boxess, kvp.Value);
                Console.WriteLine(text[kvp.Key].Length);
                Console.WriteLine(Boxess.Count);

                imgBoxess.Save(args[2] + "Verify/boxes" +( kvp.Key-missingCount).ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    //Training for just text
                    //Image imgcirc = MakeGrid(kvp.Value, Color.Black, 3, 5);
                    //imgcirc.Save(args[2] + "grandom_text" + (kvp.Key-missingCount).ToString() + ".tif", System.Drawing.Imaging.ImageFormat.Tiff);

                    kvp.Value.Save(args[2] + "random_text" + (kvp.Key-missingCount).ToString() + ".tif", System.Drawing.Imaging.ImageFormat.Tiff);
                    Console.WriteLine("Page " + (1 + kvp.Key).ToString() + " done");
                    foreach (KeyValuePair<int, int[]> kvp1 in Boxess)
                    {
                        AllBox.Add(kvp1.Key + counter, new int[4] { (int)kvp1.Value.GetValue(0), (int)kvp1.Value.GetValue(1), (int)kvp1.Value.GetValue(2), (int)kvp1.Value.GetValue(3) });
                        PageNumber.Add(kvp.Key-missingCount);
                        Heights.Add(kvp.Value.Height);

                    }
                    counter += Boxess.Count;
                }
              
                //PageSeperator[kvp.Key] = counter;
                
            }
            return AllBox;
        }


        /// <returns> Marks up the image with a horizontal grid depending on text dimensions</returns>
        public static Image MakeGrid(Image img1,Color color, int thickness=1,int sizeIncrease=0)
        {
            Bitmap img = new Bitmap(img1);
            img = whiteBackground(img);
            Bitmap temp = new Bitmap(img);
            bool whiteflag = true;
            ArrayList y1s = new ArrayList();
            ArrayList y2s = new ArrayList();
            int[] box = SingleBox(img);
            int y = box[2];
            do
            {

                for (int x = box[0]; x < box[1]; x++)
                {
                    Color pixel1 = img.GetPixel(x, y);
                    if (pixel1.R != 255 && pixel1.G != 255 && pixel1.B != 255 && whiteflag == true)
                    {
                        whiteflag = false;

                        y1s.Add((int)(y));
                        

                        goto loopend;

                    }
                    if (whiteflag == false)
                    {
                        for (int i = 0; i < img.Width; i++)
                        {
                            Color pixel = img.GetPixel(i, y);
                            if (pixel.R != 255 && pixel.G != 255 && pixel.B != 255)
                            {

                                goto loopend;
                            }
                        }
                        whiteflag = true;
                        y2s.Add((int)y);
                      
                    }
                }
            loopend:
                y++;

            } while (y < box[3]);
            img1 = DrawBox(img1, thickness,color,sizeIncrease);
            y1s.RemoveAt(0);

            Graphics g = Graphics.FromImage(img1);
            Pen pen = new Pen(color, thickness);
            for (int i = 0; i < y1s.Count; i++)
            {
                Point p1 = new Point(box[0]-sizeIncrease, ((int)y1s[i]-(int)y2s[i])/2+(int)y2s[i]);
                Point p2 = new Point(box[1]+sizeIncrease, ((int)y1s[i]-(int)y2s[i])/2+(int)y2s[i]);
                g.DrawLine(pen,p1,p2);
            }

            return img1;
        }

        /// <summary>Marks up all characters found for training (.box file) 
        /// to make sure we found right dimensions</summary>
        public static Image DrawBoxes(IDictionary<int, int[]> boxes, Image img)
        {
            Pen pen = new Pen(Color.Red);
            Image img1 = (Image)img.Clone();
            Graphics g = Graphics.FromImage(img1);
            foreach (KeyValuePair<int, int[]> kvp in boxes)
            {
                g.DrawRectangle(pen,(int)kvp.Value.GetValue(0)-1,  (int)kvp.Value.GetValue(2)-1, (int)kvp.Value.GetValue(1) - (int)kvp.Value.GetValue(0)+1, (int)kvp.Value.GetValue(3) - (int)kvp.Value.GetValue(2)+2);
   
            }
            return img1;
        }

        //draws a single box, mainly for data augmentation
        public static Image DrawBox( Image img, int thickness, Color color,int boxIncrease)
        {
            int[] box = SingleBox(img);
            Pen pen = new Pen(color,thickness);
          
            Graphics g = Graphics.FromImage(img);
           
            g.DrawRectangle(pen, box[0]-boxIncrease, box[2]-boxIncrease, box[1]-box[0]+(2*boxIncrease), box[3]-box[2]+2*boxIncrease);
            return (img);
             
        }

        public static Image DrawCircle(Image img, Color color, int thickness=1, double radiusIncrease=1 )
        {
            int[] box = SingleBox(img);
            Pen pen = new Pen(color, thickness);
            Graphics g = Graphics.FromImage(img);
            float R = ((box[1] - box[0]) / 2) + (float)radiusIncrease;

            g.DrawEllipse(pen,  box[0] - (float)radiusIncrease, (box[3]-box[2])/2+ box[2]-R, R*2, R*2);

            return img;
        }
        //Gets rid of noise in background
        public static Bitmap whiteBackground(Bitmap img)
        {
            for(int x = 0; x<img.Width;x++)
                for(int y = 0; y < img.Height; y++)
                {
                    Color pixel = img.GetPixel(x, y);
                    if(pixel.G>100 && pixel.R >100 && pixel.B > 100) //lots of grey/blue that throws off algorithm
                    {
                        img.SetPixel(x, y, Color.White);
                    }
                }
            return img;
        }



        /// <returns>the x1,x2,y1,y2 for all chars in image the int[] returned is ordered as x1,x2,y1,y2</returns>
        public static IDictionary<int, int[]> BoxesForAllChar(Image img1)
        {
            Bitmap img = new Bitmap(img1);
            
            img = whiteBackground((Bitmap)img);
            Bitmap temp = new Bitmap(img);

            Color background = Color.White;
            bool whiteflag = true;

            ArrayList x1s = new ArrayList();
            ArrayList x2s = new ArrayList();
            ArrayList topLine= new ArrayList();
            ArrayList bottomLine = new ArrayList();
            ArrayList y1s = new ArrayList();
            ArrayList y2s = new ArrayList();

            //finds x1 & x2 edges



            int y = 0;
            do
            {

                for (int x = 0; x < img.Width; x++)
                {
                    Color pixel1 = img.GetPixel(x, y);
                    if (pixel1.R != 255 && pixel1.G != 255 && pixel1.B != 255 && whiteflag == true)
                    {
                        whiteflag = false;

                        topLine.Add((int)(y));
                    

                        goto loopend;
                    }
                    if (whiteflag == false)
                    {
                        for (int i = 0; i < img.Width; i++)
                        {
                            Color pixel = img.GetPixel(i, y);
                            if (pixel.R != 255 && pixel.G != 255 && pixel.B != 255)
                            {

                                goto loopend;
                            }
                        }
                        whiteflag = true;
                        bottomLine.Add((int)y);
                     
                    }



                }
            loopend:
                y++;

            } while (y < img.Height);



 

            //***********************************************
            int[] numCharsPerLine = new int[topLine.Count];
            for (int a = 0; a < topLine.Count; a++)
            {
                int x = 0;
                int counter = 0;
                do
                {

                    for (int b = (int)topLine[a]; b < (int)bottomLine[a]; b++)
                    {
                        Color pixel1 = img.GetPixel(x, b);
                        if (pixel1.R != 255 && pixel1.G != 255 && pixel1.B != 255 && whiteflag == true)
                        {
                            whiteflag = false;

                            x1s.Add((int)(x));
                            counter++;
                         
                            goto loopend;
                        }
                        if (whiteflag == false)
                        {
                            for (int i = (int)topLine[a]; i < (int)bottomLine[a]; i++)
                            {
                                Color pixel = img.GetPixel(x, i);
                                if (pixel.R != 255 && pixel.G != 255 && pixel.B != 255)
                                {

                                    goto loopend;
                                }
                            }
                            whiteflag = true;
                            x2s.Add((int)x);
                         
                        }



                    }
                loopend:
                    x++;

                } while (x < img.Width);

                numCharsPerLine[a] = counter;
                

            }





        for (int a = 0; a < topLine.Count; a++)
        {
            
            //int a1 = 0;
            for (int b = 0; b < numCharsPerLine[a]; b++)
            {

                    int b1=0;
                    for(int k = 0; k < a; k++)
                    {
                        b1 += numCharsPerLine[k];
                    }
                for (int h = (int)topLine[a]; h < (int)bottomLine[a]; h++)
                {
                    for (int w = (int)x1s[b+b1]; w < (int)x2s[b+b1]; w++)
                    {
                        Color pixel = img.GetPixel(w, h);
                        if (pixel.R != 255 && pixel.G != 255 && pixel.B != 255)
                        {
                            y1s.Add((int)h - 1);

                            goto LoopEnd3;
                        }
                    }

                }
            LoopEnd3:
                for (int h = (int)bottomLine[a]; h > (int)topLine[a]; h--)
                {
                    for (int w = (int)x1s[b+b1]; w < (int)x2s[b+b1]; w++)
                    {
                        Color pixel = img.GetPixel(w, h);
                        if (pixel.R != 255 && pixel.G != 255 && pixel.B != 255)
                        {
                            y2s.Add((int)h - 1);

                            goto LoopEnd4;
                        }
                    }

                }
            LoopEnd4:
                bool nothing = true;

                }

                

        }
            
            IDictionary<int, int[]> Boxes = new Dictionary<int, int[]>();
            for (int i = 0; i < x1s.Count; i++)
            {
                Boxes.Add(i, new int[4] { (int)x1s[i], (int)x2s[i], (int)y1s[i], (int)y2s[i] });
            }


            return Boxes;
            
        }
 

        //finds dimensions for single box, more useful for data augmentation than training
        public static int[] SingleBox(Image img1)
        {
            Bitmap img = new Bitmap(img1);
            img = whiteBackground(img);
            

            Color background = img.GetPixel(0, 0);
            Bitmap temp = new Bitmap(img);
            int x1 = 0,x2=0,y1=0,y2=0;
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    if (img.GetPixel(x, y) != background)
                    {
                        x1 = x;
                  
                        goto LoopEnd1;
                    }
                }
            LoopEnd1:
            for (int x = img.Width - 1; x >= x1; x--)
                for (int y = 0; y < img.Height; y++)
                {
                    if (img.GetPixel(x, y) != background)
                    {
                        x2 = x;
                    
                        goto LoopEnd2;
                    }
                }
            LoopEnd2:
            
            
           
            for (int y = img.Height - 1; y >= 0; y--)
                for (int x = x1; x < x2; x++)
                {
                    if (img.GetPixel(x, y) != background)
                    {

                        y2 = y;
                     
                        goto LoopEnd4;
                    }
                }
            LoopEnd4:

            for (int y = 1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    if (img.GetPixel(x, y) != background)
                    {

                        y1 = y;

                        goto loopend3;
                    }
                }
               
            }
            loopend3:

      
            return new int[4] { x1, x2, y1, y2 };
        }

        public static void SaveBOX(String text,IDictionary<int, int[]> Boxes,ArrayList page,ArrayList Heights,string args)
        {
            Console.WriteLine("Number of Characters: "+ text.Length );
            Console.WriteLine("Number of Boxes: "+ Boxes.Count );
/*
            if (text.Length != Boxes.Count)
            {
                Console.Write("Algorithm doesnt work for this font");
                System.Environment.Exit(0);
                    
            }*/
          
                using (StreamWriter writer = new StreamWriter(args))
                {
                    foreach (KeyValuePair<int, int[]> kvp in Boxes)
                    {
                        writer.WriteLine(String.Format
           (
               "{0} {1} {2} {3} {4} {5}",
               text[kvp.Key],
               ((int)kvp.Value.GetValue(0) - 1).ToString(),
               ((int)Heights[kvp.Key] - (int)kvp.Value.GetValue(3)).ToString(),
               ((int)kvp.Value.GetValue(1) + 1).ToString(),
               ((int)Heights[kvp.Key] - (int)kvp.Value.GetValue(2)).ToString(),
               page[kvp.Key]
           ));
                    }
                    writer.Flush();
                }
            
     
        }
        
        /// <summary>Returns a given number of images with random text </summary>
        public static IDictionary<int,Image> DrawRandomText( int minLength, int maxLength, Font fontToUse,int numImages, ref string[] inputText,bool allchars,ref int height,ref int width,int numlines=1)
        {
            IDictionary<int, Image> Images = new Dictionary<int, Image>();
            Bitmap bitmap1 = new Bitmap(1,1);
            Graphics drawing1 = Graphics.FromImage(bitmap1);
            SizeF textSize1 = drawing1.MeasureString("%", fontToUse);
            if (numlines == 1)//using 1 line for drawing circles/boxes.. want the image to be square 
            {
                height = (int)textSize1.Width * maxLength + 10;
                width = (int)textSize1.Width * maxLength + 10;
            }
            else
            {
                height = (int)textSize1.Height * numlines + 20;
                width = (int)textSize1.Width * maxLength + 10;


            }

            int counter = 0;
            for (int k = 0; k < numImages; k++)
            {




               
                Bitmap bitmap = new Bitmap(width, height);
                Brush textBrush = new SolidBrush(Color.Black);
                Graphics drawing = Graphics.FromImage(bitmap);
                drawing.Clear(Color.White);
                int totalChars = 0;
                for (int j = 0; j< numlines; j++)
                {
                    System.Random rng = new Random((int)DateTime.Now.Ticks);
                    //Get Random Text
                    int textLength = rng.Next(minLength, maxLength + 1);
                    StringBuilder builder = new StringBuilder();
                    int charsPerLine = 0;
                    for (int i = 0; i < textLength; i++)
                    {
                        if (allchars)
                        {
                            byte charToUse = (byte)((counter%(127-35)+35));
                            builder.Append(Convert.ToChar(charToUse));
                            counter++;
                            if(counter >= 127 - 35)
                            {
                                allchars = false;
                            }
                        }
                        else {
                            byte charToUse = (byte)rng.Next(36, 127);
                            builder.Append(Convert.ToChar(charToUse));
                        }
                        
                        charsPerLine++;
                        totalChars++;

                    }
                    inputText[k] += builder.ToString();
                    //builder.Append(Environment.NewLine);
                    //Console.WriteLine(inputText[k]);

                    //Draw Random Text


                    SizeF textSize = drawing.MeasureString(inputText[k], fontToUse);
                    
                    if (numlines > 1)
                    {
                        drawing.DrawString(inputText[k].Substring(totalChars - charsPerLine, charsPerLine), fontToUse, textBrush, 10, 10 + textSize.Height * j, StringFormat.GenericTypographic);
                    }
                    else
                    {
                        
                        int startX = bitmap.Width / 2 - ((int)textSize.Width / 2);
                        int starty = bitmap.Height / 2 - ((int)textSize.Height / 2);
                        drawing.DrawString(inputText[k].Substring(totalChars - charsPerLine, charsPerLine), fontToUse, textBrush, startX, starty , StringFormat.GenericTypographic);

                    }


                    // tiff.SaveAdd(bitmap);

                }
                Images.Add(k, (Image)bitmap);

            }

            return Images;
        }


    }
}
