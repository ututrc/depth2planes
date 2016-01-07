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
        private double pointEpsilon;
        private double normalEpsilon;
        private int[] recalculatedIndices;
        private DepthDataFormat depthDataFormat;
        private PointCloudFormat pointCloudFormat;
        private PlaneClusterizationRule planeClusterizationRule;
        private PlaneClusterCollection planeCollection;

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
        /// Get or set the format of the source point cloud
        /// </summary>
        public PointCloudFormat PointCloudFormat
        {
            get
            {
                return pointCloudFormat;
            }
            set
            {
                pointCloudFormat = value;
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
            gridDivision = new GridDivision( this );
            imageSize = new FeatureImageSize( this );
            margin = new FeatureMargin( this );
            planeCollection = new PlaneClusterCollection( this );
        }

        #endregion

        #region Internal Featured Functions

        private void Updateindices()
        {
            recalculatedIndices = new int[( gridDivision.Horizontal + 1 ) * ( gridDivision.Vertical + 1 )];
            double xsize = imageSize.Width - margin.Left - margin.Right;
            double ysize = imageSize.Height - margin.Top - margin.Bottom;
            double xdiv = xsize / gridDivision.Horizontal;
            double ydiv = ysize / gridDivision.Vertical;
            for ( int x = 0; x <= gridDivision.Horizontal; x++ )
            {
                int realX = (int)( margin.Left + x * xdiv );
                for ( int y = 0; y < gridDivision.Vertical; y++ )
                {
                    int realY = (int)( margin.Top + y * ydiv );
                    recalculatedIndices[x + y * ( gridDivision.Vertical + 1 )] = realX + realY * imageSize.Width;
                }
            }
        }

        internal void OnDivisonChanged()
        {
            Updateindices();
        }

        internal void OnImageSizeChanged()
        {
            Updateindices();
        }

        internal void OnMarginChanged()
        {
            Updateindices();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract features from given data
        /// </summary>
        /// <param name="depthData">Pointer to depth data</param>
        /// <param name="pointCloud">Pointer to point cloud</param>
        public void ExtractFreatures( IntPtr depthData, IntPtr pointCloud )
        {

            int topLeftIndex, topRightIndex, bottomLeftIndex, bottomRightIndex;
            int realTopLeftIndex, realTopRightIndex, realBottomLeftIndex, realBottomRightIndex;
            List<Plane> planeCollection = new List<Plane>( gridDivision.Horizontal * gridDivision.Vertical );

            Vector3d planePoint, planeNormal;

            /*
             * Get the detected (and allowed) planes from the given data
             */
            for ( int x = 0; x < gridDivision.Horizontal; x++ )
            {

                for ( int y = 0; y < gridDivision.Vertical; y++ )
                {
                    topLeftIndex = x + y * gridDivision.Horizontal;
                    topRightIndex = topLeftIndex + 1;
                    bottomLeftIndex = topLeftIndex + gridDivision.Horizontal;
                    bottomRightIndex = bottomLeftIndex + 1;

                    realTopLeftIndex = recalculatedIndices[topLeftIndex];
                    realTopRightIndex = recalculatedIndices[topRightIndex];
                    realBottomLeftIndex = recalculatedIndices[bottomLeftIndex];
                    realBottomRightIndex = recalculatedIndices[bottomRightIndex];

                    if ( FeatureExtractorHelper.DepthDataValid( depthData, depthDataFormat, realTopLeftIndex, realTopRightIndex, realBottomLeftIndex, realBottomRightIndex ) )
                    {
                        if ( FeatureExtractorHelper.IsPlane( pointCloud, pointCloudFormat, realTopLeftIndex, realTopRightIndex, realBottomLeftIndex, realBottomRightIndex, pointEpsilon, out planePoint, out planeNormal ) )
                        {
                            planeCollection.Add( FeatureExtractorHelper.CreatePlane( planePoint, planeNormal, topLeftIndex ) );
                        }
                    }

                }

            }

            /*
             * Cluster the plane cells to bigger planes
             */

            this.planeCollection.Clear();

            foreach ( Plane plane in planeCollection )
            {
                this.planeCollection.AddPlane( plane );
            }
        }

        #endregion

    }
}
