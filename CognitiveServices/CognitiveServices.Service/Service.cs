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
				PhotoUrl = "https://scontent.fcwb2-2.fna.fbcdn.net/v/t1.0-9/16425807_1257197117695196_5974667038376484870_n.jpg?oh=07c0d23ee1b72f863642b1311961d595&oe=59BD3E2E",
                City = "Colombo"
			},
			new Person{
				Name = "Angelo Polotto",
				PhotoUrl = "https://scontent.fcwb2-2.fna.fbcdn.net/v/t1.0-9/13912555_1058029624279359_8288490547081103223_n.jpg?oh=f3585a247a3f727c230adc8c5c67083c&oe=59B36C0B",
				City = "São José do Rio Preto"
			},
			new Person{
				Name = "William S Rodiguez",
				PhotoUrl = "https://scontent.fcwb2-2.fna.fbcdn.net/v/t1.0-9/18557163_10211781986537510_1624888997330142326_n.jpg?oh=be4a07ae3247a8cfa4be12857610c149&oe=59A59C59",
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
