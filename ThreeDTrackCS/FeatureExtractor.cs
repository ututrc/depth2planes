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

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or set the accepted normal error value in direction comparisons
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

    }
}
