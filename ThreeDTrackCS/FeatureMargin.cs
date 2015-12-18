namespace ThreeDTrackCS
{
    public class FeatureMargin
    {
        private FeatureExtractor parent;
        private int left, top, right, bottom;

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