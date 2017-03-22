using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    public class FeatureExtractor
    {

        #region Private Fields

        private FeatureImageSize imageSize;
        private GridDivision gridDivision;
        private FeatureMargin margin;
        private FieldOfViews fieldOfViews;
        private double pointEpsilon;
        private double normalEpsilon;
        private DepthDataFormat depthDataFormat;
        private PlaneClusterizationRule planeClusterizationRule;
        private PlaneClusterCollection planeCollection;
        private CoordinateMatcher coordinateMatcher;
        private Vector3d[] coordinates;
        private Line3DCollection line3DCollection;
        private double depthHFOV;
        private double depthVFOV;
        private double HSIN;
        private double HCOS;
        private double VSIN;
        private double VCOS;
        private Vector3d planeLeftDirection;
        private Vector3d planeRightDirection;
        private Vector3d planeTopDirection;
        private Vector3d planeBottomDirection;
        private Vector3d depthFOVPlaneLeft;
        private Vector3d depthFOVPlaneRight;
        private Vector3d depthFOVPlaneTop;
        private Vector3d depthFOVPlaneBottom;
        private Vector3d depthFOVTopLeftLine;
        private Vector3d depthFOVTopRightLine;
        private Vector3d depthFOVBottomRightLine;
        private Vector3d depthFOVBottomLeftLine;
        private double horizontalMultiplier;
        private double verticalMultiplier;
        private LimitedLine2DCollection limited2dLines;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get all the resulted planes from feature extraction
        /// </summary>
        public PlaneClusterCollection PlaneCollection
        {
            get
            {
                return planeCollection;
            }
        }

        /// <summary>
        /// Get or set the minimum plane cluster size in cells
        /// </summary>
        public int MinimumPlaneClusterSize
        {
            get;
            set;
        }

        /// <summary>
        /// Get the collection of render lines (projected 2D-lines)
        /// </summary>
        public LimitedLine2DCollection RenderLineCollection
        {
            get
            {
                return limited2dLines;
            }
        }

        /// <summary>
        /// Get the collection of intersection lines
        /// </summary>
        public Line3DCollection LineCollection
        {
            get
            {
                return line3DCollection;
            }
        }

        /// <summary>
        /// Get Field of Views
        /// </summary>
        public FieldOfViews FieldOfViews
        {
            get
            {
                return fieldOfViews;
            }
        }

        /// <summary>
        /// Get or set the rule governing which planes to test
        /// </summary>
        public PlaneClusterizationRule PlaneClusterizationRule
        {
            get
            {
                return planeClusterizationRule;
            }
            set
            {
                planeClusterizationRule = value;
            }
        }

        /// <summary>
        /// Get or set the required hit test percetage when testing planes against bigger planes
        /// </summary>
        public double RequiredPlaneTestHitPercetage
        {
            get
            {
                return planeCollection.RequiredHitPercetage;
            }
            set
            {
                planeCollection.RequiredHitPercetage = value;
            }
        }

        /// <summary>
        /// Get or set the accepted normal error value in direction comparisons (for the plane clusterization)
        /// </summary>
        public double NormalEpsilon
        {
            get
            {
                return normalEpsilon;
            }
            set
            {
                normalEpsilon = value;
                planeCollection.NormalDirectionEpsilon = value;
            }
        }

        /// <summary>
        /// Get or set the epsilon value or error value accepted for the point beeing in a plane
        /// </summary>
        public double PointEpsilon
        {
            get
            {
                return pointEpsilon;
            }
            set
            {
                pointEpsilon = value;
                planeCollection.PointEpsilon = value;
            }
        }

        /// <summary>
        /// Get or set the format of the source depth data
        /// </summary>
        public DepthDataFormat DepthDataFormat
        {
            get
            {
                return depthDataFormat;
            }
            set
            {
                depthDataFormat = value;
            }
        }

        /// <summary>
        /// Get the element controlling size of the featured image
        /// </summary>
        public FeatureImageSize ImageSize
        {
            get
            {
                return imageSize;
            }
        }

        /// <summary>
        /// Get the element controlling the division of the grid
        /// </summary>
        public GridDivision GridDivision
        {
            get
            {
                return gridDivision;
            }
        }

        /// <summary>
        /// Get the element controlling the margins of the image and grid
        /// </summary>
        public FeatureMargin Margin
        {
            get
            {
                return margin;
            }
        }

        #endregion

        #region Constructors

        public FeatureExtractor()
        {
            // Setting up the divisions to respond when changed
            line3DCollection = new Line3DCollection();
            limited2dLines = new LimitedLine2DCollection();
            gridDivision = new GridDivision( this );
            imageSize = new FeatureImageSize( this );
            margin = new FeatureMargin( this );
            fieldOfViews = new FieldOfViews( this );
            planeCollection = new PlaneClusterCollection( this );
            coordinateMatcher = new CoordinateMatcher( this );
        }

        #endregion

        #region Internal Featured Functions

        internal void OnDivisonChanged()
        {
        }

        internal void OnImageSizeChanged()
        {
            UpdateFovProperties();
            coordinateMatcher.Update();
        }

        internal void OnMarginChanged()
        {
        }

        internal void OnFieldOfViewChanged()
        {
            UpdateFovProperties();
            coordinateMatcher.Update();
        }

        #endregion

        #region Methods

        public Vector3d GetCoordinateAtIndex( int index )
        {
            if(coordinates != null)
                return coordinates[index];
            return new Vector3d();
        }

        /// <summary>
        /// Extract features from given data
        /// </summary>
        /// <param name="depthData">Pointer to depth data</param>
        public void ExtractPlanes( IntPtr depthData )
        {
            coordinates = new Vector3d[ImageSize.Width * ImageSize.Height];
            coordinateMatcher.CalculateCoordinateTranslation( depthData, coordinates );

            int cellId;
            int realTopLeftIndex, realTopRightIndex, realBottomLeftIndex, realBottomRightIndex;
            List<Plane> planeCollection = new List<Plane>( gridDivision.Horizontal * gridDivision.Vertical );

            Vector3d planePoint, planeNormal;

            int horizontalGridDivision = gridDivision.Horizontal;
            int verticalGridDivision = gridDivision.Vertical;
            double pointEpsilon = this.pointEpsilon;
            double normalEpsilon = this.normalEpsilon;
            int marginLeft = margin.Left;
            int marginRight = margin.Right;
            int marginTop = margin.Top;
            int marginBottom = margin.Bottom;
            int imageWidth = imageSize.Width;
            int imageHeight = imageSize.Height;

            double xsize = imageWidth - marginLeft - marginRight;
            double ysize = imageHeight - marginTop - marginBottom;

            double xgridsize = xsize / horizontalGridDivision;
            double ygridsize = ysize / verticalGridDivision;

            double xpos = marginLeft;
            double ypos = marginTop;

            /*
             * Get the detected (and allowed) planes from the given data
             */
            for ( int x = 0; x < horizontalGridDivision; x++, xpos += xgridsize )
            {
                ypos = marginTop;
                for ( int y = 0; y < verticalGridDivision; y++, ypos += ygridsize )
                {
                    cellId = x + y * gridDivision.Horizontal;

                    realTopLeftIndex = (int)xpos + ( (int)ypos ) * imageWidth;
                    realTopRightIndex = (int)( xpos + xgridsize ) + ( (int)ypos ) * imageWidth;
                    realBottomLeftIndex = (int)xpos + ( (int)( ypos + ygridsize ) ) * imageWidth;
                    realBottomRightIndex = (int)( xpos + xgridsize ) + ( (int)( ypos + ygridsize ) ) * imageWidth;

                    if ( FeatureExtractorHelper.DepthDataValid( depthData, ref depthDataFormat, ref realTopLeftIndex, ref realTopRightIndex, ref realBottomLeftIndex, ref realBottomRightIndex ) )
                    {
                        if ( FeatureExtractorHelper.IsPlane( coordinates, ref realTopLeftIndex, ref realTopRightIndex, ref realBottomLeftIndex, ref realBottomRightIndex, ref pointEpsilon, out planePoint, out planeNormal ) )
                        {
                            planeCollection.Add( FeatureExtractorHelper.CreatePlane( ref planePoint, ref planeNormal, ref cellId ) );
                        }
                    }

                }

            }

            /*
             * Cluster the plane cells to bigger planes
             */

            this.planeCollection.Clear();
            int index = 0;
            foreach ( Plane plane in planeCollection )
            {
                this.planeCollection.AddPlane( plane );
                index++;
            }

            this.planeCollection.RemoveSmallClusters( MinimumPlaneClusterSize );

            
        }

        /// <summary>
        /// Extracts intersection lines after the plane extraction has been done
        /// </summary>
        public void ExtractIntersectionLines()
        {
            line3DCollection.Clear();
            Line3D line;
            foreach ( PlaneCluster plane1 in planeCollection )
            {
                foreach ( PlaneCluster plane2 in planeCollection )
                {
                    if ( plane1 == plane2 )
                        continue;
                    if ( FeatureExtractorHelper.CalculateIntersection( plane1, plane2, out line ) )
                    {
                        line3DCollection.Add( line );
                    }
                }
            }
        }

        /// <summary>
        /// Creates projection planes (2D) for image display
        /// </summary>
        public void CalculateProjectedLines()
        {
            List<Vector2d> acceptablePoints = new List<Vector2d>( 4 );

            Vector2d start, stop;

            limited2dLines.Clear();

            const double epsilon = .0001;

            foreach ( Line3D line in line3DCollection )
            {
                if ( Math.Abs( line.Direction * depthFOVPlaneLeft - 1 ) > epsilon )
                {
                    acceptablePoints.Add( CalculateProjectedPoint( FeatureExtractorHelper.CalculateLinePlaneIntersectionPoint( line, depthFOVPlaneLeft ) ) );
                }
                if ( Math.Abs( line.Direction * depthFOVPlaneTop - 1 ) > epsilon )
                {
                    acceptablePoints.Add( CalculateProjectedPoint( FeatureExtractorHelper.CalculateLinePlaneIntersectionPoint( line, depthFOVPlaneTop ) ) );
                }
                if ( Math.Abs( line.Direction * depthFOVPlaneRight - 1 ) > epsilon )
                {
                    acceptablePoints.Add( CalculateProjectedPoint( FeatureExtractorHelper.CalculateLinePlaneIntersectionPoint( line, depthFOVPlaneRight ) ) );
                }
                if ( Math.Abs( line.Direction * depthFOVPlaneBottom - 1 ) > epsilon )
                {
                    acceptablePoints.Add( CalculateProjectedPoint( FeatureExtractorHelper.CalculateLinePlaneIntersectionPoint( line, depthFOVPlaneBottom ) ) );
                }

                if ( FeatureExtractorHelper.FindBestLine( acceptablePoints, imageSize, out start, out stop ) )
                {
                    limited2dLines.Add( new LimitedLine2d( start, stop ) );
                }

            }
        }

        /// <summary>
        /// Calculates projected point in image for 3D point
        /// </summary>
        /// <param name="spacePoint">3D point</param>
        /// <returns>Projected point</returns>
        private Vector2d CalculateProjectedPoint( Vector3d spacePoint )
        {
            double localHalfWidth = spacePoint.Z * horizontalMultiplier;
            double localHalfHeight = spacePoint.Z * verticalMultiplier;

            return new Vector2d(
                ( spacePoint.X + localHalfWidth ) / ( 2 * localHalfWidth ) * imageSize.Width,
                (1 - ( spacePoint.Y + localHalfHeight ) / ( 2 * localHalfHeight ) ) * imageSize.Height);
        }

        private void UpdateFovProperties()
        {
            depthHFOV = fieldOfViews.Horizontal.AngleDegrees;
            depthVFOV = fieldOfViews.Vertical.AngleDegrees;

            double depthHFOVRad2 = ( depthHFOV * .5 ) * Math.PI / 180;
            double depthVFOVRad2 = ( depthVFOV * .5 ) * Math.PI / 180;

            HSIN = Math.Sin( depthHFOVRad2 );
            HCOS = Math.Cos( depthHFOVRad2 );
            VSIN = Math.Sin( depthVFOVRad2 );
            VCOS = Math.Cos( depthVFOVRad2 );

            planeLeftDirection = new Vector3d( -HSIN, 0, HCOS );
            planeRightDirection = new Vector3d( HSIN, 0, HCOS );
            planeTopDirection = new Vector3d( 0, VSIN, VCOS );
            planeBottomDirection = new Vector3d( 0, -VSIN, VCOS );

            depthFOVPlaneLeft = new Vector3d( HCOS, 0, HSIN );
            depthFOVPlaneRight = new Vector3d( -HCOS, 0, HSIN );
            depthFOVPlaneTop = new Vector3d( 0, -VCOS, VSIN );
            depthFOVPlaneBottom = new Vector3d( 0, VCOS, VSIN );

            depthFOVTopLeftLine = Vector3d.Cross( depthFOVPlaneLeft, depthFOVPlaneTop );
            depthFOVTopRightLine = Vector3d.Cross( depthFOVPlaneTop, depthFOVPlaneRight );
            depthFOVBottomRightLine = Vector3d.Cross( depthFOVPlaneRight, depthFOVPlaneBottom );
            depthFOVBottomLeftLine = Vector3d.Cross( depthFOVPlaneBottom, depthFOVPlaneLeft );

            horizontalMultiplier = planeRightDirection.X / planeRightDirection.Z;
            verticalMultiplier = planeTopDirection.Y / planeTopDirection.Z;
        }

        #endregion

    }
}
