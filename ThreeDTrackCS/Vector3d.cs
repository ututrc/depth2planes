using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    public struct Vector3d
    {

        /// <summary>
        /// The Width component of the vector
        /// </summary>
        public double X;
        /// <summary>
        /// The Height component of the vector
        /// </summary>
        public double Y;
        /// <summary>
        /// The Depth component of the vector
        /// </summary>
        public double Z;

        /// <summary>
        /// Create a double floating point vector from given values
        /// </summary>
        /// <param name="x">The Width component of the vector</param>
        /// <param name="y">The Height component of the vector</param>
        /// <param name="z">The Depth component of the vector</param>
        public Vector3d( double x, double y, double z )
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Squared length of the vector
        /// </summary>
        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        /// <summary>
        /// Precise length of the vector
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt( LengthSquared );
            }
        }

        public static Vector3d operator +( Vector3d a, Vector3d b )
        {
            return new Vector3d( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
        }
        public static Vector3d operator -( Vector3d v )
        {
            return new Vector3d( -v.X, -v.Y, -v.Z );
        }
        public static Vector3d operator -( Vector3d a, Vector3d b )
        {
            return a + ( -b );
        }
        public static Vector3d operator *( Vector3d v, double scalar )
        {
            return new Vector3d( v.X * scalar, v.Y * scalar, v.Z * scalar );
        }
        public static Vector3d operator *( double scalar, Vector3d vector )
        {
            return vector * scalar;
        }
        public static Vector3d operator /( Vector3d v, double divider )
        {
            return v * ( 1 / divider );
        }
        public static double operator *( Vector3d a, Vector3d b )
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Cross product of two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Normal vector for both vectors (cross product)</returns>
        public static Vector3d Cross( Vector3d a, Vector3d b )
        {
            return new Vector3d(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
                );
        }

        /// <summary>
        /// Changes the length of the vector so that it becomes 1.
        /// </summary>
        public void Normalize()
        {
            double len = LengthSquared;
            if ( len == 0 || len == 1 )
                return;
            len = Math.Sqrt( len );
            X *= len;
            Y *= len;
            Z *= len;
        }

        /// <summary>
        /// Get the normalized vector (length == 1) as a new vector
        /// </summary>
        public Vector3d Unit
        {
            get
            {
                return this / Length;
            }
        }

    }
}
