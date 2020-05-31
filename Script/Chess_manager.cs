using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Chess_manager : MonoBehaviour {

    static Dictionary<string, Part> name2Hex = new Dictionary<string, Part>();
    static List<Part> chooseChess = new List<Part>();
    static List<Part> openList = new List<Part>();
    static List<Part> closeList = new List<Part>();
    static List<Part> showLis = new List<Part>();
    static List<Part> arealist = new List<Part>();
    void Start()
    {
        name2Hex.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!name2Hex.ContainsKey(transform.GetChild(i).name))
                name2Hex.Add(transform.GetChild(i).name, transform.GetChild(i).GetComponent<Part>());
            else
            {
                transform.GetChild(i).name = transform.GetChild(i).name + Random.Range(0,10).ToString();
                name2Hex.Add(transform.GetChild(i).name, transform.GetChild(i).GetComponent<Part>());
            }
        }
    }
    void Update()
    {
 
    }
    public Part GetHexByName(string i)
    {
        Part v = new Part();
        name2Hex.TryGetValue(i, out v);
        return v;
    }
    public Dictionary<string, Part> GetAllPart()
    {
        return name2Hex;
    }
    public static List<Part> searchRoute(Part thisPart, Part targetPart)
    {
        openList.Clear();
        closeList.Clear();
        Part nowPart = thisPart;
        nowPart.reset();

        openList.Add(nowPart);
        bool finded = false;
        while (!finded)
        {
            openList.Remove(nowPart);//将当前节点从openList中移除  
            closeList.Add(nowPart);//将当前节点添加到关闭列表中  
            Part[] neighbors = nowPart.getNeighborList();//获取当前六边形的相邻六边形  
            //Debug.Log("当前相邻节点数----" + neighbors.size());  
            foreach (Part neighbor in neighbors)
            {
                if (neighbor == null) continue;

                if (neighbor == targetPart)
                {//找到目标节点  
                    //Debug.Log("找到目标点");  
                    finded = true;
                    neighbor.setFatherPart(nowPart);
                }
                if (closeList.Contains(neighbor) || !neighbor.canPass())
                {//在关闭列表里  
                    //Debug.Log("无法通过或者已在关闭列表");  
                    continue;
                }

                if (openList.Contains(neighbor))
                {//该节点已经在开启列表里  
                    //Debug.Log("已在开启列表，判断是否更改父节点");  
                    float assueGValue = neighbor.computeGValue(nowPart) + nowPart.getgValue();//计算假设从当前节点进入，该节点的g估值  
                    if (assueGValue < neighbor.getgValue())
                    {//假设的g估值小于于原来的g估值  
                        openList.Remove(neighbor);//重新排序该节点在openList的位置  
                        neighbor.setgValue(assueGValue);//从新设置g估值  
                        openList.Add(neighbor);//从新排序openList。  
                    }
                }
                else
                {//没有在开启列表里  
                    //Debug.Log("不在开启列表，添加");  
                    neighbor.sethValue(neighbor.computeHValue(targetPart));//计算好他的h估值  
                    neighbor.setgValue(neighbor.computeGValue(nowPart) + nowPart.getgValue());//计算该节点的g估值（到当前节点的g估值加上当前节点的g估值）  
                    openList.Add(neighbor);//添加到开启列表里  
                    neighbor.setFatherPart(nowPart);//将当前节点设置为该节点的父节点  
                }
            }

            if (openList.Count <= 0)
            {
                //Debug.Log("无法到达该目标");  
                break;
            }
            else
            {
                nowPart = openList[0];//得到f估值最低的节点设置为当前节点  
            }
        }
        openList.Clear();
        closeList.Clear();

        List<Part> route = new List<Part>();
        if (finded)
        {//找到后将路线存入路线集合  
            Part hex = targetPart;
            while (hex != thisPart)
            {
                route.Add(hex);//将节点添加到路径列表里  

                Part fatherHex = hex.getFatherPart();//从目标节点开始搜寻父节点就是所要的路线  
                hex = fatherHex;
            }
            route.Add(hex);
        }
        route.Reverse();
        return route;
        //      resetMap();  
    }

    //通过无阻挡寻路确定两个六边形的距离
    public static int GetRouteDis(Part thisPart, Part targetPart)
    {
        Part nowPart = thisPart;
        nowPart.reset();

        openList.Add(nowPart);
        bool finded = false;
        while (!finded)
        {
            openList.Remove(nowPart);//将当前节点从openList中移除  
            closeList.Add(nowPart);//将当前节点添加到关闭列表中  
            Part[] neighbors = nowPart.getNeighborList();//获取当前六边形的相邻六边形  
            //Debug.Log("当前相邻节点数----" + neighbors.size());  
            foreach (Part neighbor in neighbors)
            {
                if (neighbor == null) continue;

                if (neighbor == targetPart)
                {//找到目标节点  
                    //Debug.Log("找到目标点");  
                    finded = true;
                    neighbor.setFatherPart(nowPart);
                }
                if (closeList.Contains(neighbor))
                {//在关闭列表里  
                    //Debug.Log("无法通过或者已在关闭列表");  
                    continue;
                }

                if (openList.Contains(neighbor))
                {//该节点已经在开启列表里  
                    // Debug.Log("已在开启列表，判断是否更改父节点");  
                    float assueGValue = neighbor.computeGValue(nowPart) + nowPart.getgValue();//计算假设从当前节点进入，该节点的g估值  
                    if (assueGValue < neighbor.getgValue())
                    {//假设的g估值小于于原来的g估值  
                        openList.Remove(neighbor);//重新排序该节点在openList的位置  
                        neighbor.setgValue(assueGValue);//从新设置g估值  
                        openList.Add(neighbor);//从新排序openList。  
                    }
                }
                else
                {//没有在开启列表里  
                    //Debug.Log("不在开启列表，添加");  
                    neighbor.sethValue(neighbor.computeHValue(targetPart));//计算好他的h估值  
                    neighbor.setgValue(neighbor.computeGValue(nowPart) + nowPart.getgValue());//计算该节点的g估值（到当前节点的g估值加上当前节点的g估值）  
                    openList.Add(neighbor);//添加到开启列表里  
                    neighbor.setFatherPart(nowPart);//将当前节点设置为该节点的父节点  
                }
            }

            if (openList.Count <= 0)
            {
                // Debug.Log("无法到达该目标");  
                break;
            }
            else
            {
                nowPart = openList[0];//得到f估值最低的节点设置为当前节点  
            }
        }
        openList.Clear();
        closeList.Clear();

        List<Part> route = new List<Part>();
        if (finded)
        {//找到后将路线存入路线集合  
            Part hex = targetPart;
            while (hex != thisPart)
            {
                route.Add(hex);//将节点添加到路径列表里  

                Part fatherHex = hex.getFatherPart();//从目标节点开始搜寻父节点就是所要的路线  
                hex = fatherHex;
            }
            route.Add(hex);


        }
        return route.Count - 1;
    }

    public static List<Part> getShowArea(Part thisPart, int targetDeep,bool isRemove)
    {
        // Dictionary<string, Part> tempdic = new Dictionary<string, Part>();
        arealist.Clear();
        chooseChess.Clear();
        Part nowpart = thisPart;
        nowpart.reset();
        nowpart.setgValue(0);
        Queue<Part> willvisited = new Queue<Part>();
        willvisited.Enqueue(nowpart);
        while (willvisited.Count > 0)
        {
            nowpart = willvisited.Dequeue();

            arealist.Add(nowpart);
            if (nowpart.getgValue() < targetDeep)
            {

                Part[] neighbors = nowpart.getNeighborList();
                foreach (Part temp in neighbors)
                {
                    if (temp == null) continue;
                    if (!arealist.Contains(temp))
                    {
                        //temp.setFatherPart(nowpart);
                        temp.setgValue(1 + nowpart.getgValue());
                        if(temp.getgValue() <= targetDeep)
                        {
                            willvisited.Enqueue(temp);
                        }
                        temp.reset();
                    }

                }
            }

        }

        if(isRemove)
        {
            for(int i=0;i< arealist.Count;i++)
            {
                if (arealist[i] == null) continue;
                if(arealist[i].isControl)
                {
                    arealist.Remove(arealist[i]);
                }
            }
        }
        return arealist;
    }

    public static List<Part> PartTyeToList(PartType temp)
    {
        showLis.Clear();
        foreach (Part t in name2Hex.Values)
        {
            if (t.thistype == temp)
            {
                showLis.Add(t);
                //Debug.Log(temp);
            }

        }
        return showLis;
    }

}
