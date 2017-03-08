using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;

namespace NetworkRouting
{

    internal class data
    {
        //Internal class to keep track of the data of each point
        //This is necessary becuase the data in queue gets mixed around
        public PointF point;
        public double dist;
        public int location;
        public int original;

        public data(PointF point, double dist, int location, int original)
        {
            this.point = point;
            this.dist = dist;
            this.location = location;
            this.original = original;
        }
    }
    internal class DijkstraSolver
    {
        private List<PointF> points = new List<PointF>();
        private Graphics graphics;
        private List<HashSet<int>> adjacencyList;
        private int startNodeIndex;
        private int stopNodeIndex;
        private Dictionary<PointF, PointF> final;

        Pen b_pen = new Pen(Color.FromArgb(255, 0, 0, 255));

        public DijkstraSolver(List<PointF> points, Graphics graphics, List<HashSet<int>> adjacencyList, 
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
            // Over all we have O(|V|log|V|+|V|log|V|+|E|log|V|) which
            // reduces to O((E+V)log|V|).
            List<PointF> shortest = new List<PointF>();
            List<data> queue = new List<data>();
            List<double> distances = new List<double>();
            List<int> possible = new List<int>();
            data[] match = new data[points.Count];
            
            // Make queue step. Takes O(|V|log(|V|)) because insert takes 
            // O(log|V|) and we are inserting |V| nodes.
            for (int i = 0; i< points.Count;i++) // O(|V|)
            {
                possible.Add(i);
                if (i == startNodeIndex)
                {
                    distances.Add(0);
                    data temp = new data(points[i], 0, i, i);
                    match[i] = temp;
                    insert(ref queue, ref temp); // O(log|V|)
                }
                else
                {
                    distances.Add(double.MaxValue);
                    data temp = new data(points[i], double.MaxValue, i, i);
                    match[i] = temp;
                    insert(ref queue, ref temp); // O(log|V|)
                }
            }

            while(queue.Count > 0) 
            {
                // This while loop takes O(|V|) because we pop one node off
                // the queue at a time.

                data u = deletemin(ref queue); // O(log|V|)
                possible[u.original] = -1;
                foreach (int v in adjacencyList[u.original])
                {
                    // This step can take at worst O(|E|).
                    if (possible[v] == -1) continue;


                    double e_dist = eucl_dist(points[v], u.point);
                    if (distances[v] > u.dist + e_dist)
                    {
                        int i_v = match[v].location;
                        if (i_v < 0) continue;

                        queue[i_v].dist = u.dist + e_dist;
                        distances[v] = u.dist + e_dist;
                        if (final.ContainsKey(queue[i_v].point))
                        {
                            final[queue[i_v].point] = u.point;
                        }
                        else 
                        {
                            final.Add(queue[i_v].point, u.point);
                        }
                        decrease_key(ref queue, i_v); // O(log|V|)
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
            catch(KeyNotFoundException e)
            {
                return new List<PointF>();
            }

            return shortest;
        }

        public void decrease_key(ref List<data> queue, int index)
        {
            // The ref of queue should already have the value decreased.
            // This function moves the value up if the value at the index 
            // specificed is lower.
            // Time complecity is a order of O(log|V|).
            int index_mu = index + 1;
            if ((index / 2) - 1 < 0) return;
            if(queue[(index_mu / 2)-1].dist > queue[index_mu - 1].dist)
            {
                move_up_queue(ref queue, index_mu);
                // See function call for time complexity [O(log(|V|)]. 
            }
        }

        
        public void move_up_queue(ref List<data> queue, int start_i)
        {
            // This function is bubble up a value in binary heap.
            // This is used by insert and decrease_key functions.
            // The time complexity is one O(log|V|) because the value, at
            // worst case, only moves up the queue's height which is only
            // log|V|. Therefore the time complexity is only O(log|V|). 
            data value = queue[start_i - 1];
            int index = start_i;
            while (true)
            {
                if (queue[(index / 2) - 1].dist < value.dist) break;
                else
                {
                    data temp = queue[(index / 2) - 1];
                    queue[(index / 2) - 1] = value;
                    queue[index - 1] = temp;
                    queue[(index / 2) - 1].location = (index / 2) - 1;
                    queue[index - 1].location = index - 1;
                    index = (index / 2);
                    
                }
                if (index <= 1) break;
            }
            
        }

        public data deletemin(ref List<data> queue)
        {
            // This function allows user to delete minimum from dinary heap.
            // Time complecity is a order of O(log|V|).
            // This is proven by the fact that a binary heap bubbles down
            // a value from a heap which is only log|V| deap.
            if (queue.Count <= 1)
            {
                data temp = queue[0];
                queue.RemoveAt(0);
                return temp;
            }
            data min = queue[0];
            data last = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);
            queue[0] = last;
            queue[0].location = 0;
            int index = 1;
            int index_n;
            while (true)
            {

                if ((2 * index) - 1 >= queue.Count) break;
                data child;
                data left = queue[(2 * index) - 1];

                if ((2 * index + 1) - 1 >= queue.Count)
                {
                    child = left;
                    index_n = 2 * index;
                }
                else
                {
                    data right = queue[(2 * index + 1) - 1];

                    if (left.dist <= right.dist)
                    {
                        child = left;
                        index_n = 2 * index;
                    }
                    else
                    {
                        child = right;
                        index_n = 2 * index + 1;
                    }
                }
                if (child.dist < queue[index - 1].dist)
                {
                    data temp = queue[index_n - 1];
                    queue[index_n - 1] = queue[index - 1];
                    queue[index - 1] = temp;
                    queue[index - 1].location = index - 1;
                    queue[index_n - 1].location = index_n - 1;

                    index = index_n;
                    
                }
                else break;

            }
            
            return min;
        }

        public void insert(ref List<data> queue, ref data value)
        {
            // Function to insert a value into the queue.
            // Overall complexity of O(log|V|).
            queue.Add(value);
            int index = queue.Count;
            if (queue.Count == 1) return;
            if(queue.Count == 2)
            {
                if(queue[1].dist < queue[0].dist)
                {
                    data temp = queue[1];
                    queue[1] = queue[0];
                    queue[0] = temp;
                }
                return;
            }
            move_up_queue(ref queue, index); 
            // See function call for time complexity [O(log(|V|)]. 

        }

        public double eucl_dist(PointF one, PointF two)
        {
            //Simple function to calculate euclidean distance.
            return Math.Pow(Math.Pow((one.Y - two.Y), 2) + Math.Pow((one.X - two.X), 2),0.5);
        }
        public void draw(PointF one, PointF two)
        {
            // Helper Function to draw lines
            graphics.DrawLine(b_pen, one.X, one.Y, two.X, two.Y);
        }
    }
}