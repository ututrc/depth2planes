using System;
using System.Runtime.InteropServices;

namespace ThreeDTrackCS
{
    internal class CoordinateMatcher
    {
        private FeatureExtractor parent;
        private Vector3d[] vectors;

        public CoordinateMatcher( FeatureExtractor featureExtractor )
        {
            parent = featureExtractor;
            Update();
        }

        internal void Update()
        {
            double horizontalFOV = parent.FieldOfViews.Horizontal.AngleRadians;
            double verticalFOV = parent.FieldOfViews.Vertical.AngleRadians;

            double angleYmin = verticalFOV * .5;
            double angleYmax = -angleYmin;
            double angleXmin = -horizontalFOV * .5;
            double angleXmax = -angleXmin;

            vectors = new Vector3d[parent.ImageSize.Width * parent.ImageSize.Height];

            double xmulti = ( angleXmax - angleXmin ) / parent.ImageSize.Width;
            double ymulti = ( angleYmax - angleYmin ) / parent.ImageSize.Height;

            for ( int y = 0; y < parent.ImageSize.Height; y++ )
            {
                double currentYAngle = angleYmin + y * ymulti;
                for ( int x = 0; x < parent.ImageSize.Width; x++ )
                {
                    double currentXAngle = angleXmin + x * xmulti;
                    vectors[x + y * parent.ImageSize.Width] = new Vector3d( Math.Sin(currentXAngle) * Math.Cos(currentYAngle), Math.Sin(currentYAngle), Math.Cos(currentXAngle) * Math.Cos(currentYAngle) );
                }
            }
        }

        internal void CalculateCoordinateTranslation( IntPtr depthDataPointer, Vector3d[] coordinates )
        {
            ushort depthPoint;
            for ( int i = 0; i < vectors.Length; i++ )
            {
                depthPoint = Marshal.PtrToStructure<ushort>( depthDataPointer + i * sizeof( ushort ) );
                if ( depthPoint > 0 )
                    coordinates[i] = depthPoint * 0.001 * vectors[i];
                else
                {
                    coordinates[i] = new Vector3d( double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity );
                }
            }
        }
    }
}