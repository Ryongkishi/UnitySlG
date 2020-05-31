using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using YounGenTech.HealthScript;
public struct influce {
    public PartType type;
    public int Heath_influnce;
    public int attack_influence;
    public int attackdis_influence;
    public int movedis_influence;
    public int move_influence;
}//地形的影响结构体
public enum Arm_type
{
    ARMY,//陆军
    Naval//水军
};
public class ChessControl : MonoBehaviour {
    public Transform currentPart;//棋子下方的part
    public Transform tarChess;//目标棋子
    public bool ischoose = false;//是否被选中
    public List<Part> show_List;//显示棋子的列表
    public int chess_behavio;//棋子行为类型，1攻击，2移动
    public bool isactive;
    protected bool isfirst;
    public bool init;

    [Header("局内棋子信息")]
    //当前回合控制攻击和移动的次数
    public int now_movelim;
    public int now_attacklim;
    //当前回合生命 攻击，移动
    public int now_attck_dis;
    public int now_attack;
    public int now_movedis;
    protected int life_buff;

    [Header("棋子信息")]
    public int move_Dis = 2;
    public int attack_Dis = 2;
    public int Attack = 2;
    public int Life = 10;
    public Arm_type arm_type;
    //回合限制次数
    public int movelim = 1;
    public int attacklim = 2;
    public influce influence_lastturn;//上回合地形对棋子的影响
    [Header("动画")]
    protected Animator animator;
    [Header("音效")]
    public AudioSource audio;
    public AudioClip[] music;
    // Use this for initialization

    void Start()
    {
        audio = this.GetComponent<AudioSource>();
        show_List = new List<Part>();
        init = true;
        chess_behavio = 0;
        animator = this.GetComponent<Animator>();
        isfirst = true;
        influence_lastturn = new influce();
        influence_lastturn.type = PartType.NORMAL;
        getnowPart();
        currentPart.GetComponent<Part>().isControl = true;
    }
	void Update ()
    {
        if(ischoose)//回合第一次点击
        {
            StopAllCoroutines();
            CancelInvoke();
            inis_chess();
        }
        if(isactive)
        {
            if (now_movelim > 0 && chess_behavio == 2)
            {
                Invoke("Chess_Move", 0);
            }
            else if (now_attacklim > 0 && chess_behavio == 1)
            {
                Invoke("Chess_Attack", 0);
            }
        }
        if (now_movelim <= 0 && now_attacklim <= 0)
        {
            isactive = false;
        }
        if(GameManager.instance.Turn_flag)
        {
            isactive = true;
            isfirst = true;
            init = true;
            chess_behavio = 0;
        }
    }
    //初始化激活棋子
    public void inis_chess()
    {
        init = true;
        isactive = true;
        ischoose = false;
        if (isfirst)
        {
            int a_buff = 0;
            int a_dis_buff = 0;
            int m_buff = 0;
            int m_dis_buff = 0;

                switch (influence_lastturn.type)
                {
                    case PartType.CASTLE:
                        a_buff = influence_lastturn.attack_influence;
                        if (attack_Dis <= 1)
                            a_dis_buff = influence_lastturn.attackdis_influence;
                        break;
                    case PartType.FOREST:
                        m_dis_buff = influence_lastturn.movedis_influence;
                        a_buff = influence_lastturn.attack_influence;
                        break;
                    case PartType.CROP:
                        a_buff = influence_lastturn.attack_influence;
                        break;
                    case PartType.ROAD:
                        m_dis_buff = influence_lastturn.attackdis_influence;
                        break;
                    case PartType.MOUNTAIN:
                        m_dis_buff = influence_lastturn.movedis_influence;
                        break;
                    case PartType.ROCK:
                        m_buff = influence_lastturn.move_influence;
                        break;
                    case PartType.WATER:
                        m_buff = influence_lastturn.move_influence;
                        break;
                    default:
                        break;
                }

                now_movelim = movelim + m_buff;
                now_attacklim = attacklim;
                now_attck_dis = (attack_Dis + a_dis_buff) >= 1 ? (attack_Dis + a_dis_buff) : 1;//更新攻击距离
                now_attack = (Attack + a_buff) >= 1 ? (Attack + a_buff) : 1;//更新本回合攻击值
                now_movedis = (move_Dis + m_dis_buff) >= 1 ? (move_Dis + m_dis_buff) : 1;//更新移动距离
            isfirst = false;
        }

    }

