using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PartType
{
    NORMAL = 1,//平通节点，无影响
    FOREST = 2,//森林节点，攻击距离为1的单位增加一攻击力，下个回合移动距离减一
    WATER = 3,//水流,会使单位无法移动
    CASTLE = 4,//城堡，提供棋子2点hp回复，对于攻击距离为1的，攻击距离加一点，提升一点攻击力
    VILLAGE = 6,//村庄，提供棋子一点hp回复
    CROP = 7,//农田，下个回合攻击力加一
    MOUNTAIN = 8,//山脉，下个回合第一次能运动的距离减少1，最低一
    ROCK = 9,//泥泞，下个回合不能行动，扣除一点生命值
    ROAD = 10//道路,下个回合第一次能运动的距离加一
};
public class Part : MonoBehaviour {

    Part father = null;
    public Part[] neighbors = new Part[6];
    public bool passFlag = true;
    float gValue = 999f;
    float hValue = 999f;
    public PartType thistype;
    public bool isShowArea;
   // public bool isShowOnce = true;
    public Material redMat;
    public Material greenMat;
    public bool belong;//0属于玩家，1属于敌人
    public bool isControl = false;
    public int life = 0;
   // public bool 
    void Start()
    {
        isShowArea = false;
        Collider[] hit;
        int index = 0;
        hit = Physics.OverlapSphere(transform.position, 1.50f);
        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.GetComponent<Part>()&&hit[i].name!= this.name)
                {

                    Part temp = hit[i].transform.GetComponent<Part>();
                    neighbors.SetValue(temp,index++);
                }
            }
        }
        //redMat = Resources.Load<Material>("Assets/Player_shader.mat");
        //greenMat = Resources.Load<Material>("Assets/Enemy_shader.mat");


    }

    void Update()
    {
        if (isShowArea)
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }

            if (belong)
            {
                this.GetComponent<Renderer>().material = redMat;
                //Debug.Log(redMat);
            }
            else
            {
                this.GetComponent<Renderer>().material = greenMat;
            }




    }
    public void reset()
    {
        float gValue = 999f;
        float hValue = 999f;
    }

    public Part[] getNeighborList()
    {
        return neighbors;
    }

    public void setFatherPart(Part f)
    {
        father = f;
    }

    public Part getFatherPart()
    {
        return father;
    }

    //设置可以通行

    public bool canPass()
    {
        return passFlag;
    }

    public float computeGValue(Part hex)
    {
        return 1f;
    }

    public void setgValue(float v)
    {
        gValue = v;
    }

    public float getgValue()
    {
        return gValue;
    }

    public void sethValue(float v)
    {
        hValue = v;
    }

    public float gethValue()
    {
        return hValue;
    }

    public float computeHValue(Part hex)
    {

        return Vector3.Distance(transform.position, hex.transform.position);
    }

    public Vector3 getPosition()
    {
        return new Vector3(0, 1.0f, 0) + transform.position;
    }
    public static explicit operator Part(Dictionary<string, Part>.ValueCollection v)
    {
        throw new NotImplementedException();
    }

}
