

using Microsoft.ProjectOxford.Common;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// *********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// *********************************************************

namespace Microsoft.ProjectOxford.Emotion.Contract
{
    public class Emotion
    {
        /// <summary>
        /// Gets or sets the face rectangle.
        /// </summary>
        /// <value>
        /// The face rectangle.
        /// </value>
        public Rectangle FaceRectangle { get; set; }

        /// <summary>
        /// Gets or sets the emotion scores.
        /// </summary>
        /// <value>
        /// The emotion scores.
        /// </value>
        public Scores Scores { get; set; }

        #region overrides
        public override bool Equals(object o)
        {
            if (o == null) return false;

            var other = o as Emotion;

            if (other == null) return false;

            if (this.FaceRectangle == null)
            {
                if (other.FaceRectangle != null) return false;
            }
            else
            {
                if (!this.FaceRectangle.Equals(other.FaceRectangle)) return false;
            }

            if (this.Scores == null)
            {
                return other.Scores == null;
            }
            else
            {
                return this.Scores.Equals(other.Scores);
            }
        }

        public override int GetHashCode()
        {
            int r = (FaceRectangle == null) ? 0x33333333 : FaceRectangle.GetHashCode();
            int s = (Scores == null) ? 0xccccccc : Scores.GetHashCode();
            return r ^ s;
        }
        #endregion
    }
}
