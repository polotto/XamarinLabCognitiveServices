using Foundation;
using System;
using UIKit;
using CoreFoundation;

namespace CognitiveServices.iOS
{
    public partial class MVPCell : UITableViewCell
    {
        public MVPCell (IntPtr handle) : base (handle)
        {
        }

		public void BindData(Person data)
		{
			_nameLabel.Text = data.Name;
			_cityLabel.Text = data.City;
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				_userImage.Image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(data.PhotoUrl)));
			});
		}
    }
}