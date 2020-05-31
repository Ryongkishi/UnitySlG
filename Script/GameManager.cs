using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {


    public static GameManager instance;
    public static List<ChessControl> player_arm = new List<ChessControl>();
    public static List<EnemyBehavior> enemy_arm = new List<EnemyBehavior>();
    [Header("游戏内控制信息")]
    public bool Turn_flag;//false玩家回合，true为敌人回合
    public int Choose_index;//选中棋子在list的编号
    public Chess_manager chess_manager;//棋子管理实例
    public bool enemy_hadone;
    bool isfirst;//敌人的一次攻击

    public   Gamestate allgamestate;//游戏进程
    public GameState game_state;//游戏进程
    [Header("ui控制")]
    public Daillog_System dail_sys;
    public UI_Manager ui_manager;
    public Transform ChosenChess;//选中的物品
    public Button[] game_over;//游戏结束按钮
    public bool isNext;//是否对这一次的点击进行反应
    public bool anbeavior;//一次点击所对应的事件是否完成
                          // Use this for initialization
    public RTS_Cam.RTS_Camera cam;
    [Header("场景生成")]
    public GameObject[] prefab;
    [Header("剧情进度控制")]
    bool isupdate;
    public int story_num;
    public Game_UI game_ui;
    public Animator game_Ani;
    bool isfirsttxt;
    void Awake()
    {
        Turn_flag = false;
        instance = this;
        isNext = true;
        initChess_list();
        enemy_hadone = false;
        isfirst = true;
        game_state = this.GetComponent<GameState>();
        isupdate = true ;
        isfirsttxt = true;
    }
    void start()
    {

    }
    void Update()
    {
        if (isupdate)
        {
            game_state.update_Townlist();
            isupdate = false;
        }
        switch (allgamestate)
        {
            case Gamestate.GAME_BG:
                dail_sys.enabled = true;
                dail_sys.session = 0;
                if (isfirsttxt)
                {
                    dail_sys.istxtupdate = true;
                    isfirsttxt = false;
                }
                daillog_bg();
                break;
            case Gamestate.GAME_ON:
                dail_sys.enabled = false;
                Game_on();
                break;
            case Gamestate.GAME_HALF:
                dail_sys.enabled = true;
                dail_sys.session = 1;
                if (isfirsttxt)
                {
                    dail_sys.istxtupdate = true;
                    isfirsttxt = false;
                }
                daillog_bg();
                break;
            case Gamestate.GAME_OVER_ENEMY:
                dail_sys.enabled = false;
                Gameover();
                break;
            case Gamestate.GAME_OVER_PLAYER:
                dail_sys.enabled = false;
                Gameover();
                break;
            case Gamestate.GAME_OVER_PLAYER_BG:
                dail_sys.enabled = true;
                dail_sys.session = 2;
                if (isfirsttxt)
                {
                    dail_sys.istxtupdate = true;
                    isfirsttxt = false;
                }
                daillog_bg();
                break;
        }

        //Game_on();
        //
    }
    /// <summary>
    /// 敌人ai控制
    /// </summary>
    int getChooseChess()
    {
        int index = 0;
        List<ChessControl> temp = null;
        temp = player_arm;
        for (int i=0;i<player_arm.Count;i++)
        {
            if (temp[i] == null || ChosenChess == null) continue;
            if (temp[i].name == ChosenChess.name)
            {
                index = i;
            }
        }
        return index;
    }
    public void initChess_list()
    {
        player_arm.Clear();
        enemy_arm.Clear();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0;i< temp.Length;i++)
        {
            player_arm.Add(temp[i].GetComponent<ChessControl>());
        }
        temp = null;
        temp = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < temp.Length; i++)
        {
            enemy_arm.Add(temp[i].GetComponent<EnemyBehavior>());
        }
    }
    public void Chess_Move()
    {
        int choose_index = getChooseChess();
        player_arm[choose_index].chess_behavio = 2;
    }

    public void Chess_Attack()
    {
        int choose_index = getChooseChess();
        player_arm[choose_index].chess_behavio = 1;
    }

    public void turn_change()
    {

        CancelInvoke();
        StopAllCoroutines();
        PartTYpe();//在敌人回合处理一起棋子的状态
        game_state.update_Townlist();
        Turn_flag = !Turn_flag;
        game_ui.ischangecall = true;
        if (Turn_flag)
            isfirst = true;
    }
    //对手回合的执行
    void Enemy_Beahavior()
    {
        ui_manager.On_Off_UI(false);
        Choose_index = 0;
        StartCoroutine(enemy_change());
        if(enemy_hadone)
        {
            enemy_hadone = false;
        }
    }
    //对地形进行处理
    void PartTYpe()
    {
        if(!Turn_flag)
            for (int i = 0; i < player_arm.Count; i++)
            {
                if (player_arm[i] == null)
                {
                    continue;
                }
                player_arm[i].Type_chess_deal();
            }
        else
            for (int i = 0; i < enemy_arm.Count; i++)
            {
                if (enemy_arm[i] == null)
                {
                    continue;
                }
                enemy_arm[i].Type_chess_deal();
            }
    }
    IEnumerator enemy_change()
    {
        for (int i = 0; i < enemy_arm.Count; i++)
        {
            if (enemy_arm[i] == null)
            {
                continue;
            }

            enemy_arm[i].active = true;
            cam.SetTarget(enemy_arm[i].transform);
            yield return new WaitForSeconds(2.0f);
            enemy_arm[i].active = false;
            //enemy_arm[i].GetComponent<EnemyBehavior>().enabled = false;

        }
        turn_change();
        enemy_hadone = true;
    }

    void Game_on()
    {
        if (Turn_flag)
        {
            if (isfirst)
            {
                Enemy_Beahavior();
                isfirst = false;
            }
        }
        else
        {
            //处理点击信息
            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit mouseHit;
                //Debug.Log("click_test");
                if (Physics.Raycast(mouseRay, out mouseHit))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())//不包括ui組件
                        switch (mouseHit.transform.tag)
                        {
                            case "Player":
                                if (isNext)
                                {
                                    anbeavior = false;
                                    ChosenChess = mouseHit.transform;
                                    ChosenChess.GetComponent<ChessControl>().ischoose = true;
                                    ui_manager.On_Off_UI(true);
                                    isNext = false;
                                }
                                break;
                            case "Enemy":
                                ChosenChess = mouseHit.transform;
                                isNext = true;
                                break;
                            case "Chess":
                                ChosenChess = mouseHit.transform;
                                ui_manager.On_Off_UI(false);
                                //isNext = true;
                                break;
                            default:
                                ui_manager.On_Off_UI(false);
                                break;
                        }
                }
            }
            if (anbeavior && !isNext)
            {
                ui_manager.On_Off_UI(false);
                isNext = true;
            }
        }

    }
    /// <summary>
