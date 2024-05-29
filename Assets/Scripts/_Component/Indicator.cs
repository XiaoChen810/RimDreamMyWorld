using DG.Tweening;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private Transform selectbox_bl;
    [SerializeField] private Transform selectbox_br;
    [SerializeField] private Transform selectbox_tl;
    [SerializeField] private Transform selectbox_tr;

    private void Start()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingLayerName = "Above";
            sr.sortingOrder = 1;
        }
    }

    /// <summary>
    /// 一个边框括起父类的动画
    /// </summary>
    public void DoAnim()
    {
        Bounds Boxbounds = transform.parent.GetComponent<SpriteRenderer>().bounds;

        float offset = 0.5f;    // 动画偏移量
        float duration = 0.5f;  // 动画时间

        // 四个角落的开始的位置
        Vector2 sp_bl = new Vector2(Boxbounds.min.x, Boxbounds.min.y) + new Vector2(-offset, -offset);
        Vector2 sp_br = new Vector2(Boxbounds.max.x, Boxbounds.min.y) + new Vector2(offset, -offset);
        Vector2 sp_tl = new Vector2(Boxbounds.min.x, Boxbounds.max.y) + new Vector2(-offset, offset);
        Vector2 sp_tr = new Vector2(Boxbounds.max.x, Boxbounds.max.y) + new Vector2(offset, offset);

        // 四个角落的结束位置
        Vector2 ep_bl = new Vector2(Boxbounds.min.x, Boxbounds.min.y);
        Vector2 ep_br = new Vector2(Boxbounds.max.x, Boxbounds.min.y);
        Vector2 ep_tl = new Vector2(Boxbounds.min.x, Boxbounds.max.y);
        Vector2 ep_tr = new Vector2(Boxbounds.max.x, Boxbounds.max.y);

        // 执行动画
        selectbox_bl.position = sp_bl;
        selectbox_br.position = sp_br;
        selectbox_tl.position = sp_tl;
        selectbox_tr.position = sp_tr;

        selectbox_bl.DOMove(ep_bl, duration);
        selectbox_br.DOMove(ep_br, duration);
        selectbox_tl.DOMove(ep_tl, duration);
        selectbox_tr.DOMove(ep_tr, duration);
    }

    private void Update()
    {
        if(transform.parent.localScale.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }
}
