using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Babcock
{

    public class AutoSink
    {
        static int numCities;
        static int numHighways;
        static int numTrips;
        static Dictionary<string, int> cities = new Dictionary<string, int>();
        static Dictionary<string, List<string>> highways = new Dictionary<string, List<string>>();
        static List<Tuple<string, string>> trips = new List<Tuple<string, string>>();
        static Dictionary<string, int> topoValue = new Dictionary<string, int>();

        public static void GetInformation()
        {
            string line;

            if ((line = Console.ReadLine()) != null)
            {
                numCities = Convert.ToInt32(line);
            }
            // Add highways to dictionary
            for (int i = 0; i < numCities; i++)
            {
                line = Console.ReadLine();
                var lineValue = line.Split(' ')?.ToList();
                cities.Add(lineValue[0], Convert.ToInt32(lineValue[1]));
            }

            // get number of trips
            if ((line = Console.ReadLine()) != null)
            {
                numHighways = Convert.ToInt32(line);
            }
            // Add trips to dictionary
            for (int i = 0; i < numHighways; i++)
            {
                line = Console.ReadLine();
                var lineValue = line.Split(' ')?.ToList();

                //City already has an edge
                if (highways.ContainsKey(lineValue[0]))
                {
                    highways[lineValue[0]].Add(lineValue[1]);
                }
                //Else first edge out of city
                else
                {
                    List<string> firstEdge = new List<string>();
                    firstEdge.Add(lineValue[1]);
                    highways.Add(lineValue[0], firstEdge);
                }

            }

            if ((line = Console.ReadLine()) != null)
            {
                numTrips = Convert.ToInt32(line);
            }

            for (int i = 0; i < numTrips; i++)
            {
                line = Console.ReadLine();
                var lineValue = line.Split(' ')?.ToList();
                trips.Add(new Tuple<string, string>(lineValue[0], lineValue[1]));
            }
        }

        public static void TopologicalSort()
        {
            Dictionary<string, bool> cycleCity = new Dictionary<string, bool>();
            Stack<string> cityStack = new Stack<string>();


            // creates topological sort with each city unvisited
            foreach (string s in cities.Keys)
            {
                cycleCity.Add(s, false); // -1 represents unvisited
            }

            foreach(string s in cycleCity.Keys.ToList())
            {
                if (cycleCity[s] == false) 
                {
                    cycleCity[s] = true; //0 means visited
                    DFS(s, cityStack, cycleCity);
                }
            }
            int postOrder = 0;
            while(cityStack.Count > 0) 
            {
                string theCity = cityStack.Pop();
                topoValue.Add(theCity, postOrder);
                postOrder++;
            }
        }

        public static void DFS(string city, Stack<string> cityStack, Dictionary<string, bool> cycleCity)
        {
            if (highways.ContainsKey(city)) //
            {
                foreach (var s in highways[city]) // for each connected city
                {
                    if (cycleCity[s] == false)
                    {
                        cycleCity[s] = true;
                        DFS(s, cityStack, cycleCity);
                    }
                }
            }

            cityStack.Push(city);
        }

        public static string ShortestPath(string startCity, string endCity)
        {
            //Edge cases and guarantee NOs
            if(startCity == endCity)
            {
                return "0";
            }
            else if (topoValue[startCity] > topoValue[endCity] || numHighways == 0) //start after end city on DAG, won't reach
            {
                return "NO";
            }
            else
            {
               Dictionary<string, int> relaxing = new Dictionary<string, int>();

                //Fills distances with max values except for start city
                foreach(string city in topoValue.Keys.ToList())
                {
                    if(city == startCity)
                    {
                        relaxing.Add(city, 0);
                    }
                    else
                    {
                        relaxing.Add(city, int.MaxValue);
                    }
                }
                foreach(string ci in topoValue.Keys.ToList()) // go through all citiees in topological order
                {
                    if (relaxing[ci] != int.MaxValue) //visited
                    {
                        if(highways.ContainsKey(ci))
                        {
                            foreach (string nextCity in highways[ci]) //for each of the cities above, look at their nodes
                            {
                                if (relaxing[nextCity] > (relaxing[ci] + cities[nextCity])) //if cost to get to node < node value then change
                                {
                                    relaxing[nextCity] = (relaxing[ci] + cities[nextCity]);
                                }
                            }
                        }
                    }
                }
                int returnInt = relaxing[endCity];
                if (returnInt == int.MaxValue)
                {
                    return "NO";
                }
                else
                {
                    return returnInt.ToString();
                }
            }
        }

        public static void buildString(StringBuilder returnString)
        {
            if (numTrips > 1)
            {
                for (int i = 0; i < numTrips - 1; i++)//return lowest cost of each trip 
                {
                    returnString.Append(ShortestPath(trips[i].Item1, trips[i].Item2));
                    returnString.Append("\n"); //add newline
                }
            }
            //append last trip
            returnString.Append(ShortestPath(trips[numTrips - 1].Item1, trips[numTrips - 1].Item2));
        }

        public static string DoTheThing()
        {
            StringBuilder returnString = new StringBuilder();

            GetInformation();
            TopologicalSort();
            buildString(returnString);

            return (returnString.ToString());
        }

        static void Main(string[] args)
        {
            Console.WriteLine(DoTheThing());
        }
    }
}
