using UnityEngine.Networking;
using SimpleJSON;
using System.Threading.Tasks;
using UnityEngine;

public class GetTagPosition
{
    public int PlantTypeId { get; private set; }
    public float PositionX { get; private set; }

    public async Task GetPositionAsync()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:8000/getPosition"))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                var jsonObject = JSON.Parse(json);
                var tags = jsonObject["tags"];
                var tag = tags[0];
                PlantTypeId = tag["id"];
                var position = tag["position"];
                PositionX = position["x"];
            }
        }
    }
}
