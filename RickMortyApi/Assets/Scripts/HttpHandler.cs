using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HttpHandler : MonoBehaviour
{
    public int UserId = 1;
    public string apiRickAndMorty = "https://rickandmortyapi.com/api/character";
    public string url = "https://my-json-server.typicode.com/SantiagoAlvarezz/JsonDB";

    [SerializeField]
    private TMP_Text UserNameLabel;
    [SerializeField]
    private RawImage[] Mydeck;
    [SerializeField]
    private TMP_Text[] cardNames;

    private User MyUser;

    public void SendRequest()
    {

        StartCoroutine(GetUsers());

    }
    public void ChangeUser()
    {
        for (int i = 1; i <= 3; i++)
        {
            UserId = i;
            StartCoroutine(GetUsers());
        }
        //UserId = Random.Range(1, 3);
        
    }

    IEnumerator GetCharacter(int index)
    {

        int characterNum = MyUser.deck[index];
        UnityWebRequest request = UnityWebRequest.Get(apiRickAndMorty + "/" + characterNum);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                Debug.Log(request.GetResponseHeader("Content-Type"));
                Characters character = JsonUtility.FromJson<Characters>(request.downloadHandler.text);
                StartCoroutine(DownloadImage(character.image, Mydeck[index]));

                cardNames[index].text = character.name;
                Debug.Log(character.name);//poner.name en label
            }
        }
    }

    IEnumerator GetUsers()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/users/" + UserId);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                //transformacion data
                MyUser = JsonUtility.FromJson<User>(request.downloadHandler.text);
                UserNameLabel.text = MyUser.username;
                for (int i = 0; i < MyUser.deck.Length; i++)
                {
                    StartCoroutine(GetCharacter(i));

                }

                foreach (int card in MyUser.deck)
                {
                    StartCoroutine(GetCharacter(card));

                }


            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator DownloadImage(string MediaURL, RawImage image)
    {
        //string MediaURL = "https://rickandmortyapi.com/api/character/avatar/1.jpeg";
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else if (!request.isHttpError)
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

        }

    }




}
public class UsersList
{
    public List<User> users;
}

public class Worlds
{
    public List<int> worlds;
}

[System.Serializable]
//clase modelo: modelar info del server
public class User
{
    public int id;
    public string username;
    public bool state;
    public int[] deck;

}
public class Characters
{
    public int id;
    public string name;
    public string image;
}
