using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum HurtType
{
    ROBOT_SMALL = 10
}

public class UIManager : GenericSingleton<UIManager> {

    public int robotHPMax; //get from Game Manager

    public Slider robotHPSlider;

    int robotCurrentHP;

	// Use this for initialization
	void Start () {
        robotCurrentHP = robotHPMax;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("h")) RobotHurt(HurtType.ROBOT_SMALL);
    }

    public void RobotHurt(HurtType ht)
    {
        if (robotCurrentHP <= 0)
        {
            Debug.Log("Die");
            return;
        }
        robotCurrentHP -= (int)ht;
        Debug.Log("robut current hp:" + robotCurrentHP);
        robotHPSlider.value = robotCurrentHP;
        
    }
}
