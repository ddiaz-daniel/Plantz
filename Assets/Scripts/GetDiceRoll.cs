using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Threading.Tasks;

public class GetDiceRoll : MonoBehaviour
{
    public int RedDiceCount { get; private set; }
    public int PinkDiceCount { get; private set; }
    public int GreenDiceCount { get; private set; }
    public int BlueDiceCount { get; private set; }
    public int DarkBlueDiceCount { get; private set; }
    public int DarkGreenDiceCount { get; private set; }
    public int YellowDiceCount { get; private set; }

    public int RedDotsCount { get; private set; }
    public int PinkDotsCount { get; private set; }
    public int GreenDotsCount { get; private set; }
    public int BlueDotsCount { get; private set; }
    public int DarkBlueDotsCount { get; private set; }
    public int DarkGreenDotsCount { get; private set; }
    public int YellowDotsCount { get; private set; }

    private async void Start()
    {
        await GetDiceAsync();
    }

    public async Task GetDiceAsync()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:8000/getSeeds"))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                var jsonObject = JSON.Parse(json);
                Debug.Log(jsonObject);

                // Parse the "dice" counts
                var diceCounts = jsonObject["dice"];
                RedDiceCount = diceCounts["red"].AsInt;
                PinkDiceCount = diceCounts["pink"].AsInt;
                GreenDiceCount = diceCounts["green"].AsInt;
                BlueDiceCount = diceCounts["blue"].AsInt;
                DarkBlueDiceCount = diceCounts["darkBlue"].AsInt;
                DarkGreenDiceCount = diceCounts["darkGreen"].AsInt;
                YellowDiceCount = diceCounts["yellow"].AsInt;

                // Parse the "dots" counts
                var dotsCounts = jsonObject["dots"];
                RedDotsCount = dotsCounts["red"].AsInt;
                PinkDotsCount = dotsCounts["pink"].AsInt;
                GreenDotsCount = dotsCounts["green"].AsInt;
                BlueDotsCount = dotsCounts["blue"].AsInt;
                DarkBlueDotsCount = dotsCounts["darkBlue"].AsInt;
                DarkGreenDotsCount = dotsCounts["darkGreen"].AsInt;
                YellowDotsCount = dotsCounts["yellow"].AsInt;

                // Check conditions for dark versions
                if (RedDiceCount == 2 && PinkDiceCount == 0)
                {
                    PinkDiceCount = 1;
                    PinkDotsCount = RedDotsCount / 2;
                    RedDiceCount = 1;
                    RedDotsCount /= 2;
                }
                if (GreenDiceCount == 2 && DarkGreenDiceCount == 0)
                {
                    DarkGreenDiceCount = 1;
                    DarkGreenDotsCount = GreenDotsCount / 2;
                    GreenDiceCount = 1;
                    GreenDotsCount /= 2;
                }
                if (BlueDiceCount == 2 && DarkBlueDiceCount == 0)
                {
                    DarkBlueDiceCount = 1;
                    DarkBlueDotsCount = BlueDotsCount / 2;
                    BlueDiceCount = 1;
                    BlueDotsCount /= 2;
                }

                // if any value is 0 set a random nunmbre between 1 and 6
                if (RedDotsCount == 0 && RedDiceCount > 0)
                {
                    RedDotsCount = Random.Range(1, 6);
                }
                if (PinkDotsCount == 0 && PinkDiceCount > 0)
                {
                    PinkDotsCount = Random.Range(1, 6);
                }
                if (GreenDotsCount == 0 && GreenDiceCount > 0)
                {
                    GreenDotsCount = Random.Range(1, 6);
                }
                if (BlueDotsCount == 0 && BlueDiceCount > 0)
                {
                    BlueDotsCount = Random.Range(1, 6);
                }
                if (DarkBlueDotsCount == 0 && DarkBlueDiceCount > 0)
                {
                    DarkBlueDotsCount = Random.Range(1, 6);
                }
                if (DarkGreenDotsCount == 0 && DarkGreenDiceCount > 0)
                {
                    DarkGreenDotsCount = Random.Range(1, 6);
                }
                if (YellowDotsCount == 0 && YellowDiceCount > 0)
                {
                    YellowDotsCount = Random.Range(1, 6);
                }

            }
        }
    }
}
