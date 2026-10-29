using UnityEngine;
using UnityEngine.UI;//引入UI命名空间，用于使用ScrollRect组件
using UnityEngine.EventSystems;//引入事件系统命名空间，用于处理事件数据
using System;//引入 System命名空间，用于使用 Action<int> 事件

public class LevelScrollRect : MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
    private ScrollRect scroll;//ScrollRect组件实例
    private float[] pagePosition = new float[3]{0f,0.5f,1f};//页面位置数组

    private float targetPosition = 0f;//目标滚动位置，默认是0
    private bool isMoving = false;//是否正在拖动，默认是false
    private float moveSpeed = 7f;//移动速度，默认是0.1f
    // 定义一个事件：翻页完成时，把当前页码传出去
    // ✅ 使用Action<int>，接收一个int参数（页码）
    public event Action<int> OnPageChanged;
    void Start()
    {
        scroll = GetComponent<ScrollRect>();//获取ScrollRect组件实例
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            scroll.horizontalNormalizedPosition =  Mathf.Lerp(scroll.horizontalNormalizedPosition,targetPosition,moveSpeed * Time.deltaTime);
            if(Mathf.Abs(scroll.horizontalNormalizedPosition - targetPosition) < 0.01f)
            {
                isMoving = false;
                scroll.horizontalNormalizedPosition = targetPosition;
            }
        }
    }
    //继承IBeginDragHandler接口，监听拖动开始事件（OnBeginDrag方法）
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }
    //拖拽松开，自动吸附到最近的页面位置
    //继承IEndDragHandler接口，监听拖动结束事件（OnEndDrag方法）
    public void OnEndDrag(PointerEventData eventData)
    {
        // 计算当前滚动位置的索引
        float currentPosition = scroll.horizontalNormalizedPosition;
        // 计算最近的页面索引
        int index = 0;
        // 计算当前滚动位置与每个页面位置的偏移量
        float offset = Mathf.Abs(currentPosition - pagePosition[0]);
        // 遍历页面位置数组，找到最近的页面索引
        for(int i = 1; i < 3; i++)
        {
            if(Mathf.Abs(currentPosition-pagePosition[i]) < Mathf.Abs(offset))
            {
                index = i;
                offset = Mathf.Abs(currentPosition - pagePosition[i]);
            }
        }
        // 滚动到最近的页面
        targetPosition = pagePosition[index];
        isMoving = true;
        // ✅ 拖拽吸附完成，向外广播：当前页码是 index
        OnPageChanged?.Invoke(index);
    }
    /// <summary>
    /// Toggle点击跳转页面
    /// </summary>
    /// <param name="pageIndex">目标页码 0,1,2...</param>
    public void MoveToPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pagePosition.Length)
            return;

        isMoving = true;
        targetPosition = pagePosition[pageIndex];
        OnPageChanged?.Invoke(pageIndex);
    }
}
//就像站队：地上有 3 个定点，你随便站一个地方，有人帮你看，你离哪个定点最近，就把你拉到那个点上。