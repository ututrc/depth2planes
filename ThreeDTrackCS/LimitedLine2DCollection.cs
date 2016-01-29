using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeDTrackCS
{
    public class LimitedLine2DCollection : IEnumerable<LimitedLine2d>
    {

        private Dictionary<int, LimitedLine2d> lines = new Dictionary<int, LimitedLine2d>();
        private int lineIdGenerator;

        public IEnumerator<LimitedLine2d> GetEnumerator()
        {
            return ( (IEnumerable<LimitedLine2d>)lines.Values ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lines.Values.GetEnumerator();
        }

        public void Clear()
        {
            lines.Clear();
            lineIdGenerator = 0;
        }

        public int Add( LimitedLine2d limitedLine2d )
        {
            int lineIndex = lineIdGenerator++;
            lines.Add( lineIndex, limitedLine2d );
            return lineIndex;
        }
    }
}