using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAppRecog.Databases;
using WPFAppRecog.Models;

namespace WPFAppRecog.Extensions
{
    public static class ListExtensions
    {
        public static double[][] GetInstances(this List<Sample> database)
        {
            var instances = new double[database.Count][];
            for (int i = 0; i < instances.Length; i++)
                instances[i] = database[i].Features;

            return instances;
        }

        public static void GetInstances(this List<Sample> set, out double[][] input, out int[] output)
        {
            input = new double[set.Count][];
            output = new int[set.Count];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = set[i].Features;
                output[i] = set[i].Class;
            }
        }

        public static double[][] GetInstances(this IEnumerable<Sample> database)
        {
            return database.ToList().GetInstances();
        }

        public static void GetInstances(this IEnumerable<Sample> database, out double[][] input, out int[] output)
        {
            database.ToList().GetInstances(out input, out output);
        }

        public static void GetInstances(this IEnumerable<Sample> database, int classes, out double[][] input, out double[][] output)
        {
            database.ToList().GetInstances(classes, out input, out output);
        }

        public static void GetInstances(this List<Sample> set, int classes, out double[][] input, out double[][] output)
        {
            input = new double[set.Count][];
            output = new double[set.Count][];
            for (int i = 0; i < input.Length; i++)
            {
                int total = classes;

                input[i] = set[i].Features;
                output[i] = new double[total];
                output[i][set[i].Class] = 1;
            }
        }
    }
}
