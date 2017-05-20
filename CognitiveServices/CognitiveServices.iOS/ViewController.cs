using System;
using System.Threading.Tasks;
using BigTed;
using Plugin.Media;
using UIKit;

namespace CognitiveServices.iOS
{
	public partial class ViewController : UIViewController
	{
		async partial void BtnAnalyzeFace_Action(UIBarButtonItem sender)
		{
			if (CrossMedia.Current.IsPickPhotoSupported)
			{
				try
				{
					var result = await CrossMedia.Current.PickPhotoAsync();
					if (result == null)
						return;
                    BTProgressHUD.Show($"{ResourcesTexts.Analyzing}");
					var face = await Service.Instance.AnalyzeFace(result.GetStream());
					if (face == null)
					{
						BTProgressHUD.Dismiss();
						return;
					}
					BTProgressHUD.Dismiss();
					await Task.Delay(1000);
					var stringData = $"{ResourcesTexts.Gender}: {face.FaceAttributes.Gender}" + Environment.NewLine;
					stringData += $"{ResourcesTexts.Age}: {face.FaceAttributes.Age}" + Environment.NewLine;
					stringData += $"{ResourcesTexts.Glasses}: {face.FaceAttributes.Glasses.ToString()}" + Environment.NewLine;
					stringData += $"{ResourcesTexts.Beard}: {face.FaceAttributes.FacialHair.Beard}" + Environment.NewLine;
					stringData += $"{ResourcesTexts.Moustache}: {face.FaceAttributes.FacialHair.Moustache}" + Environment.NewLine;
					stringData += $"{ResourcesTexts.Sideburns}: {face.FaceAttributes.FacialHair.Sideburns}" + Environment.NewLine;
					stringData += $"{ResourcesTexts.Smile}: {face.FaceAttributes.Smile}";
                    new UIAlertView($"{ResourcesTexts.MSCS}", stringData, null, $"{ResourcesTexts.Ok}").Show();
				}
				catch
				{
					BTProgressHUD.Dismiss();
				}
			}
		}

		async partial void BtnVerifyFace_Action(UIBarButtonItem sender)
		{
			if (CrossMedia.Current.IsPickPhotoSupported)
			{
				try
				{
					var result = await CrossMedia.Current.PickPhotoAsync();
					if (result == null)
						return;
                    BTProgressHUD.Show($"{ResourcesTexts.Searching}");
					var persons = await Service.Instance.FindSimilarFace(result.GetStream());
					BTProgressHUD.Dismiss();
					if (persons == null || persons.Count == 0)
					{
						BTProgressHUD.Dismiss();
                        new UIAlertView($"{ResourcesTexts.MSCS}", $"{ResourcesTexts.NotFound}", null, $"{ResourcesTexts.Ok}").Show();
						return;
					}
					await Task.Delay(1000);

					string personsString = "";
					foreach (var item in persons)
					{
						personsString += item.Name + ", \n";
					}

					new UIAlertView($"{ResourcesTexts.MSCS}", personsString, null, $"{ResourcesTexts.Ok}").Show();
				}
                catch (Exception e) {
					BTProgressHUD.Dismiss();
				}
			}
		}

		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			var dataSource = new MVPDataSource();
			_mainTableView.Delegate = dataSource;
			_mainTableView.DataSource = dataSource;
			_mainTableView.ReloadData();
            BTProgressHUD.Show($"{ResourcesTexts.Initializing}");
			await Service.Instance.RegisterEmployees();
			BTProgressHUD.Dismiss();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