    //显示周围距离内的棋子
    protected void show_move_Dis(bool state,int ditance,bool isremove)
    {
        if (!state)
        {
            foreach (Part p in show_List)
            {
                //Debug.Log("ok");
                if (p != null)
                    p.isShowArea = false;
            }
            show_List.Clear();
        }
        else
        {
            if (currentPart != null)
            {
                //show_move_Dis(false,0, isremove);
                show_List.Clear();
                show_List = Chess_manager.getShowArea(currentPart.GetComponent<Part>(), ditance, isremove);
                //if(arm_type == Arm_type.Naval)
                //{
                //    foreach (Part p in show_List)
                //    {
                //        if (p.thistype == PartType.WATER)
                //        {
                //            show_List.Remove(p);
                //        }
                //    }
                //}
                foreach (Part p in show_List)
                    {
                        if (p != null)
                        {
                            p.isShowArea = state; 
                        }
                    }
            }

        }
    }
    //分别对应攻击和移动的函数
    protected virtual IEnumerator MoveToTar()
    {
        List<Part> route = new List<Part>();
        route = Chess_manager.searchRoute(currentPart.GetComponent<Part>(), tarChess.GetComponent<Part>());
        audio.clip = music[0];
        audio.PlayOneShot(music[0]);
        for (int i=0;i<route.Count;i++)
        {   
            yield return new WaitForSeconds(0.5f);
            transform.position = Vector3.Lerp(transform.position, route[i].getPosition(), 0.99f);
            currentPart = route[i].transform;
            if (currentPart.GetComponent<Part>().life == 0)
                currentPart.GetComponent<Part>().belong = true;
            if(i == route.Count-1)
            {
                animator.SetBool("ismove", false);
                currentPart.GetComponent<Part>().isControl = true ;
            }

        }
        GameManager.instance.anbeavior = true;
        GameManager.instance.game_state.update_Townlist();
        now_movelim--;
    }
    protected virtual IEnumerator AttackTar()
    {
      
        animator.SetBool("isattack", true);

        audio.clip = music[1];
        audio.PlayOneShot(music[1]);

        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isattack", false);
        yield return new WaitForSeconds(0.2f);
        underAttack(now_attack);
        GameManager.instance.anbeavior = true;
        now_attacklim--;
        Chess_ideal();
    }
    protected virtual void Chess_Move()
    {
        //Debug.Log("选择移动点");
        if(init)
        {
            getnowPart();
            show_move_Dis(true, now_movedis,true);
            init = false;
        }

        animator.SetBool("ismove", true);
        if (Input.GetMouseButtonDown(1))
        {
            //show_move_Dis(false, now_movedis, true);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (mouseHit.transform.GetComponent<Part>() != null&& !mouseHit.transform.GetComponent<Part>().isControl)
                {
                    tarChess = mouseHit.transform;

                    int dis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), tarChess.GetComponent<Part>());
                    if (dis <= now_movedis)
                    {
                        float  temp = tarChess.position.x - currentPart.position.x;
                        if (temp > 0)
                        {
                            this.GetComponent<SpriteRenderer>().flipX = true;
                        }
                        else
                        {
                            this.GetComponent<SpriteRenderer>().flipX = false ;
                        }
                        StartCoroutine(MoveToTar());
                        currentPart.GetComponent<Part>().isControl = false;//离开当前part

                    }
                }
                Chess_ideal();

