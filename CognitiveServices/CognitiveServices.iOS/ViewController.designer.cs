// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CognitiveServices.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView _mainTableView { get; set; }

        [Action ("BtnAnalyzeFace_Action:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnAnalyzeFace_Action (UIKit.UIBarButtonItem sender);

        [Action ("BtnVerifyFace_Action:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnVerifyFace_Action (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (_mainTableView != null) {
                _mainTableView.Dispose ();
                _mainTableView = null;
            }
        }
    }
}