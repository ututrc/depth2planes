using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    /// <summary>
    /// A tool that aids in variety of feature extraction calculations
    /// </summary>
    public static class FeatureExtractorHelper
    {
        internal static bool DepthDataValid( IntPtr depthDataPointer, ref DepthDataFormat format, ref int topLeftIndex, ref int topRightIndex, ref int bottomLeftIndex, ref int bottomRightIndex )
        {
            switch ( format )
            {
                case DepthDataFormat.UInt16mm:
                    return Marshal.PtrToStructure<ushort>( depthDataPointer + topLeftIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + topRightIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + bottomLeftIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + bottomRightIndex * 2 ) > 0;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Checks if given pointset forms a plane
        /// </summary>
        /// <param name="pointCloud">Points</param>
        /// <param name="topLeftIndex">Index of first point</param>
        /// <param name="topRightIndex">Index of second point</param>
        /// <param name="bottomLeftIndex">Index of third point</param>
        /// <param name="bottomRightIndex">Index of fourth point</param>
        /// <param name="epsilon">Error allowed in calculations (noise filter)</param>
        /// <param name="planePoint">A position in plane</param>
        /// <param name="planeNormal">Normal of plane</param>
        /// <returns>true if the points form a plane</returns>
        public static bool IsPlane( Vector3d[] pointCloud, ref int topLeftIndex, ref int topRightIndex, ref int bottomLeftIndex, ref int bottomRightIndex, ref double epsilon, out Vector3d planePoint, out Vector3d planeNormal )
        {

            planePoint = pointCloud[topLeftIndex];
            planeNormal = Vector3d.Cross( pointCloud[topRightIndex] - pointCloud[topLeftIndex], pointCloud[bottomLeftIndex] - pointCloud[topLeftIndex] );
            planeNormal.Normalize();

            return Math.Abs( planeNormal.X * pointCloud[bottomRightIndex].X + planeNormal.Y * pointCloud[bottomRightIndex].Y + planeNormal.Z * pointCloud[bottomRightIndex].Z - ( planeNormal.X * planePoint.X + planeNormal.Y * planePoint.Y + planeNormal.Z * planePoint.Z ) ) <= epsilon;
        }

        /// <summary>
        /// Checks if given pointset forms a plane
        /// </summary>
        /// <param name="epsilon">Allowed error</param>
        /// <param name="planePoint">Resulting plane position</param>
        /// <param name="planeNormal">Resulting plane direction</param>
        /// <param name="points">Points in plane</param>
        /// <returns>true if given points are in the same plane</returns>
        public static bool IsPlane( double epsilon, out Vector3d planePoint, out Vector3d planeNormal, params Vector3d[] points )
        {
            if ( points.Length < 3 )
            {
                planePoint = planeNormal = new Vector3d();
                return false;
            }
            planePoint = points[0];
            planeNormal = Vector3d.Cross( points[1] - points[0], points[2] - points[0] );
            planeNormal.Normalize();
            if (points.Length == 3)
            {
                return true;
            }

            for ( int i = 3; i < points.Length; i++ )
            {
                if ( Math.Abs( planeNormal.X * points[i].X + planeNormal.Y * points[i].Y + planeNormal.Z * points[i].Z - ( planeNormal.X * planePoint.X + planeNormal.Y * planePoint.Y + planeNormal.Z * planePoint.Z ) ) > epsilon )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a plane from given data
        /// </summary>
        /// <param name="point">A position in plane</param>
        /// <param name="normal">Direction of plane</param>
        /// <param name="id">A differeating id of the plane</param>
        /// <returns>A plane</returns>
        internal static Plane CreatePlane( ref Vector3d point, ref Vector3d normal, ref int id )
        {

            return new Plane( ref point, ref normal, ref id );
        }

        /// <summary>
        /// Calculates an intersection line between planes
        /// </summary>
        /// <param name="plane1">First plane</param>
        /// <param name="plane2">Second plane</param>
        /// <param name="line">Resulting intersection</param>
        /// <returns>true if planes are not parallel</returns>
        internal static bool CalculateIntersection( PlaneCluster plane1, PlaneCluster plane2, out Line3D line )
        {
            Vector3d normal1 = plane1.AverageNormal;
            Vector3d normal2 = plane2.AverageNormal;
            Vector3d point1 = plane1.AveragePoint;
            Vector3d point2 = plane2.AveragePoint;

            double d1 = normal1 * point1;
            double d2 = normal2 * point2;
            if ( Math.Abs( normal1 * normal2 - 1 ) < double.Epsilon )
            {
                line = new Line3D();
                return false;
            }

            Vector3d lineNormal = Vector3d.Cross( normal1, normal2 );

            Vector3d linePointAsVector = -Vector3d.Cross( normal1 * d2 - normal2 * d1, lineNormal ) / lineNormal.LengthSquared;

            line = new Line3D { Position = linePointAsVector, Direction = lineNormal };

            return true;
        }

        /// <summary>
        /// Calculates line-plane intersection point
        /// </summary>
        /// <param name="line">Intersecting line</param>
        /// <param name="planeNormal">intersected plane</param>
        /// <returns>A point of intersection</returns>
        /// <remarks>Please note that if line direction is perpendicular to planeNormal, the function will throw zero division exception.</remarks>
        internal static Vector3d CalculateLinePlaneIntersectionPoint( Line3D line, Vector3d planeNormal )
        {
            return line.Position + line.Direction * -( planeNormal * line.Position ) / ( line.Direction * planeNormal );
        }

        /// <summary>
        /// Finds points from given collection that are furthest from each other
        /// </summary>
        /// <param name="acceptablePoints">Collection of points</param>
        /// <param name="start">Start point</param>
        /// <param name="stop">End point</param>
        internal static void FindMostDistantPoints( List<Vector2d> acceptablePoints, out Vector2d start, out Vector2d stop )
        {
            double maxDist = 0;
            int sourcePair = -1, targetPair = -1;
            double dist;
            for ( int i = 0; i < acceptablePoints.Count; i++ )
            {
                for ( int j = i + 1; j < acceptablePoints.Count; j++ )
                {
                    dist = ( acceptablePoints[i] - acceptablePoints[j] ).LengthSquared;
                    if ( dist > maxDist )
                    {
                        sourcePair = i;
                        targetPair = j;
                    }
                }
            }
            start = acceptablePoints[sourcePair];
            stop = acceptablePoints[targetPair];
        }


        internal static bool FindBestLine( List<Vector2d> acceptablePoints, FeatureImageSize size, out Vector2d start, out Vector2d stop )
        {
            const double epsilon = 5;
            List<Vector2d> uniquePoints = new List<Vector2d>();
            foreach ( Vector2d point in acceptablePoints )
            {
                if ( IsUnique( point, uniquePoints, epsilon ) && HasAcceptableValues( point, size ) )
                {
                    uniquePoints.Add( point );
                }
            }

            if ( uniquePoints.Count < 2 )
            {
                start = stop = new Vector2d();
                return false;
            }

            if ( uniquePoints.Count > 2 )
            {
                FindMostDistantPoints( uniquePoints, out start, out stop );
                return true;
            }

            start = uniquePoints[0];
            stop = uniquePoints[1];
            return true;
        }

        private static bool HasAcceptableValues( Vector2d point, FeatureImageSize size )
        {
            const double epsilon = 1;
            return point.X >= -epsilon && point.X <= size.Width + epsilon && point.Y >= -epsilon && point.Y <= size.Height + epsilon;
        }

        /// <summary>
        /// Checks if a point, with given error range, is found in given collection
        /// </summary>
        /// <param name="point"></param>
        /// <param name="uniquePoints"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        private static bool IsUnique( Vector2d point, List<Vector2d> uniquePoints, double epsilon )
        {
            epsilon *= epsilon;
            for ( int i = 0; i < uniquePoints.Count; i++ )
            {
                if ( epsilon >= ( uniquePoints[i] - point ).LengthSquared )
                    return false;
            }
            return true;
        }
    }
}
