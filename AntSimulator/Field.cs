using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Schema;

namespace AntSimulator
{
    public class Field
    {
        public (int food, TrackPoint track)[,] field;
        private List<Ant> Ants;
        public (int width, int height) size;
        private string imagesPath;
        private int maxFood;
        private Random rnd;
        private int scale;
        private int tracklifetime;

        private int i;
        


        public Field(int width, int height, int nbAnts, bool randomStrength, int strength, string imagesPath, int maxFood, int scale, int tracklifetime, (int x, int over) RndGeneralProba, (int x, int over) RndHomeProba)
        {
            rnd = new Random();
            field = new (int, TrackPoint)[width,height];
            Ants = new List<Ant>();
            size = (width, height);
            this.imagesPath = imagesPath;
            this.maxFood = maxFood;
            this.scale = scale;
            this.tracklifetime = tracklifetime;
            for (int i = 0; i < nbAnts; i++)
            {
                if(randomStrength)
                    strength = rnd.Next(strength);
                
                Ants.Add(new Ant(width, height, strength, this, rnd, tracklifetime, RndGeneralProba, RndHomeProba));
            }
            int maxi = 0;
        }
       
        
        
        public void Update(int maxi)
        {

            while (i <= maxi)
            {
                if (i % 100 == 0)
                    Console.WriteLine(i);
                PrintField(i);
                
                foreach (Ant ant in Ants)
                {
                    ant.UpdateMove();
                }

                i += 1;
            }
            
        }
        public void CreateFoodPoint(int x, int y, int range)
        {
            for (int i = -range; i < range; i++)
            {
                for (int j = -range; j < range; j++)
                {
                    (int newX, int newY) = (x + i, y + j);
                    int hypo = i*i + j*j;
                    if (Math.Sqrt(hypo) <= range && InField(newX, newY))
                        field[newX, newY].food = maxFood;
                }
            }
            
        }

        public bool InField(int x, int y)
        {
            if (! (x < size.width && x >= 0))
                return false;
                    
            // y coordinates outside of the field
            if (! (y < size.height && y >= 0))
                return false;

            return true;

        }

        public void PrintField(int i)
        {
            Bitmap map = new Bitmap(size.width, size.height);
            for (int x = 0; x < size.width; x++)
            {
                for (int y = 0; y < size.height; y++)
                {
                    
                    if (field[x, y].food != 0)
                        map.SetPixel(x,y,Color.FromArgb(0, field[x,y].food*10, 0));
                    else if (!(field[x, y].track is null) && !(field[x, y].track.prevpoint is null))
                    {
                        int max = field[x, y].track.Time*255/tracklifetime;
                        map.SetPixel(x, y, Color.FromArgb(42, max, max));
                    } else
                        map.SetPixel(x,y,Color.FromArgb(7, 42, 69));
                }
            }
            

            foreach (Ant ant in Ants)
            {   
                map.SetPixel(ant.coords.x,ant.coords.y,Color.FromArgb(211, 23,7));
            }

            map = Upscale(map, scale);

            string prefix = "Image ";
            map.Save(imagesPath + prefix + i + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private Bitmap Upscale(Bitmap map, int scale)
        {
            Bitmap newmap = new Bitmap(map.Width * scale, map.Height * scale);
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Color color = map.GetPixel(x, y);
                    for (int i = 0; i < scale; i++)
                    {
                        for (int j = 0; j < scale; j++)
                        {
                            newmap.SetPixel(scale*x + i, scale*y + j, color);
                        }
                    }
                }
            }

            return newmap;
        }
    }
}