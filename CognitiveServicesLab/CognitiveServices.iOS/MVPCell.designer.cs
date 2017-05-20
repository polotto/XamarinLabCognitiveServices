// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CognitiveServices.iOS
{
    [Register ("MVPCell")]
    partial class MVPCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _cityLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _nameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView _userImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_cityLabel != null) {
                _cityLabel.Dispose ();
                _cityLabel = null;
            }

            if (_nameLabel != null) {
                _nameLabel.Dispose ();
                _nameLabel = null;
            }

            if (_userImage != null) {
                _userImage.Dispose ();
                _userImage = null;
            }
        }
    }
}