using Android.App;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Plugin.Permissions;
using Android.Content.PM;
using Plugin.Media;
using AndroidHUD;
using System.Threading.Tasks;
using System;

namespace CognitiveServices.Android
{
	[Activity(Label = "Cognitive Services", Theme="@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true, Icon = "@mipmap/icon",
              ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : AppCompatActivity
	{
		async void BtnVerify_Click(object sender, System.EventArgs e)
		{
			if (CrossMedia.Current.IsPickPhotoSupported)
			{
				try
				{
					var result = await CrossMedia.Current.PickPhotoAsync();
					if (result == null)
						return;
                    AndHUD.Shared.Show(this, ResourcesTexts.Searching);
					var persons = await Service.Instance.FindSimilarFace(result.GetStream());
					AndHUD.Shared.Dismiss(this);
                    if (persons == null || persons.Count == 0)
					{
						AndHUD.Shared.Dismiss(this);
						global::Android.Support.V7.App.AlertDialog.Builder message =
									   new global::Android.Support.V7.App.AlertDialog.Builder(this)
									   .SetTitle(ResourcesTexts.MSCS)
									   .SetPositiveButton($"{ResourcesTexts.Ok}", (sender1, e1) => { })
                                           .SetMessage($"{ResourcesTexts.NotFound}");
						message.Show();
						return;
					}
					await Task.Delay(1000);

                    string personsString="";
                    foreach (var item in persons)
                    {
                        personsString += item.Name + ", \n";
                    }

                    global::Android.Support.V7.App.AlertDialog.Builder builder =
									   new global::Android.Support.V7.App.AlertDialog.Builder(this)
                                       .SetTitle(ResourcesTexts.MSCS)
                                       .SetPositiveButton($"{ResourcesTexts.Ok}", (sender1, e1) => { })
                                       .SetMessage(personsString);
                    builder.Show();
				}
                catch(Exception ex)
				{
					AndHUD.Shared.Dismiss(this);
				}
			}
		}

		async void BtnAnalyze_Click(object sender, System.EventArgs e)
		{
			if (CrossMedia.Current.IsPickPhotoSupported)
			{
				try
				{
					var result = await CrossMedia.Current.PickPhotoAsync();
					if (result == null)
						return;
					AndHUD.Shared.Show(this, ResourcesTexts.Analyzing);
					var face = await Service.Instance.AnalyzeFace(result.GetStream());
					if (face == null)
					{
						AndHUD.Shared.Dismiss(this);
						return;
					}
					AndHUD.Shared.Dismiss(this);
					await Task.Delay(1000);
                    var stringData = $"{ResourcesTexts.Gender}: {face.FaceAttributes.Gender}" + Environment.NewLine;
                    stringData += $"{ResourcesTexts.Age}: {face.FaceAttributes.Age}" + Environment.NewLine;
                    stringData += $"{ResourcesTexts.Glasses}: {face.FaceAttributes.Glasses.ToString()}" + Environment.NewLine;
                    stringData += $"{ResourcesTexts.Beard}: {face.FaceAttributes.FacialHair.Beard}" + Environment.NewLine;
                    stringData += $"{ResourcesTexts.Moustache}: {face.FaceAttributes.FacialHair.Moustache}" + Environment.NewLine;
                    stringData += $"{ResourcesTexts.Sideburns}: {face.FaceAttributes.FacialHair.Sideburns}" + Environment.NewLine;
                    stringData += $"{ResourcesTexts.Smile}: {face.FaceAttributes.Smile}";

					global::Android.Support.V7.App.AlertDialog.Builder builder =
									   new global::Android.Support.V7.App.AlertDialog.Builder(this)
                                       .SetTitle(ResourcesTexts.MSCS)
									   .SetPositiveButton(ResourcesTexts.Ok, (sender1, e1) => { })
									   .SetMessage(stringData);
					builder.Show();
				}
				catch
				{
					AndHUD.Shared.Dismiss(this);
				}
			}
		}

		protected async override void OnCreate(global::Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			var recycler = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			recycler.SetAdapter(new MVPAdapter());
			recycler.SetLayoutManager(new LinearLayoutManager(this));

			var btnAnalyze = FindViewById<Button>(Resource.Id.btnAnalyze);
			btnAnalyze.Click += BtnAnalyze_Click;

			var btnVerify = FindViewById<Button>(Resource.Id.btnVerify);
			btnVerify.Click += BtnVerify_Click;

            AndHUD.Shared.Show(this, $"{ResourcesTexts.Initializing}");
			await Service.Instance.RegisterEmployees();
			AndHUD.Shared.Dismiss();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

