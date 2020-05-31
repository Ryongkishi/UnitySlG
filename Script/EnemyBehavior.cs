using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YounGenTech.HealthScript;

public class EnemyBehavior : ChessControl {
    [Header("敌人棋子信息")]
    public bool active;
    public Transform near_player;
    public Transform near_city;
    public bool isdone;
    public bool ishasTar;
    public bool isatTown;
    // Use this for initialization
    void Awake()
    {
        getnowPart();
    }

    void Start () {
        audio = this.GetComponent<AudioSource>();
        isactive = false;
        show_List = new List<Part>();
        init = true;
        chess_behavio = 0;
        animator = this.GetComponent<Animator>();
        isfirst = true;
        //if (currentPart == null)
        //    Debug.Log(gameObject);
        currentPart.GetComponent<Part>().isControl = true ;
        //if (currentPart == null)
        //    Debug.Log(gameObject);
        int seed = (int)System.DateTime.Now.Ticks;
        PlayerPrefs.SetInt("Seed", (int)System.DateTime.Now.Ticks);
        Random.InitState(seed);
        //enabled = false;
    }
	void Update () {
		if(active)
        {
            StopAllCoroutines();
            CancelInvoke();
            active = false;
            isdone = false;
            inis_chess();
            if (ishasTar)
                attack_with_target();
            else if (isatTown)
                attack_atTown();
            else
                attack_without_target();
        }
        if (now_movelim <= 0 && now_attacklim <= 0)
        {
            isactive = false;
        }
        if (!GameManager.instance.Turn_flag)
        {
            isactive = true;
            isfirst = true;
            init = true;
            chess_behavio = 0;
        }

    }
    protected override IEnumerator MoveToTar()
    {
        audio.clip = music[0];
        audio.PlayOneShot(music[0]);
        if (now_movelim > 0)
        {
            for (int i = 0; i <= now_movedis; i++)
            {
                yield return new WaitForSeconds(0.5f);
                transform.position = Vector3.Lerp(transform.position, show_List[i].getPosition(), 0.99f);
                if (tarChess == show_List[i].GetComponent<Transform>())
                {

                    currentPart = show_List[i].transform;
                    if (currentPart.GetComponent<Part>().life == 0)
                        currentPart.GetComponent<Part>().belong = false;
                    currentPart.GetComponent<Part>().isControl = true;
                    animator.SetBool("ismove", false);
                    break;
                }
                currentPart = show_List[i].transform;
            if (currentPart.GetComponent<Part>().life == 0)
                currentPart.GetComponent<Part>().belong = false;
            }
        }

            GameManager.instance.anbeavior = true;
            GameManager.instance.game_state.update_Townlist();
            if (near_player!=null)
            {
                int tempdis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), near_player.GetComponent<ChessControl>().currentPart.GetComponent<Part>());
                now_movelim--;
                if(tempdis <= now_attck_dis&&now_attacklim>0)
                {
                    Invoke("Chess_Attack", 0.6f);
                }
                else 
                {
                    isdone = true;
                }
            }




    }
    protected override IEnumerator AttackTar()
    {
        animator.SetBool("isattack", true);
        //Debug.Log("attack_test");
        audio.clip = music[1];
        audio.PlayOneShot(music[1]);
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isattack", false);
        yield return new WaitForSeconds(0.2f);
        underAttack(now_attack);
        GameManager.instance.anbeavior = true;
        now_attacklim--;
        isdone = true;
    }
    protected override void Chess_Move()
    {
        animator.SetBool("ismove", true);

        float temp = tarChess.position.x - currentPart.position.x;
        if (temp <= 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        currentPart.GetComponent<Part>().isControl = false;

        StartCoroutine(MoveToTar());

    }
    protected override void Chess_Attack()
    {
        float temp = tarChess.position.x - near_player.position.x;
        if (temp <= 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }
        StartCoroutine(AttackTar());
    }
    public override void underAttack(int attack)
    {
        Health health = near_player.GetComponent<Health>();
        if (health)
            if (near_player.CompareTag("Player") )
                health.Damage(new HealthEvent(gameObject, attack));
        near_player.GetComponent<ChessControl>().Life -= attack;
    }
    void getnearest_chess_and_dis()
    {
        show_List.Clear();
        List<ChessControl> player_arm = new List<ChessControl>();
        player_arm = GameManager.player_arm;
        ChessControl min = null;
        float mindis = 99999;
        getnowPart();
        foreach (ChessControl p in player_arm)
        {
            if (p == null) continue;
            int dis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), p.currentPart.GetComponent<Part>());
            if (mindis > dis)
            {
                mindis = dis;          
                min = p;
            }
        }
        if (min != null)
        {
            show_List = Chess_manager.searchRoute(currentPart.GetComponent<Part>(), min.currentPart.GetComponent<Part>());
            near_player = min.transform;
            if (show_List.Count > 2)
            {
                if(!show_List[now_movedis].isControl)
                    tarChess = show_List[now_movedis].GetComponent<Transform>();
                else
                {
                    show_List = Chess_manager.getShowArea(currentPart.GetComponent<Part>(), now_movedis,true);
                    //Debug.Log(show_List.Count);
                    int standdis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), min.currentPart.GetComponent<Part>());
                    for (int i = 0;i<show_List.Count;i++)
                    {
                        int dis = Chess_manager.GetRouteDis(show_List[i], min.currentPart.GetComponent<Part>());
                        if ((show_List[i] != null && show_List[i].thistype == PartType.WATER && arm_type == Arm_type.ARMY)||(dis > standdis))
                            show_List.Remove(show_List[i]);
                    }
                    int index1 = (int)(Random.Range(0,1) * (show_List.Count-1));
                    if (show_List[index1].isControl || (show_List[index1].thistype == PartType.WATER && this.arm_type == Arm_type.ARMY))
                        tarChess = currentPart;
                    else
                        tarChess = show_List[index1].GetComponent<Transform>();
                }
            }
            else
            tarChess = currentPart;
        }
        show_List = Chess_manager.searchRoute(currentPart.GetComponent<Part>(), tarChess.GetComponent<Part>());
    }
    void attack_without_target()
    {
        getnearest_chess_and_dis();
        Chess_Move();
    }
    void chooseTarget()
    {
        List<Part> List_Town = new List<Part>();
        Part min = null;
        float mindis = 99999;
        if (GameManager.instance.getEnemyNum() < GameManager.instance.getPlayerNum())
        {
            List_Town = GameManager.instance.game_state.enemy_town;
        }
        else
        {
            List_Town = GameManager.instance.game_state.play_town;
        }
        foreach (Part p in List_Town)
        {

            if (p == null) continue;
            int dis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), p);
            if (mindis > dis)
            {
                mindis = dis;
                min = p;
            }
        }
        if (min != null)
        {
            show_List = Chess_manager.searchRoute(currentPart.GetComponent<Part>(), min);
            near_city = min.transform;
            if (show_List.Count > 2)
            {
                if (!show_List[now_movedis].isControl&& show_List[now_movedis].thistype!=PartType.WATER)
                    tarChess = show_List[now_movedis].GetComponent<Transform>();
                else
                {
                    show_List = Chess_manager.getShowArea(currentPart.GetComponent<Part>(), now_movedis, true);
                    //Debug.Log(show_List.Count);
                    int standdis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), min);
                    for (int i = 0; i < show_List.Count; i++)
                    {
                        int dis = Chess_manager.GetRouteDis(show_List[i], min);
                        if ((show_List[i] != null && show_List[i].thistype == PartType.WATER && arm_type == Arm_type.ARMY) || (dis > standdis))
                            show_List.Remove(show_List[i]);
                    }
                    int index1 = (int)(Random.Range(0, 1) * (show_List.Count - 1));
                    if(show_List[index1].isControl|| (show_List[index1].thistype == PartType.WATER&&this.arm_type == Arm_type.ARMY))
                        tarChess = currentPart;
                    else
                    tarChess = show_List[index1].GetComponent<Transform>();
                }
            }
            else
                tarChess = currentPart;

        }
        show_List = Chess_manager.searchRoute(currentPart.GetComponent<Part>(), tarChess.GetComponent<Part>());
    }
    void attack_with_target()
    {
        getnearest_chess_and_dis();
        chooseTarget();
        Chess_Move();
    }
    void attack_atTown()
    {
        getnearest_chess_and_dis();
        if (near_player != null)
        {
            int tempdis = Chess_manager.GetRouteDis(currentPart.GetComponent<Part>(), near_player.GetComponent<ChessControl>().currentPart.GetComponent<Part>());
            now_movelim--;
            if (tempdis <= now_attck_dis && now_attacklim > 0)
            {
                Invoke("Chess_Attack", 0.6f);
            }
            else
            {
                isdone = true;
            }
        }
    }
}
