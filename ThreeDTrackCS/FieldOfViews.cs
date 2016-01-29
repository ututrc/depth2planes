using System;

namespace ThreeDTrackCS
{
    public class FieldOfViews
    {
        private FeatureExtractor parent;

        public FieldOfView Horizontal
        {
            get;
            private set;
        }

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