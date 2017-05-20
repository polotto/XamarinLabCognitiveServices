using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace CognitiveServices
{
	public class Service
	{
		public static Service Instance { get; } = new Service();

		public Service()
		{
			FaceServiceClient = new FaceServiceClient("f68416e4ff7a4176b3ecfe4cdddd233b");
            _personGroupId = Guid.NewGuid().ToString();
		}

		public List<Person> People { get; } = new List<Person>
		{
			new Person{
				Name = "Bruno Scrok Brunoro",
				PhotoUrl = "https://polottoblob.blob.core.windows.net/images/bruno.jpg",
				City = "Colombo"
			},
			new Person{
				Name = "Angelo Polotto",
				PhotoUrl = "https://polottoblob.blob.core.windows.net/images/angelo.jpg",
				City = "São José do Rio Preto"
			},
			new Person{
				Name = "William S Rodiguez",
				PhotoUrl = "https://polottoblob.blob.core.windows.net/images/william.jpg",
				City = "Curitiba"
			}
		};

		string _personGroupId;

		public FaceServiceClient FaceServiceClient { get; private set; }

		public async Task RegisterEmployees()
		{
			await FaceServiceClient.CreatePersonGroupAsync(_personGroupId, "Xamarin Fest Curitiba");

			foreach (var xmvp in People)
			{
				var p = await FaceServiceClient.CreatePersonAsync(_personGroupId, xmvp.Name);
				await FaceServiceClient.AddPersonFaceAsync(_personGroupId, p.PersonId, xmvp.PhotoUrl);
				xmvp.GroupId = _personGroupId;
				xmvp.PersonId = p.PersonId.ToString();
			}

			await TrainPersonGroup();
		}

		public async Task<List<Person>> FindSimilarFace(Stream faceData)
		{
			var faces = await FaceServiceClient.DetectAsync(faceData);
			var faceIds = faces.Select(face => face.FaceId).ToArray();

			var results = await FaceServiceClient.IdentifyAsync(_personGroupId, faceIds);
            var persons = new List<Person>();
            foreach (var faceid in results)
            {
                if (faceid.Candidates.Count() != 0)
                {
                    var person = await FaceServiceClient.GetPersonAsync(_personGroupId, faceid.Candidates[0].PersonId);
                    persons.Add(new Person { Name = person.Name, PersonId = person.PersonId.ToString() });
                }
            }
            return persons;
		}

		public async Task<bool> AddFace(Stream faceData, Person person)
		{
			try
			{
				var result = await FaceServiceClient.AddPersonFaceAsync(person.GroupId, Guid.Parse(person.PersonId), faceData);
				if (result == null || string.IsNullOrWhiteSpace(result.PersistedFaceId.ToString()))
					return false;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task TrainPersonGroup()
		{
			try
			{
				await FaceServiceClient.TrainPersonGroupAsync(_personGroupId);
				TrainingStatus trainingStatus = null;
				while (true)
				{
					trainingStatus = await FaceServiceClient.GetPersonGroupTrainingStatusAsync(_personGroupId);

					if (trainingStatus.Status != Status.Running)
					{
						break;
					}

					await Task.Delay(1000);
				}
				return;
			}
			catch
			{
				return;
			}
		}

		public async Task<Face> AnalyzeFace(Stream faceData)
		{
			var faces = await FaceServiceClient.DetectAsync(faceData, false, false, new List<FaceAttributeType> {
				FaceAttributeType.Age,
				FaceAttributeType.FacialHair,
				FaceAttributeType.Gender,
				FaceAttributeType.Glasses,
				FaceAttributeType.HeadPose,
				FaceAttributeType.Smile
			});
			if (faces.Length > 0)
				return faces[0];
			return null;
		}
	}
}
