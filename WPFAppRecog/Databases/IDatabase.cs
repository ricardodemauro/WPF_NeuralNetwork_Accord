// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace WPFAppRecog.Databases
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;
    using WPFAppRecog.Models;

    /// <summary>
    ///   Common interface for databases. You can plug your own
    ///   database in the application by implementing this interface.
    /// </summary>
    /// 
    public interface IDatabase : INotifyPropertyChanged
    {
        /// <summary>
        ///   Gets the number of classes in
        ///   the classification problem.
        /// </summary>
        /// 
        int Classes { get; }

        /// <summary>
        ///   Gets the collection of training instances.
        /// </summary>
        /// 
        IEnumerable<Sample> Training { get; }

        /// <summary>
        ///   Gets the colleection of testing instances.
        /// </summary>
        /// 
        IEnumerable<Sample> Testing { get; }

        /// <summary>
        ///   Converts a input vector in feature representation into a Bitmap.
        /// </summary>
        /// 
        Bitmap ToBitmap(double[] features);

        /// <summary>
        ///   Converts a bitmap into a feature vector.
        /// </summary>
        ///  
        double[] ToFeatures(Bitmap bitmap);

        /// <summary>
        ///   Applies any preprocessing to feature vectors.
        /// </summary>
        void Normalize(double[] inputs);

        /// <summary>
        ///   Applies any preprocessing to feature vectors.
        /// </summary>
        void Normalize(double[][] inputs);

        /// <summary>
        ///   Gets or sets whether features should be normalized.
        /// </summary>
        /// 
        bool IsNormalized { get; set; }

        /// <summary>
        ///   Loads the training and testing samples.
        /// </summary>
        /// 
        void Load();
    }
}
