using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    public float coldTime = 2f;
    public KeyCode skillKey = KeyCode.Alpha1;
    // 冷却计时器
    private float timer = 0;
       // 是否正在冷却中
    public bool isCooling = false;
    private Image coldMask;
    // Start is called before the first frame update
    void Start()
    {
        //通过名称Find方法找到ColdMask子对象，获取其Image组件的引用
        coldMask = transform.Find("bg/ColdMask").GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isCooling)
        {
            // 冷却中，更新冷却进度
            timer += Time.deltaTime;
            // 计算冷却进度比值，0-1之间，1表示冷却完成fillAmount值从1到0
            coldMask.fillAmount = (coldTime - timer)/coldTime;

            // 冷却完成后，重置冷却状态
            // 当timer>2f时 因为coldTime=2f fillAmount值为0 所以冷却结束
            if(timer > coldTime)
            {
                // 冷却为假
                isCooling = false;
                //进度条为0
                coldMask.fillAmount = 0;
                // 重置冷却计时器
                timer = 0;
            }

        }
        // 点击技能项时触发事件，开始冷却
        if(Input.GetKeyDown(skillKey))
        {
            ReleaseSkill();
        }
        
    }
    // 点击技能项时触发事件，开始冷却
    public void OnSkillClick()
    {
        if(isCooling == false)
        {
            isCooling = true;
            timer = 0;
            // 重置冷却进度
            coldMask.fillAmount = 1;
        }
        
    }
    public void ReleaseSkill()
    {
        if(isCooling == false)
        {
            isCooling = true;
            timer = 0;
            // 重置冷却进度
            coldMask.fillAmount = 1;
        }
    }
}