                animator.SetBool("ismove", false);
            }
            show_move_Dis(false, now_movedis, true);
        }

    }
    protected virtual void Chess_Attack()
    {
        if(init)
        {
            getnowPart();
            show_move_Dis(true, now_attck_dis,false);
            init = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            //show_move_Dis(false, now_attck_dis, false);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (mouseHit.transform.GetComponent<EnemyBehavior>() != null&&
                    mouseHit.transform.CompareTag("Enemy"))
                {
                    //Debug.Log(mouseHit.transform);
                    tarChess = mouseHit.transform;
                    int dis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), tarChess.GetComponent<EnemyBehavior>().currentPart.GetComponent<Part>());
                    if (dis <= now_attck_dis)
                    {
                        float temp = tarChess.position.x - currentPart.position.x;
                        if (temp > 0)
                        {
                            this.GetComponent<SpriteRenderer>().flipX = true;
                        }
                        else
                        {
                            this.GetComponent<SpriteRenderer>().flipX = false;
                        }
                        StartCoroutine(AttackTar());
                    }
                }
            }
            show_move_Dis(false, now_attck_dis, false);
            Chess_ideal();
        }
    }
    //回归初始状态
    protected  void Chess_ideal()
    {
        chess_behavio = 0;
        ischoose = false;
        clearList();
        //show_move_Dis(false, 0);
        init = true;
    }
    //棋子血量
    public virtual void underAttack(int attack)
    {
        Health health = tarChess.GetComponent<Health>();
        if (health)
            if (tarChess.CompareTag("Enemy"))
                health.Damage(new HealthEvent(gameObject, attack));
        tarChess.GetComponent<ChessControl>().Life -= attack;
    }
    //获取当前棋子下的节点
    protected void getnowPart()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up * 100);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<Part>() != null)
                currentPart = hit.transform;

        }
    }
    //让2dsprite显示阴影
    protected void OnEnable()
    {

        transform.GetComponent<SpriteRenderer>().receiveShadows = true;
        transform.GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
    }
    protected void clearList()
    {
        foreach (Part p in show_List)
        {
            if (p != null)
                p.isShowArea = false;
        }
        show_List.Clear();
    }
    public void Type_chess_deal()
    {
        //Debug.Log("type_test");
        Health health = this.GetComponent<Health>();
        Part t = currentPart.GetComponent<Part>();
        PartType tempType = t.thistype;
            switch (tempType)
            {
                case PartType.CASTLE:
                influence_lastturn.type = PartType.CASTLE;
                //玩家占领对方城池;敌人占领玩家城池
                if ((t.life > 0 && (t.belong == false && this.transform.CompareTag("Player")))
                    || (t.life > 0 && (t.belong == true && this.transform.CompareTag("Enemy"))))
                    t.life--;
                if (t.life == 0 && (t.isControl == true && this.transform.CompareTag("Player")))
                {
                    currentPart.GetComponent<Part>().belong = true;
                    t.life++;
                }
                if (t.life == 0 && (t.isControl == false && this.transform.CompareTag("Enemy")))
                {
                    currentPart.GetComponent<Part>().belong = false;
                    t.life++;
                }
                //没有占领的城市无法提供增益
                if ((t.belong == false && !this.transform.CompareTag("Enemy")) ||
                        (t.belong == true && !this.transform.CompareTag("Player")))
                    break;
                health.Heal(new HealthEvent(gameObject, 2));
                Life = (Life + 2) <= 10 ? (Life + 2) : 10;
                Life = (Life + 2) > 0 ? (Life + 2) : 0;
                influence_lastturn.attack_influence = 1;
                influence_lastturn.attackdis_influence = 1;

                    break;
                case PartType.VILLAGE:
                influence_lastturn.type = PartType.VILLAGE;
                influence_lastturn.Heath_influnce = 1;
                break;
            case PartType.FOREST:
                influence_lastturn.type = PartType.FOREST;
                influence_lastturn.movedis_influence = -1;
                if(Attack==1)
                    influence_lastturn.attack_influence = -1;
                break;
            case PartType.CROP:
                influence_lastturn.type = PartType.CROP;
                influence_lastturn.attack_influence = 1;
                break;
                case PartType.ROAD:
                influence_lastturn.type = PartType.ROAD;
                influence_lastturn.movedis_influence = 1;
                break;
                case PartType.MOUNTAIN:
                influence_lastturn.type = PartType.MOUNTAIN;
                influence_lastturn.movedis_influence = -1;
                break;
                case PartType.ROCK:
                influence_lastturn.type = PartType.ROCK;
                influence_lastturn.move_influence = -movelim;
                health.Heal(new HealthEvent(gameObject, -1));
                Life = (Life - 1) <= 10 ? (Life - 1) : 10;
                Life = (Life - 1) > 0 ? (Life - 1) : 0;
                break;
                case PartType.WATER:
                influence_lastturn.type = PartType.WATER;
                influence_lastturn.move_influence = -movelim;
                if(arm_type == Arm_type.Naval)
                    influence_lastturn.move_influence = 0;
                //Debug.Log(influence_lastturn.move_influence);
                break;
                default:
                influence_lastturn.type = PartType.NORMAL;
                break;
        }  
    }
}
