namespace ThreeDTrackCS
{
    public class FieldOfView
    {

        private const double RadiansToDegrees = 180 / System.Math.PI;
        private const double DegreesToRadians = System.Math.PI / 180;

        private double angleDegrees, angleRadians;
        private FieldOfViews parent;

        internal FieldOfView( FieldOfViews parent )
        {
            this.parent = parent;
        }
        
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