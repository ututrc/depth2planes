using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeDTrackCS
{
    /// <summary>
    /// Collection of 3d-lines
    /// </summary>
    public class Line3DCollection : IEnumerable<Line3D>
    {
        private int lineIdGenerator;
        private Dictionary<int, Line3D> lines;

        public Line3DCollection()
        {
            lines = new Dictionary<int, Line3D>();
        }

        public IEnumerator<Line3D> GetEnumerator()
        {
            return ( (IEnumerable<Line3D>)lines.Values ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lines.Values.GetEnumerator();
        }
        /// <summary>
        /// Add a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int Add( Line3D line )
        {
            int lineIndex = lineIdGenerator++;
            lines.Add( lineIndex, line );
            return lineIndex;
        }
        /// <summary>
        /// Clear lines
        /// </summary>
        public void Clear()
        {
            lines.Clear();
            lineIdGenerator = 0;
        }
    }
}