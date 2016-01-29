namespace ThreeDTrackCS
{
    public struct Vector2d
    {

        public double X, Y;

        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public double Length
        {
            get
            {
                return System.Math.Sqrt( LengthSquared );
            }
        }

        public Vector2d( double x, double y )
        {
            X = x;
            Y = y;
        }

        public static Vector2d operator +( Vector2d a, Vector2d b )
        {
            return new Vector2d( a.X + b.X, a.Y + b.Y );
        }

        public static Vector2d operator -( Vector2d v )
        {
            return new Vector2d( -v.X, -v.Y );
        }

        public static Vector2d operator -( Vector2d a, Vector2d b )
        {
            return a + ( -b );
        }

        public static Vector2d operator *( Vector2d a, double b )
        {
            return new Vector2d( a.X * b, a.Y * b );
        }

        public static Vector2d operator *( double a, Vector2d b )
        {
            return b * a;
        }

        public static Vector2d operator /( Vector2d a, double b )
        {
            return a * ( 1 / b );
        }

        public static double operator *( Vector2d a, Vector2d b )
        {
            return a.X * b.X + a.Y * b.Y;
        }

    }
}