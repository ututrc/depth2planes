namespace ThreeDTrackCS
{
    public class FeatureMargin
    {
        private FeatureExtractor parent;
        private int left, top, right, bottom;

        /// <summary>
        /// Get or set left margin
        /// </summary>
        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                parent.OnMarginChanged();
            }
        }
        /// <summary>
        /// Get or set top margin
        /// </summary>
        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                parent.OnMarginChanged();
            }
        }
        /// <summary>
        /// Get or set right margin
        /// </summary>
        public int Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
                parent.OnMarginChanged();
            }
        }
        /// <summary>
        /// Get or set bottom margin
        /// </summary>
        public int Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                bottom = value;
                parent.OnMarginChanged();
            }
        }

        internal FeatureMargin( FeatureExtractor extractor )
        {
            parent = extractor;
        }

    }
}