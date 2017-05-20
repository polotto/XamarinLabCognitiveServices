using System;
using System.Threading.Tasks;
using BigTed;
using Foundation;
using Plugin.Media;
using Plugin.Media.Abstractions;
using UIKit;

namespace CognitiveServices.iOS
{
	public class MVPDataSource : UITableViewDataSource, IUITableViewDelegate, IUIActionSheetDelegate
	{
		NSIndexPath _lastItemClicked;

		public MVPDataSource()
		{
		}

		#region UITableViewDataSource

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("MVPCell", indexPath);
			(cell as MVPCell).BindData(Service.Instance.People[indexPath.Row]);
			return cell;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return Service.Instance.People.Count;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		#endregion

		#region IUITableViewDelegate

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			_lastItemClicked = indexPath;
			tableView.DeselectRow(indexPath, true);
            var actionSheet = new UIActionSheet($"{ResourcesTexts.MSCS}", this, $"{ResourcesTexts.Cancel}", null, $"{ResourcesTexts.AddFace}");
			actionSheet.ShowInView(tableView);
		}

		#endregion

		#region IUIActionSheetDelegate

		[Export("actionSheet:clickedButtonAtIndex:")]
		public async void Clicked(UIActionSheet actionSheet, nint buttonIndex)
		{
			switch (buttonIndex)
			{
				case 0:
					await AddFace();
					break;
				default:
					return;
			}
		}

		#endregion

		async Task AddFace()
		{
			if (CrossMedia.Current.IsTakePhotoSupported)
			{
				await Task.Delay(500);
				try
				{
					var result = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions {
						PhotoSize = PhotoSize.Medium,
						DefaultCamera = CameraDevice.Front
					});
					if (result == null)
						return;
                    BTProgressHUD.Show($"{ResourcesTexts.AddingFace}");
					var face = await Service.Instance.AddFace(result.GetStream(),Service.Instance.People[_lastItemClicked.Row]);
					if (!face)
					{
						BTProgressHUD.Dismiss();
						return;
					}
					BTProgressHUD.Dismiss();
					await Task.Delay(1000);
                    BTProgressHUD.ShowSuccessWithStatus($"{ResourcesTexts.FaceAdded}");
				}
				catch
				{
					BTProgressHUD.Dismiss();
				}
			}
		}
	}
}
