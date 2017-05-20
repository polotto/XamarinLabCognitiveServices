using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace CognitiveServices.Android
{
	public class MVPAdapter : RecyclerView.Adapter
	{
		class DownloadImageTask : AsyncTask
		{
			ImageView _bmImage;

			public DownloadImageTask(ImageView bmImage)
			{
				this._bmImage = bmImage;
			}

			protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
			{
				string urldisplay = @params[0].ToString();
				Bitmap mIcon11 = null;
				try
				{
					var stream = new Java.Net.URL(urldisplay).OpenStream();
					mIcon11 = BitmapFactory.DecodeStream(stream);
				}
				catch (System.Exception e)
				{
                    Log.Error($"{ResourcesTexts.Error}", e.Message);
				}
				return mIcon11;
			}

			protected override void OnPostExecute(Java.Lang.Object result)
			{
				_bmImage.SetImageBitmap(result as Bitmap);
			}
		}

		class MVPViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
		{
			async void HandleOptionClicked(object sender, global::Android.Content.DialogClickEventArgs e)
			{
				if (e.Which == 0)
				{
					await AddFace();
				}
			}

			async Task AddFace()
			{
				if (CrossMedia.Current.IsTakePhotoSupported)
				{
					await Task.Delay(500);
					try
					{
						var result = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
						{
							DefaultCamera = CameraDevice.Front,
							PhotoSize = PhotoSize.Medium
						});
						if (result == null)
							return;
                        AndHUD.Shared.Show(_city.Context, $"{ResourcesTexts.AddingFace}");
						var face = await Service.Instance.AddFace(result.GetStream(), Service.Instance.People[Position]);
						if (!face)
						{
							AndHUD.Shared.Dismiss(_city.Context);
							return;
						}
						AndHUD.Shared.Dismiss(_city.Context);
						await Task.Delay(1000);
                        AndHUD.Shared.ShowSuccessWithStatus(_city.Context, $"{ResourcesTexts.FaceAdded}", MaskType.Black, TimeSpan.FromMilliseconds(1000));
					}
					catch
					{
						AndHUD.Shared.Dismiss(_city.Context);
					}
				}
			}

			TextView _title, _city;
			ImageView _thumbnail;

			public MVPViewHolder(View view):base(view)
			{
				view.SetOnClickListener(this);
				_title = (TextView)view.FindViewById(Resource.Id.name);
				_city = (TextView)view.FindViewById(Resource.Id.city);
				_thumbnail = (ImageView)view.FindViewById(Resource.Id.thumbnail);
			}

			public void BindData(Person person)
			{
				_title.Text = person.Name;
				_city.Text = person.City;
				new DownloadImageTask(_thumbnail).Execute(person.PhotoUrl);
			}

			#region View.IOnClickListener

			public void OnClick(View v)
			{
				global::Android.Support.V7.App.AlertDialog.Builder builder =
								   new global::Android.Support.V7.App.AlertDialog.Builder(_city.Context)
                                   .SetTitle($"{ResourcesTexts.MSCS}")
								   .SetItems(new string[] {
                                    $"{ResourcesTexts.AddFace}"
									}, HandleOptionClicked);
				builder.Show();
			}

			#endregion
		}

		public MVPAdapter():base()
		{
		}

		public override int ItemCount
		{
			get
			{
				return Service.Instance.People.Count;
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			(holder as MVPViewHolder).BindData(Service.Instance.People[position]);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From(parent.Context)
										  .Inflate(Resource.Layout.MVP_Card, parent, false);

			return new MVPViewHolder(itemView);
		}
	}
}
