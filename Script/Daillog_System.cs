using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Daillog_System : MonoBehaviour {

    [Header("UI组件")]
    public Text text_label;
    public Image featureImage;
    public Text text_name;
    [Header("文本文件")]
    public TextAsset textfile;
    public int index;
    public float textSpeed;
    [Header("头像")]
    public Sprite feature_01, feature_02;
    public bool istxtupdate;
    bool textfinish;//是否完成打字
    bool canceTypeing;//取消打字
    List<string> textList = new List<string>();
    public int session;
	// Use this for initialization
	void Awake () {
        //session = 0;
        getTextformFile(textfile);
        textSpeed = 0.1f; // index = 0;f
        istxtupdate = false;
    }
	private void OnEnable()
    {
        text_name.text = " ";
        text_label.text = " ";
        index++;
        textfinish = true;
    }
	// Update is called once per frame
	void Update () {
        if(istxtupdate)
        {
            getTextformFile(textfile);
            istxtupdate = false;
        }
        if (Input.GetMouseButton(0)&&index == textList.Count - 1)
        {
            index = 0;
        }
            if (Input.GetMouseButton(0))
        {
            if(textfinish&&!canceTypeing)
            {
                StartCoroutine(SetTextUI());
            }
            else if(!textfinish)
            {
                canceTypeing = !canceTypeing;
            }
        }
	}
    void getTextformFile(TextAsset file)
    {
        textList.Clear();
        index = 0;
        var linedata = file.text.Split('-');
        //Debug.Log(linedata[session]);
        // Debug.Log(linedata[1]);
        // Debug.Log(linedata[0]);
        //Debug.Log(linedata.Length);
        var txtdata = linedata[session].Split('\n');
        foreach (var line in txtdata )
        {
            textList.Add(line);
        }
    }
    IEnumerator SetTextUI()
    {
        textfinish = false;
        text_label.text = " ";
        switch (textList[index])
        {
            case "BG\r":
                featureImage.sprite = feature_01;
                //text_name.text = textList[index];
                index++;
                break;
            case "EN\r":
                featureImage.sprite = feature_01;
                text_name.text = textList[++index];
                index++;
                break;
            case "PL\r":
                featureImage.sprite = feature_01;
                text_name.text = textList[++index];
                index ++;
                break;
        }
        int letter = 0;
        while(!canceTypeing && letter< textList[index].Length-1)
        {
            text_label.text += textList[index][letter];
            letter++;
            yield return new WaitForSeconds(textSpeed);
        }
        text_label.text = textList[index];
        canceTypeing = false;
        // for(int i = 0;i<textList[index].Length;i++)
        //{
        //    text_label.text += textList[index][i];
        //    yield return new WaitForSeconds(textSpeed);
        //}
        textfinish = true;
        index++;
    }

}
