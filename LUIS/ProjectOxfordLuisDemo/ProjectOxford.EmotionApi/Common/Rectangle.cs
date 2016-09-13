// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class Rectangle
    {
        /// <summary>
        /// 
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; set; }

        #region overrides
        public override bool Equals(object o)
        {
            if (o == null) return false;

            var other = o as Rectangle;

            if (other == null) return false;

            return this.Left == other.Left &&
                this.Top == other.Top &&
                this.Width == other.Width &&
                this.Height == other.Height;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }
        #endregion
    }
}
