using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NAudio;
using NAudio.Wave;
using NAudio.FileFormats;

using Id3;
using Id3.Id3v2;
using UnityEngine.UI;
public class Mp3ToAudioClip : MonoBehaviour {

    [SerializeField] string filepath = "C:/1.mp3";
    string url = "file:///";
    string tempFile = "";
    [SerializeField] string[] tags = new string[3];

    [SerializeField] RawImage image;
    [SerializeField] Text[] text;

    private void Start()
    {
        url += filepath;
        StartCoroutine(GetAudioClip());
        tempFile = Application.dataPath + "/../bytes1.wav";
    }


    IEnumerator GetAudioClip()
    {

        WWW www = new WWW(url);
        yield return www;
        byte[] rawData = www.bytes;
        byte[] mp3Byte = rawData;
        byte[] wavByte;


        using (Stream stream = new MemoryStream())
        {
            stream.Write(mp3Byte, 0,mp3Byte.Length);

            stream.Position = 0;

            using (WaveStream pcm = new Mp3FileReader(stream))
            {
                
                using (var mp3 = new Mp3File(filepath))
                {
                    Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2x);
                    text[0].text = "タイトル　" + tag.Title.Value;
                    text[1].text ="アーティスト名　"　+ tag.Artists.Value[0];
                    Debug.Log(tag.Album.EncodingType);
                    var bytesData = System.Text.Encoding.GetEncoding(932).GetBytes(tag.Album.Value);
                    var str = System.Text.Encoding.UTF8.GetString(bytesData);


                    text[2].text = "アルバム名　"+ tag.Album.Value;
                    var pic = tag.Pictures[0].PictureData;
                    Texture2D tex = new Texture2D(500,500);
                    tex.LoadImage(pic);
                    image.texture = tex;
                }

                
                    wavByte = new byte[pcm.Length];
                //wavByteに読み込み
                //pcm.Read(wavByte, 0,(int)pcm.Length);
                WaveFileWriter.CreateWaveFile(tempFile, pcm);
            }
        }
        WWW loader = new WWW("file://" + tempFile);
        yield return loader;
        if (!System.String.IsNullOrEmpty(loader.error))
            Debug.LogError(loader.error);
        
        AudioClip s1 = loader.GetAudioClip(false, false, AudioType.WAV);
        

        s1.name = "wavデータ";

        var AS = GetComponent<AudioSource>();
        AS.clip = s1;
        AS.Play();

    }

    private void OnDestroy()
    {
        File.Delete(tempFile);
    }
}
