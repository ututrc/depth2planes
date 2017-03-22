using System;

namespace ThreeDTrackCS
{
    /// <summary>
    /// Contains both vertical and horizontal field of views
    /// </summary>
    public class FieldOfViews
    {
        private FeatureExtractor parent;

        /// <summary>
        /// Get the horizontal field of view
        /// </summary>
        public FieldOfView Horizontal
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the vertical field of view
        /// </summary>
        public FieldOfView Vertical
        {
            get;
            private set;
        }

        public FieldOfViews( FeatureExtractor featureExtractor )
        {
            parent = featureExtractor;
            Horizontal = new FieldOfView( this );
            Vertical = new FieldOfView( this );
        }

        internal void OnAngleChange( FieldOfView fieldOfView )
        {
            parent.OnFieldOfViewChanged();
        }
    }
}