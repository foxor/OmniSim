using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class DFSLayoutMath
    {
        public const int BRANCHING_FACTOR = 8;
        public const int LAST_NUMBER = BRANCHING_FACTOR - 1;
        public const int DEPTH = 2;
        public const int LAST_INDEX_DEPTH_TEST = DEPTH - 1;

        protected static int[] STRIDES = new int[DEPTH];

        static DFSLayoutMath()
        {
            int stride = 1;
            for (int i = DEPTH - 1; i > -1; i--)
            {
                STRIDES[i] = stride;
                stride = stride * BRANCHING_FACTOR + 1;
            }
        }

        public static IEnumerable<List<int>> build(int depth)
        {
            if (depth == 0)
            {
                yield break;
            }
            for (int i = 0; i < BRANCHING_FACTOR; i++)
            {
                yield return new List<int>(DEPTH) { i };
                foreach (List<int> path in build(depth - 1))
                {
                    path.Insert(0, i);
                    yield return path;
                }
            }
        }

        public static void nextPath(List<int> path)
        {
            int lastIndex = path.Count - 1;
            if (lastIndex == LAST_INDEX_DEPTH_TEST)
            {
                if (path[lastIndex] != LAST_NUMBER)
                {
                    path[lastIndex] += 1; // optimized execution path
                }
                else
                {
                    for (; lastIndex >= 0 && path[lastIndex] == LAST_NUMBER; lastIndex--) ;
                    if (lastIndex >= 0)
                    {
                        int removing = lastIndex + 1;
                        path.RemoveRange(removing, path.Count - removing);
                        path[lastIndex] += 1;
                    }
                    else
                    {
                        path.Clear();
                        path.Add(0);
                    }
                }
            }
            else
            {
                path.Add(0);
            }
        }

        public static int index(List<int> path)
        {
            int index = path.Count - 1;
            for (int i = path.Count - 1; i >= 0; i--)
            {
                index += STRIDES[i] * path[i];
            }
            return index;
        }

        public static List<int> path(int index)
        {
            List<int> r = new List<int>(DEPTH);
            for (int i = 0; i < DEPTH; i++)
            {
                r.Add(index / STRIDES[i]);
                index -= r[i] * STRIDES[i];
                if (index == 0)
                {
                    return r;
                }
                index -= 1;
            }
            //throw new Exception("Couldn't figure out the path for index: " + index);
            return null;
        }

        public static int count()
        {
            return index(Enumerable.Range(0, DEPTH).Select(x => BRANCHING_FACTOR - 1).ToList()) + 1;
        }

        protected static string print(List<int> p)
        {
            return "[" + p.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y) + "]";
        }

        public static void verify()
        {
            int stepIndex = 0;
            List<int> iterativePath = new List<int>() { 0 };
            foreach (List<int> stepPath in build(DEPTH))
            {
                if (index(stepPath) != stepIndex)
                {
                    Debug.Log("Failed to index path: " + print(stepPath) + ".  Actual: " + stepIndex + ", Predicted: " + index(stepPath));
                }
                if (index(path(stepIndex)) != stepIndex)
                {
                    Debug.Log("Failed to path index: " + stepIndex + ".  Actual path: " + print(stepPath) + ", Predicted: " + print(path(stepIndex)));
                }
                if (index(iterativePath) != stepIndex)
                {
                    Debug.Log("Failed to generate next path.  Actual path: " + print(stepPath) + ", Generated path: " + print(iterativePath));
                }
                stepIndex++;
                nextPath(iterativePath);
            }
            if (index(iterativePath) != 0)
            {
                Debug.Log("Failed to loop around to first path");
            }
            Debug.Log("Estimated length: " + count() + " Actual length: " + stepIndex);
        }
    }
}
