using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeDTrackCS
{
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

        public int Add( Line3D line )
        {
            int lineIndex = lineIdGenerator++;
            lines.Add( lineIndex, line );
            return lineIndex;
        }

        public void Clear()
        {
            lines.Clear();
            lineIdGenerator = 0;
        }
    }
}