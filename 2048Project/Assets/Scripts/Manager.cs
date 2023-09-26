using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager _isnstance;                   //单例模式的引用

    public Transform poolManager;                       //生成数字的池子
    private GameObject numPrefab;                        //数字的预制体
    public Number[,] numbers = new Number[4, 4];        //保存方格中的数组

    public List<Number> isMovingNum = new List<Number>();  //正在移动中的Num
    public bool hasMove = false;                           //是否有数字发生了移动

    public int level_score = 0;                            //此次关卡分数
    public Text Now_Score;                                 //当前关卡显示
    public Text Hight_Score;                               //最高分数

    public GameObject UIFinsh;                             //游戏结束页面
    public GameObject UISucceed;                           //游戏成功页面


    void Awake()
    {
        _isnstance = this;
    }
    // Use this for initialization
    void Start()
    {
        numPrefab = Resources.Load<GameObject>("num");
        ReStartBtn();
    }

    // 重新开始
    public void ReStartBtn()
    {
        isMovingNum.Clear();
        numbers = new Number[4, 4];
        for (int i = poolManager.childCount - 1; i >= 0; i--)
        {
            Destroy(poolManager.GetChild(i).gameObject);
        }
        hasMove = false;
        //分数
        level_score = 0;
        Now_Score.text = "0";
        if (PlayerPrefs.HasKey("HightScroe"))
        {
            Hight_Score.text = PlayerPrefs.GetInt("HightScroe").ToString();
        }
        else
        {
            Hight_Score.text = "0";
        }
        //游戏开始生成两个数字
        CreateNun();
        CreateNun();
        UIFinsh.SetActive(false);
        UISucceed.SetActive(false);
    }

    void CreateNun()
    {
        GameObject go = Instantiate(numPrefab, poolManager, true);
        go.transform.localScale = Vector3.one * 2;
    }

    #region 检测键盘和触摸输入
    void Update()
    {
        //匹配手机上的返回键
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    BackMenu();
        //
        //}

        // 重新开始
        //if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        //{
        //   Replay();
        //}

        //触屏，，，
        //有触摸点，且滑动
        if (isMovingNum.Count == 0)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                int dieX = 0;
                int dieY = 0;
                //获取滑动的距离
                Vector2 touchDelPos = Input.GetTouch(0).deltaPosition;
                if (Mathf.Abs(touchDelPos.x) > Mathf.Abs(touchDelPos.y))
                {
                    //滑动距离
                    if (touchDelPos.x > 10)
                    {
                        dieX = 1;
                    }
                    else
                    if (touchDelPos.x < -10)
                    {
                        dieX = -1;
                    }
                }
                else
                {
                    if (touchDelPos.y > 10)
                    {
                        dieY = 1;
                    }
                    else if (touchDelPos.y < -10)
                    {
                        dieY = -1;
                    }
                }
                MoveNum(dieX, dieY);
            }
        }

        //PC  端测试
        if (isMovingNum.Count == 0)
        {
            int dieX = 0;
            int dieY = 0;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                dieX = -1;
            }
            else
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                dieX = 1;
            }
            else
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                dieY = 1;
            }
            else
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                dieY = -1;
            }
            MoveNum(dieX, dieY);
        }

        if (hasMove && isMovingNum.Count == 0)   //生成新的数字
        {
            CreateNun();
            hasMove = false;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (numbers[i, j] != null)
                    {
                        numbers[i, j].OneMove = false;
                    }
                }
            }
        }
    }
    #endregion


    #region 游戏逻辑
    /// <summary>
    /// 数字移动方法
    /// </summary>
    /// <param name="directionX"></param>
    /// <param name="directionY"></param>
    public void MoveNum(int directionX, int directionY)
    {
        if (directionX == 1)  //向右移动  
        {
            //首先将空格填满   最右侧列不需做判断
            for (int j = 0; j < 4; j++)
            {
                for (int i = 2; i >= 0; i--)
                {
                    if (numbers[i, j] != null)  //格子中有物体（数字），，调用移动方法
                    {
                        numbers[i, j].Move(directionX, directionY);
                    }
                }
            }
        }
        else

        //===========向左移动==================
        if (directionX == -1)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 1; i < 4; i++)
                {   //最左侧的一列 [0,0] [0,1] [0,2] [0,3]
                    if (numbers[i, j] != null)
                    {
                        numbers[i, j].Move(directionX, directionY);
                    }
                }
            }
        }
        else

        //===========向上移动==================
        if (directionY == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 2; j >= 0; j--)
                {
                    if (numbers[i, j] != null)
                    {
                        numbers[i, j].Move(directionX, directionY);
                    }
                }
            }
        }
        else

        //===========向下移动==================
        if (directionY == -1)
        {
            for (int i = 3; i >= 0; i--)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (numbers[i, j] != null)  //有物体（数字）就移动
                    {
                        numbers[i, j].Move(directionX, directionY);
                    }
                }
            }
        }

        SetScore();
    }


    /// <summary>
    /// 判断是否是空格的方法
    /// </summary>
    /// <param name="x">数组索引X</param>
    /// <param name="y">数组索引Y</param>
    /// <returns></returns>
    public bool isEmpty(int x, int y)
    {
        if (x < 0 || x > 3 || y < 0 || y > 3)
        {
            return false;
        }
        else if (numbers[x, y] != null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 判断游戏是否结束
    /// </summary>
    /// <returns>返回true则游戏结束</returns>
    public bool isDead()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (numbers[i, j] == null)
                {
                    return false;
                }
            }
        }

        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (numbers[i, j].value == numbers[i + 1, j].value)
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (numbers[i, j].value == numbers[i, j + 1].value)
                {
                    return false;
                }
            }
        }
        return true;
    }

    #endregion

    /// <summary>
    /// 计算得分
    /// </summary>
    void SetScore()
    {
        int macValue = 0;
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (numbers[i, j])
                    macValue += numbers[i, j].value;
            }
        }
        if (macValue > level_score)
        {
            level_score = macValue;
            //更新分数显示
            Now_Score.text = macValue.ToString();
           
        }
        if (macValue > PlayerPrefs.GetInt("HightScroe"))
        {
            PlayerPrefs.SetInt("HightScroe", macValue);
            Hight_Score.text = PlayerPrefs.GetInt("HightScroe").ToString();
        }
    }

    /// <summary>
    /// 显示游戏成功
    /// </summary>
    public void ShowSucceed()
    {
        UISucceed.SetActive(true);
    }


    /// <summary>
    /// 复活
    /// </summary>
    void FuHuo()
    {
        UIFinsh.SetActive(false);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                Number nu = numbers[i, j];
                if (isMovingNum.Contains(nu))
                {
                    isMovingNum.Remove(nu);
                }
                //删除2,4
                if (nu.value == 2 || nu.value == 4 || nu.value == 8)
                {                    
                    Destroy(numbers[i, j].gameObject);
                    numbers[i, j] = null;
                }
            }
        }
    }

    /// <summary>
    /// 退出应用
    /// </summary>
    public void QuitApp()
    {
        Application.Quit();
    }
}