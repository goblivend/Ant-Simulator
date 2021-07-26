using System;
using System.IO;
using System.Xml.Schema;
using System.Diagnostics;

namespace AntSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
           
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int scale = 1;
                int width = 300/scale;
                int height = 300/scale;
                Random rnd = new Random();
                Field field = new Field(width, height, 1000, false, 10, "../../../../../Images/", 20, scale, width+height, (1, 5), (1, 2));
                field.CreateFoodPoint(rnd.Next(width), rnd.Next(height), rnd.Next(50));
                field.CreateFoodPoint(rnd.Next(width), rnd.Next(height), rnd.Next(50));
                field.CreateFoodPoint(rnd.Next(width), rnd.Next(height), rnd.Next(50));
                field.Update(10000);
            /*int i = 3000;
            do
            {
                field.Update(i);
            } while (int.TryParse(Console.ReadLine(), out i));*/
            //Console.WriteLine(sw.ElapsedMilliseconds);
            // 375587 : optimised track
            // 381954 : rnd + optimised update
            // 350941 : tracktime : 300 => 400 + less console
            
        } 
    }
}