/// 游戏Onui动画部分的

/// </summary>
    void daillog_bg()
    {
        game_Ani.SetBool("isdaillog",true);
    }
    public void dail_over()
    {
        game_Ani.SetBool("isdaillog", false);
        if (allgamestate == Gamestate.GAME_OVER_PLAYER_BG)
        {
            allgamestate = Gamestate.GAME_OVER_PLAYER;
            game_state.enabled = false;
        }
        else
            game_state.Game_state_on();
        
        isfirsttxt = true;
    }
    //更新棋子列表
    public void update_list()
    {
        //Debug.Log("after"+player_arm.Count);
        for (int i = 0; i < player_arm.Count; i++)
        {
            if(player_arm[i] == null)
                player_arm.Remove(player_arm[i]);
        }
        for (int i = 0; i < enemy_arm.Count; i++)
        {
            if (enemy_arm[i] == null)
                enemy_arm.Remove(enemy_arm[i]);
        }
        //Debug.Log("update_"+player_arm.Count);
    }
    public int getEnemyNum()
    {
        return enemy_arm.Count;
    }
    public int getPlayerNum()
    {
        return player_arm.Count;
    }
    /// <summary>
    /// 游戏结束动画部分的
    /// </summary>
    public void Gameover()
    {
        game_Ani.SetBool("isgameover", true);
        if(allgamestate == Gamestate.GAME_OVER_ENEMY)
        {
            game_over[0].enabled = false;
            game_over[0].GetComponent<Image>().enabled = false;
            game_over[1].enabled = true;
            game_over[1].GetComponent<Image>().enabled = true;
        }
        if(allgamestate == Gamestate.GAME_OVER_PLAYER)
        {
            game_over[1].enabled = false;
            game_over[1].GetComponent<Image>().enabled = false;
            game_over[0].enabled = true;
            game_over[0].GetComponent<Image>().enabled = true;
        }
    }

    public void back_scene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("start");//要切换到的场景名
    }
    public void agin_game()
    {

        SceneManager.LoadScene("test01");//要切换到的场景名
    }
    public void Pause()
    {
        game_Ani.SetBool("ispause", true);
        StartCoroutine(TimeScale());
    }
    public void Pause_back()
    {
        Time.timeScale = 1;
        game_Ani.SetBool("ispause", false);
    }
    public IEnumerator TimeScale()
    {
        //Debug.Log("ok");
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;
    }
}
