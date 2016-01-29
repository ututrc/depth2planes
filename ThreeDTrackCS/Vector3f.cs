using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    public struct Vector3f
    {

        /// <summary>
        /// The Width component of the vector
        /// </summary>
        public float X;
        /// <summary>
        /// The Height component of the vector
        /// </summary>
        public float Y;
        /// <summary>
        /// The Depth component of the vector
        /// </summary>
        public float Z;

        public static int Size
        {
            get
            {
                return size;
            }
        }
        private static int size = Marshal.SizeOf<Vector3f>();

        /// <summary>
        /// Create a floating point vector from given values
        /// </summary>
        /// <param name="x">The Width component of the vector</param>
        /// <param name="y">The Height component of the vector</param>
        /// <param name="z">The Depth component of the vector</param>
        public Vector3f( float x, float y, float z )
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Squared length of the vector
        /// </summary>
        public float LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        /// <summary>
        /// Precise length of the vector
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt( LengthSquared );
            }
        }

        public static Vector3f operator +( Vector3f a, Vector3f b )
        {
            return new Vector3f( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
        }
        public static Vector3f operator -( Vector3f v )
        {
            return new Vector3f( -v.X, -v.Y, -v.Z );
        }
        public static Vector3f operator -( Vector3f a, Vector3f b )
        {
            return a + ( -b );
        }
        public static Vector3f operator *( Vector3f v, float scalar )
        {
            return new Vector3f( v.X * scalar, v.Y * scalar, v.Z * scalar );
        }
        public static Vector3f operator *( float scalar, Vector3f vector )
        {
            return vector * scalar;
        }
        public static Vector3f operator /( Vector3f v, float divider )
        {
            return v * ( 1 / divider );
        }
        public static float operator *( Vector3f a, Vector3f b )
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Cross product of two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Normal vector for both vectors (cross product)</returns>
        public static Vector3f Cross( Vector3f a, Vector3f b )
        {
            return new Vector3f(
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
            float len = LengthSquared;
            if ( len == 0 || len == 1 )
                return;
            len = 1 / (float)Math.Sqrt( len );
            X *= len;
            Y *= len;
            Z *= len;
        }

        /// <summary>
        /// Get the normalized vector (length == 1) as a new vector
        /// </summary>
        public Vector3f Unit
        {
            get
            {
                return this / Length;
            }
        }

        public static explicit operator Vector3f( Vector3d vector )
        {
            return new Vector3f( (float)vector.X, (float)vector.Y, (float)vector.Z );
        }

        public static implicit operator Vector3d( Vector3f vector )
        {
            return new Vector3d( vector.X, vector.Y, vector.Z );
        }

    }
}
