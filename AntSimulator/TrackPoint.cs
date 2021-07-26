using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimulator
{
    public class TrackPoint
    {
        public int lifetime;
        public Point prevpoint => GetPrevPoint(); 
        public List<Point> prevpoints;
        public int Time => TimePoint.lifetime;
        public Point TimePoint;
        
        
        public (int x, int y) currPoint;
        private Random rnd;

        public TrackPoint((int, int) coords, Random rnd, int lifetime)
        {
            this.rnd = rnd;
            prevpoints = new List<Point>();
            currPoint = coords;
            this.lifetime = lifetime;
            TimePoint = null;
        }

        public Point Add((int x, int y) coords)
        {
            Point thaPoint;
            if (!prevpoints.Exists(point => point.coords == coords))
            {
                thaPoint = new Point(coords, lifetime);
                prevpoints.Add(thaPoint);
            }
            else
            {
                thaPoint =  prevpoints.Find(point => point.coords == coords);
                thaPoint.lifetime = lifetime;
            }
            TimePoint = thaPoint;
            return thaPoint;
        }

        private Point GetPrevPoint()
        {
            if (prevpoints.Count == 0)
                return null;
            
            Point mpoint = prevpoints[rnd.Next(prevpoints.Count)];
            if (mpoint.lifetime <= 0)
                Console.WriteLine("Bad point");
            return mpoint;
        }
        public bool Update(Point point)
        {
            if (!prevpoints.Contains(point))
                return false;
            
            
            point.lifetime -= 1;
            if (point.lifetime <= 0)
            {
                prevpoints.Remove(point);
                return false;
            }
            
            return true;
        }
        
    }

    public class Point
    {
        public (int x, int y) coords;
        public int lifetime;

        public Point((int x, int y) coords, int lifetime)
        {
            this.coords = coords;
            this.lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"({coords.x}, {coords.y} {lifetime})";
        }
    }
}