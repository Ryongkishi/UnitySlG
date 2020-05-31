using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Gamestate
{
    GAME_BG = 1,//游戏背景介绍
    GAME_ON = 2,//正式游戏进行
    GAME_HALF =3,//游戏进行一半
    GAME_OVER_ENEMY = 4,//敌人胜利
    GAME_OVER_PLAYER_BG,//玩家胜利
    GAME_OVER_PLAYER//玩家胜利
};

public class GameState : MonoBehaviour {
    public Gamestate[] state = { Gamestate.GAME_BG, Gamestate.GAME_ON, Gamestate.GAME_HALF, Gamestate.GAME_ON };
    public  List<Part> play_town = new List<Part>();
    public  List<Part> enemy_town = new List<Part>();
    bool isfirst;
    int total;
    int index;
    float time_update = 0.5f;
    // Use this for initialization
    private void Awake()
    {


    }
    void Start () {
        index = 0;
        GameManager.instance.allgamestate = state[index];
    }
	
	// Update is called once per frame
	void Update () {
        if(GameManager.instance.allgamestate == Gamestate.GAME_ON)
        {
            time_update -= Time.deltaTime;
            if(time_update<0)
            {
                check_victor();
                time_update = 0.5f;
            }
        }


	}
    public void update_Townlist()//更新所控制的城镇
    {
        enemy_town.Clear();
        play_town.Clear();
        List<Part> temp = null;
        temp = Chess_manager.PartTyeToList(PartType.CASTLE);
        for (int i = 0; i < temp.Count; i++)
        {
            if (!temp[i].belong)
            {
                enemy_town.Add(temp[i]);
            }
            else
            {
                play_town.Add(temp[i]);
            }
        }
    }
    public void check_victor()
    {
        GameManager.instance.update_list();
        if (enemy_town.Count == 0)
        {

            if (GameManager.instance.getEnemyNum() == 0)
            {
                GameManager.instance.allgamestate = Gamestate.GAME_OVER_PLAYER_BG;
                Debug.Log(GameManager.instance.getEnemyNum());
            }
            else
                Game_state_on();
        }
        else if ( GameManager.instance.getPlayerNum() == 0)
        {
            GameManager.instance.allgamestate = Gamestate.GAME_OVER_ENEMY;
        }
    }
    public void Game_state_on()
    {
        index++;
        if (index >= 4)
            index = 3;
        GameManager.instance.allgamestate = state[index];


    }
}
