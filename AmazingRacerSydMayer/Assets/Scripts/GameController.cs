using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private string apiurl = "https://fvtcdp.azurewebsites.net/api/leaderboard";
    public List<Leaderboard> games;
    public string userName = "SDS";
    public float elapsedTime = 25;
    
    public GameObject btnGetScores;
    public GameObject btnSubmitScore;

    public GameObject ddlGameList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void GetScores()
    {
        Debug.Log("Get Games...");
         try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                dynamic items = (JArray)JsonConvert.DeserializeObject(jsonResponse);

                
                games = items.ToObject<List<Leaderboard>>();
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                List<Leaderboard> scoreList = new List<Leaderboard>();

                //Added section to sort top 5 scores (lowest scores)
                int min = 0;
                int placeholdermin = 0;
                int scoreCount = 0;
                foreach(Leaderboard leaderboard in games)
                {
                    min = leaderboard.Score;
                    foreach(Leaderboard compareScore in games)
                    {
                        if (compareScore.Score <= min)
                        {
                            placeholdermin = min;
                            min = compareScore.Score;
                            scoreList.Add(leaderboard);
                            foreach(Leaderboard savedScores in scoreList)
                            {
                                if (compareScore.Equals(savedScores) && scoreCount != 0)
                                {
                                    min = placeholdermin;
                                    scoreList.RemoveAt(scoreCount);
                                    break;
                                }
                            }
                        }
                    }
                    scoreCount++;
                    if (scoreCount >= 5)
                        break;
                        
                }

                //prints out first 5 elements
                int count = 0;
                //string topScores = "";
                foreach(Leaderboard leaderboard in scoreList)
                {
                    print(count.ToString() + ": " + leaderboard);
                    //topScores = string.Concat(topScores, leaderboard.ToString());
                    count++;
                    if (count >= 5)
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error " + ex.Message);
            }
    }

   public void SubmitLeaderboard()
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl);
		request.Method = "POST";

		Leaderboard leaderboard = new Leaderboard
		{
			Id = Guid.Empty,
			UserName = userName,
			Score = ((int)elapsedTime),
			GameTime = DateTime.Now
		};

		string serializedObject = JsonConvert.SerializeObject(leaderboard);
		byte[] byteArray = Encoding.UTF8.GetBytes(serializedObject);
		request.ContentType = "application/json";
		request.ContentLength = byteArray.Length;
        Debug.Log(serializedObject);

		Stream dataStream = request.GetRequestStream();
		dataStream.Write(byteArray,0,byteArray.Length);
		WebResponse response = request.GetResponse();

		// Display the status
		dataStream = response.GetResponseStream();
		StreamReader reader = new StreamReader(dataStream);
		string reponseFromServer = reader.ReadToEnd();
        
		reader.Close();
		dataStream.Close();
		response.Close();

        Debug.Log("Finished..." + reponseFromServer);

	}
    
}
