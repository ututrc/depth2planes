using System;

namespace ThreeDTrackCS
{
    public class GridDivision
    {

        const int VerticalDefaultValue = 40;
        const int HorizontalDefaultValue = 40;

        private int vertical = VerticalDefaultValue;
        private int horizontal = HorizontalDefaultValue;
        private FeatureExtractor parent;

        /// <summary>
        /// Get or set the vertical division count of the image
        /// </summary>
        public int Vertical
        {
            get
            {
                return vertical;
            }
            set
            {
                vertical = value;
                parent.OnDivisonChanged();
            }
        }

        /// <summary>
        /// Get or set the horizontal division count of the image
        /// </summary>
        public int Horizontal
        {
            get
            {
                return horizontal;
            }
            set
            {
                horizontal = value;
                parent.OnDivisonChanged();
            }
        }

        internal GridDivision( FeatureExtractor extractor )
        {
            parent = extractor;
        }

    }
}