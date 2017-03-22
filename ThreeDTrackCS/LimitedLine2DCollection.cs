using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeDTrackCS
{
    /// <summary>
    /// A collection of rays
    /// </summary>
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
        /// <summary>
        /// Clear all lines
        /// </summary>
        public void Clear()
        {
            lines.Clear();
            lineIdGenerator = 0;
        }
        /// <summary>
        /// Add a line
        /// </summary>
        /// <param name="limitedLine2d"></param>
        /// <returns></returns>
        public int Add( LimitedLine2d limitedLine2d )
        {
            int lineIndex = lineIdGenerator++;
            lines.Add( lineIndex, limitedLine2d );
            return lineIndex;
        }
    }
}