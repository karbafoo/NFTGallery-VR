using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class MediaManager : MonoBehaviour
{
    string ipfsGateWay = "http://dweb.link/ipfs/";
    public GameObject[] firstRoomImages;
    public int[] tokenIDs;
    public string collectionIPFS;
    public int maxSupply = 10000;
    // Start is called before the first frame update
    void Start()
    {

        tokenIDs = new int[firstRoomImages.Length];
        for(int i = 0 ; i < firstRoomImages.Length; i++){
            tokenIDs[i] = Random.Range(0, maxSupply);
            StartCoroutine(downloadMeta(i));
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    IEnumerator downloadMeta(int tokenID)
    {

        using(UnityWebRequest www = UnityWebRequest.Get(ipfsGateWay + collectionIPFS + "/" + tokenID.ToString()))
        {
            //Send Request and wait
            yield return www.SendWebRequest();

 
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(ipfsGateWay + collectionIPFS + "/" + tokenID.ToString());
                Debug.Log("Error while Receiving: " + www.error);
                Debug.Log("Error while Receiving: " + www.downloadHandler.error);
            }
            else
            {
                Debug.Log("Success");
                var courses = www.downloadHandler.text;
                StartCoroutine(downloadImage(NFTMeta.getCID(NFTMeta.CreateFromJSON(courses).image), firstRoomImages[tokenID].GetComponent<Image>()));
            }
        }
    }
    IEnumerator downloadImage(string url, Image targetImage)
    {
        using(UnityWebRequest www = UnityWebRequestTexture.GetTexture(ipfsGateWay + url))
        {
            //Send Request and wait
            yield return www.SendWebRequest();

 
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error while Receiving: " + www.error);
            }
            else
            {
                Debug.Log("Success");

                //Load Image
                var texture2d = DownloadHandlerTexture.GetContent(www);
                var sprite = Sprite.Create(texture2d, new Rect(0.0f, 0.0f, texture2d.width, texture2d.height), Vector2.zero);
                targetImage.sprite = sprite;
            }
        }
    }
}

[System.Serializable]
public class NFTMeta {
    public string image;
    public static NFTMeta CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NFTMeta>(jsonString);
    }
    public static string getCID(string image){
        return image.Substring(7);
    }
}

