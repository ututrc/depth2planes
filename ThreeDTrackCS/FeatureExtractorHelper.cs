using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    static unsafe class FeatureExtractorHelper
    {
        internal static bool DepthDataValid( IntPtr depthDataPointer, DepthDataFormat format, int topLeftIndex, int topRightIndex, int bottomLeftIndex, int bottomRightIndex )
        {
            switch ( format )
            {
                case DepthDataFormat.UInt16mm:
                    uint* uint_pointer = (uint*)depthDataPointer;
                    return uint_pointer[topLeftIndex] > 0 && uint_pointer[topRightIndex] > 0 && uint_pointer[bottomLeftIndex] > 0 && uint_pointer[bottomRightIndex] > 0;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static bool IsPlane( IntPtr pointCloudPointer, PointCloudFormat format, int topLeftIndex, int topRightIndex, int bottomLeftIndex, int bottomRightIndex, double epsilon, out Vector3d planePoint, out Vector3d planeNormal )
        {


            switch ( format )
            {
                case PointCloudFormat.FloatVector:
                    {
                        Vector3f* pointer = (Vector3f*)pointCloudPointer;
                        Vector3f topLeft = pointer[topLeftIndex];
                        Vector3f topRight = pointer[topRightIndex];
                        Vector3f bottomLeft = pointer[bottomLeftIndex];
                        Vector3f bottomRight = pointer[bottomRightIndex];

                        Vector3f dir1 = topRight - topLeft;
                        Vector3f dir2 = bottomLeft - topLeft;

                        Vector3f norm = Vector3f.Cross( dir1, dir2 );
                        norm.Normalize();

                        planePoint = topLeft;
                        planeNormal = norm;

                        float A = norm.X;
                        float B = norm.Y;
                        float C = norm.Z;
                        float D = -A * topLeft.X - B * topLeft.Y - C * topLeft.Z;

                        return Math.Abs( A * bottomRight.X + B * bottomRight.Y + C * bottomRight.Z + D )  <= epsilon;

                    }
                case PointCloudFormat.DoubleVector:
                    {
                        Vector3d* pointer = (Vector3d*)pointCloudPointer;
                        Vector3d topLeft = pointer[topLeftIndex];
                        Vector3d topRight = pointer[topRightIndex];
                        Vector3d bottomLeft = pointer[bottomLeftIndex];
                        Vector3d bottomRight = pointer[bottomRightIndex];

                        Vector3d dir1 = topRight - topLeft;
                        Vector3d dir2 = bottomLeft - topLeft;

                        Vector3d norm = Vector3d.Cross( dir1, dir2 );
                        norm.Normalize();

                        planePoint = topLeft;
                        planeNormal = norm;

                        double A = norm.X;
                        double B = norm.Y;
                        double C = norm.Z;
                        double D = -A * topLeft.X - B * topLeft.Y - C * topLeft.Z;

                        return Math.Abs( A * bottomRight.X + B * bottomRight.Y + C * bottomRight.Z + D ) <= epsilon;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        internal static Plane CreatePlane( Vector3d point, Vector3d normal, int id )
        {

            return new Plane( point, normal, id );
        }
    }
}
