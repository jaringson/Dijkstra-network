using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NetworkRouting
{
    
    
    public class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public Pair() { }
        public Pair(T1 x,T2 y)
        {
            First = x;
            Second = y;
        }
    }
    
    internal class ArraySolver
    {
        private List<PointF> points = new List<PointF>();
        private Graphics graphics;
        private List<HashSet<int>> adjacencyList;
        private int startNodeIndex;
        private int stopNodeIndex;
        private Dictionary<PointF, PointF> final;

        Pen b_pen = new Pen(Color.FromArgb(255, 0, 0, 255));

        public ArraySolver(List<PointF> points, Graphics graphics, List<HashSet<int>> adjacencyList, 
                            int startNodeIndex, int stopNodeIndex)
        {
            this.points = points;
            this.graphics = graphics;
            this.adjacencyList = adjacencyList;
            this.startNodeIndex = startNodeIndex;
            this.stopNodeIndex = stopNodeIndex;
            final = new Dictionary<PointF, PointF>();

        }
        public List<PointF> solve()
        {
            // This is the solving function.
            // Over all we have O(|V|+|V|^2+|E|) which
            // reduces to O(|V|^2).

            List<int> indexes = new List<int>();
            List<PointF> shortest = new List<PointF>();
            List<Pair<PointF, double>> queue = new List<Pair<PointF, double>>();

            // Make queue step. Takes O(|V|) because insert takes 
            // O(1) and we are inserting |V| nodes.
            for (int i = 0; i < points.Count; i++) // O(|V|)
            {
                if (i == startNodeIndex)
                {
                    int index = i;
                    indexes.Add(index); // O(1)
                    queue.Add(new Pair<PointF, double>(points[i],0)); 
                }
                else
                {
                    int index = i;
                    indexes.Add(index); // O(1)
                    queue.Add(new Pair<PointF, double>(points[i], double.MaxValue)); 
                }
            }

            while (queue.Count > 0)
            {
                // This while loop takes O(|V|) because we pop one node off
                // the queue at a time.

                double smallest = double.MaxValue;
                Pair<PointF, double> u = new Pair<PointF, double>();

                // This represents the delete min function in Dijsktra's
                // The time complexity is O(|V|) becuase you need to check
                // every point for the lowest value.
                for (int i = 0; i < queue.Count; i++) 
                {
                    if(queue[i].Second < smallest)
                    {
                        smallest = queue[i].Second;
                        u = queue[i];
                    }
                }
                queue.Remove(u);
                int index = -1;
                for (int i = 0; i < points.Count; i++)
                {
                    if(points[i].X == u.First.X && points[i].Y == u.First.Y)
                    {
                        index = i;
                    }
                }
                if(index ==-1)
                {
                    u = queue[0];
                    queue.Remove(u);
                    index = 0;
                }

                foreach (int v in adjacencyList[index])
                {
                    int i_v = find_v(queue, points[v]);
                    if (i_v == -1) continue;

                    double dist = eucl_dist(queue[i_v].First, u.First);
                    if(queue[i_v].Second > u.Second+ dist)
                    {
                        queue[i_v].Second = u.Second + dist;
                        // Represents decrease key in O(1) time

                        if (final.ContainsKey(queue[i_v].First))
                        {
                            final[queue[i_v].First] = u.First;
                        }
                        else
                        {
                            final.Add(queue[i_v].First, u.First);
                        }
                    }
                    
                }

            }

            try
            {
                PointF start = points[stopNodeIndex];
                PointF end = final[points[stopNodeIndex]];

                shortest.Add(start);
                shortest.Add(end);

                while (end.X != points[startNodeIndex].X && end.Y != points[startNodeIndex].Y)
                {
                    start = end;
                    end = final[start];
                    shortest.Add(end);
                }
            }
            catch (KeyNotFoundException e)
            {
                return new List<PointF>();
            }

            return shortest;
        }
        public int find_v(List<Pair<PointF, double>> queue, PointF point)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].First.X == point.X && queue[i].First.Y == point.Y)
                {
                    return i;
                }
            }
            return -1;
        }
        public double eucl_dist(PointF one, PointF two)
        {
            return Math.Pow(Math.Pow((one.Y - two.Y), 2) + Math.Pow((one.X - two.X), 2), 0.5);
        }
        public void draw(PointF one, PointF two)
        {
            graphics.DrawLine(b_pen, one.X, one.Y, two.X, two.Y);
        }

    }
}
