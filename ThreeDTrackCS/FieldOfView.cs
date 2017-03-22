namespace ThreeDTrackCS
{
    public class FieldOfView
    {
        /* Basic translators between radians and degrees (used with multiplication) */
        private const double RadiansToDegrees = 180 / System.Math.PI;
        private const double DegreesToRadians = System.Math.PI / 180;

        private double angleDegrees, angleRadians;
        private FieldOfViews parent;

        internal FieldOfView( FieldOfViews parent )
        {
            this.parent = parent;
        }
        
        /// <summary>
        /// Get or set the angle in degrees
        /// </summary>
        public double AngleDegrees
        {
            get
            {
                return angleDegrees;
            }
            set
            {
                angleDegrees = value;
                angleRadians = value * DegreesToRadians;
                parent.OnAngleChange( this );
            }
        }

        /// <summary>
        /// Get or set the angle in radians
        /// </summary>
        public double AngleRadians
        {
            get
            {
                return angleRadians;
            }
            set
            {
                angleRadians = value;
                angleDegrees = value * RadiansToDegrees;
                parent.OnAngleChange( this );
            }
        }

    }
}