namespace ThreeDTrackCS
{
    public class FeatureImageSize
    {
        private FeatureExtractor parent;
        private int width, height;

        /// <summary>
        /// Get or set the width of feature image size
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                parent.OnImageSizeChanged();
            }
        }

        /// <summary>
        /// Get or set the height of feature image size
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                parent.OnImageSizeChanged();
            }
        }

        internal FeatureImageSize( FeatureExtractor featureExtractor )
        {
            parent = featureExtractor;
        }
    }
}