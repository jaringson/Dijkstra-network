using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;

namespace NetworkRouting
{

    internal unsafe class data
    {

        public PointF point;
        public double dist;
        public HashSet<int> edges;
        public int location;

        public data(PointF point, double dist, HashSet<int> edges, int location)
        {
            this.point = point;
            this.dist = dist;
            this.edges = edges;
            this.location = location;
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

        public DijkstraSolver(List<PointF> points, Graphics graphics, List<HashSet<int>> adjacencyList, int startNodeIndex, int stopNodeIndex)
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
            List<PointF> shortest = new List<PointF>();
            List<data> queue = new List<data>();
            List<double> distances = new List<double>();
            List<int> possible = new List<int>();
            //List<PointF> prev = new List<PointF>();


            for (int i = 0; i< points.Count;i++)
            {
                possible.Add(i);
                if (i == startNodeIndex)
                {
                    distances[i] = 0;
                    insert(ref queue, new data(points[i], 0, adjacencyList[i],i));
                }
                else
                {
                    distances[i] = double.MaxValue;
                    insert(ref queue, new data(points[i], double.MaxValue, adjacencyList[i],i));
                }
            }
            while(queue.Count > 0)
            {
                data u = deletemin(ref queue);
                possible.Remove(u.location);
                foreach (int v in u.edges)
                {
                    if (!possible.Contains(v)) continue;

                    int i_v = find_v(queue, distances[v], 1);

                    double e_dist = eucl_dist(queue[i_v].point, u.point);
                    if (queue[i_v].dist > u.dist + e_dist)
                    {
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
                        decrease_key(ref queue, i_v);
                    }
                }
            }
            PointF start = points[stopNodeIndex];
            PointF end = final[points[stopNodeIndex]];

            while (end.X != points[startNodeIndex].X && end.Y != points[startNodeIndex].Y)
            {
                draw(start, end);
                start = end;
                end = final[start];
            }
            draw(start, end);


            /*insert(ref queue, new data(new PointF(), 2, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 4, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 3, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 9, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 11, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 8, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 7, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 1, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 5, new HashSet<int>(), new PointF()));
            insert(ref queue, new data(new PointF(), 10, new HashSet<int>(), new PointF()));

            Console.WriteLine();
            Console.WriteLine(deletemin(ref queue).dist);
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine(deletemin(ref queue).dist);
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine(deletemin(ref queue).dist);
            Console.WriteLine();

            decrease_key(ref queue, 6.5,5);*/

            int index = 0;
            foreach (data i in queue)
            {
                index++;
                Console.WriteLine(i.dist);
                if (index == 100) break;
            }

    

            return shortest;
        }
        public void decrease_key(ref List<data> queue, int index)
        {
            //queue[index].dist = new_value;
            int index_mu = index + 1;
            if ((index / 2) - 1 < 0) return;
            if(queue[(index_mu / 2)-1].dist > queue[index_mu - 1].dist)
            {
                move_up_queue(ref queue, index_mu);
            }
        }
        public int find_v(List<data> queue, double distance, int index)
        {
            double value = -1;
            if(queue[index-1].dist == distance)
            {
                return index;
            }
            if(2*index > queue.Count)
            if(queue[(2*index) - 1].dist < distance)
            {
                return find_v(queue, distance, 2 * index);
            }
            if (queue[(2 * index + 1) - 1].dist < distance)
            {
                return find_v(queue, distance, 2 * index);
            }
            return value;
        }
        public void move_up_queue(ref List<data> queue, int start_i)
        {
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
                    index = (index / 2);
                }
                if (index <= 1) break;
            }
        }
        public data deletemin(ref List<data> queue)
        {
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

                    if (left.dist < right.dist)
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
                    index = index_n;
                }
                else break;

            }

            return min;
        }

        public void insert(ref List<data> queue, data value)
        {
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


        }

        public double eucl_dist(PointF one, PointF two)
        {
            return Math.Pow(Math.Pow((one.Y - two.Y), 2) + Math.Pow((one.X - two.X), 2),0.5);
        }
        public void draw(PointF one, PointF two)
        {
            graphics.DrawLine(b_pen, one.X, one.Y, two.X, two.Y);
        }
    }
}