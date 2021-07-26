using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Schema;

namespace AntSimulator
{
    public class Ant
    {
        public List<(TrackPoint tp, Point point)> privateTrackField;
        public (int x, int y) coords;
        public (int x, int y) home;
        public int food;
        public int strength;
        public Field field;
        public (int x, int y) direction;
        public Random rnd;
        public int TrackLifeTime;
        public (int x, int over) RndGeneralProba;
        public (int x, int over) RndHomeProba;
        
        private (int x, int y) prevFoodTrack;
        private (int x, int y) prevpos;
        
        
        private List<(int x, int y)> possiblemoves = GetPossibleMoves();
        

        public Ant(int width, int height, int strength, Field field, Random rnd, int lifetime, (int x, int over) RndGeneralProba, (int x, int over) RndHomeProba)
        {
            privateTrackField = new List<(TrackPoint tp, Point point)>();
            home = (width / 2, height / 2);
            coords = home;
            food = 0;
            this.strength = strength;
            this.field = field;
            this.rnd = rnd;
            TrackLifeTime = lifetime;
            ChangeDir();
            this.RndGeneralProba = RndGeneralProba;
            this.RndHomeProba = RndHomeProba;

        }
        
        private static List<(int, int)> GetPossibleMoves()
        {
            List<(int, int)> list = new List<(int, int)>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    list.Add((i, j));
                }
            }

            return list;
        }

        private bool MoveToFoodTrack()
        {
            if (!(field.field[coords.x, coords.y].track is null) &&
                !(field.field[coords.x, coords.y].track.prevpoint is null))
            {
                
                Point newcos = field.field[coords.x, coords.y].track.prevpoint;
                if (newcos.lifetime <= 0)
                {
                    Console.WriteLine("Bad");
                    throw new Exception("Bad food track");
                }
                
                
                coords = newcos.coords;
                return true;
            }

            List<(int x, int y)> foodTracks = new List<(int x, int y)>();
            foreach ((int x, int y) possiblemove in possiblemoves)
            {
                (int x, int y) coordinates = (coords.x + possiblemove.x, coords.y + possiblemove.y);
                    
                // x coordinates outside of the field
                if (! (coordinates.x < field.size.width && coordinates.x >= 0))
                    continue;
                    
                // y coordinates outside of the field
                if (! (coordinates.y < field.size.width && coordinates.y >= 0))
                    continue;
                    
                // No food here
                if (field.field[coordinates.x, coordinates.y].track is null || field.field[coordinates.x, coordinates.y].track.prevpoint is null)
                    continue;
                    
                if (prevFoodTrack == coordinates && coords != home)
                    continue;
                    
                    
                foodTracks.Add((coordinates.x, coordinates.y));
            }
            

            // No food track arround me
            if (foodTracks.Count == 0)
                return false;
            prevFoodTrack = coords;
            coords = foodTracks[rnd.Next(foodTracks.Count)];
            return true;
        }

        private bool GetProba((int x, int over) proba)
        {
            return proba.x > rnd.Next(proba.over);
        }
        
        private bool MoveToFood()
        {
            foreach ((int x, int y) possiblemove in possiblemoves)
            {   
                (int x, int y) coordinates =(coords.x + possiblemove.x, coords.y + possiblemove.y);
                    
                // x coordinates outside of the field
                if (! (coordinates.x < field.size.width && coordinates.x >= 0))
                    continue;
                    
                // y coordinates outside of the field
                if (! (coordinates.y < field.size.height && coordinates.y >= 0))
                    continue;
                    
                // No food here
                if (field.field[coordinates.x, coordinates.y].food == 0)
                    continue;

                coords = coordinates;
                return true;
            }
            return false;
        }

        private void TakeFood()
        {
            if (field.field[coords.x, coords.y].food != 0)
            {
                if (field.field[coords.x, coords.y].food > strength)
                {
                    food = strength;
                    field.field[coords.x, coords.y].food -= strength;
                }
                else
                {
                    food = field.field[coords.x, coords.y].food;
                    field.field[coords.x, coords.y].food = 0;
                }
            }
            CreateMyTrack();
        }

        private void CreateMyTrack()
        {
            if (field.field[coords.x, coords.y].track is null)
            {
                field.field[coords.x, coords.y].track = new TrackPoint(coords, rnd, TrackLifeTime);
                field.field[coords.x, coords.y].track.lifetime = TrackLifeTime;
            }
            
            Point thapoint = field.field[coords.x, coords.y].track.Add(prevpos);
            privateTrackField.Add((field.field[coords.x, coords.y].track, thapoint));
        }
        
        private void UpdateMyTrack()
        {
            List<(TrackPoint tp, Point point)> tpToDelete = new List<(TrackPoint tp, Point point)>();
            foreach ((TrackPoint tp, Point point) tp in privateTrackField)
            {
                if (!tp.tp.Update(tp.point))
                    tpToDelete.Add(tp);
            }

            foreach ((TrackPoint tp, Point point) tp in tpToDelete)
            {
                privateTrackField.Remove(tp);
            }
        }
        
        private void ChangeDir()
        {
            direction = (rnd.Next(field.size.width), rnd.Next(field.size.height));
            //Console.WriteLine(direction);   //----------------------------------------------------------
        }

        private void DropFood()
        {
            //Console.WriteLine("Dropped food");  //------------------------------------------------------
            food = 0;
        }

        private void MoveSomewhere((int x, int y) dir)
        {
            if (coords.x < dir.x)
                coords.x += 1;
            else if (coords.x > dir.x)
                coords.x -= 1;

            if (coords.y < dir.y)
                coords.y += 1;
            else if (coords.y > dir.y)
                coords.y -= 1;
        }
        
        
        private void MoveWithFood()
        {
            if (GetProba(RndHomeProba))
                MoveRandom();
            else
                MoveSomewhere(home);

            CreateMyTrack();
        }

        private void MoveRandom()
        {
            (int x, int y) delta;
            do
            {
                delta = possiblemoves[rnd.Next(possiblemoves.Count)];
            } while (! field.InField(delta.x + coords.x, delta.y + coords.y));
             
            coords.x += delta.x;
            coords.y += delta.y;
        }
        
        public void UpdateMove()
        {
            UpdateMyTrack();
            
            prevpos = coords;
            
            // Not carrying food and looking for it
            if (food == 0 && MoveToFood())
            {
                TakeFood();
                //Console.WriteLine("Took food");   //------------------------------------------------------------------
                return;
            }
            
            
            

            if (food != 0)
            {
                if (home == coords)
                    DropFood();
                else
                    MoveWithFood();
            }
            // Not carrying food and looking for track
            if (food == 0 && MoveToFoodTrack())
            {
                //Console.WriteLine($"On my food track, after, have to go : {direction}"); //---------------------------------------------------
            }
            else if (food == 0)
            {
                if (coords == direction)
                    ChangeDir();
                if(GetProba(RndGeneralProba))
                    MoveRandom();
                else
                    MoveSomewhere(direction);
                
            }
                
            
            //Console.WriteLine("One good update");
        }
    }
}