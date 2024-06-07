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
        if (!transform.parent.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            return;
        }
        Bounds Boxbounds = sr.bounds;

        // 取最大矩形框
        //Vector2 min = new Vector2(Mathf.Floor(Boxbounds.min.x), Mathf.Floor(Boxbounds.min.y));
        //Vector2 max = new Vector2(Mathf.Ceil(Boxbounds.max.x), Mathf.Ceil(Boxbounds.max.y)); 
        Vector2 min = new Vector2(Boxbounds.min.x, Boxbounds.min.y);
        Vector2 max = new Vector2(Boxbounds.max.x, Boxbounds.max.y);

        float duration = 0.5f;  // 动画时间

        // 四个角落的开始的位置
        float offset1 = 0.5f;    
        Vector2 sp_bl = new Vector2(min.x, min.y) + new Vector2(-offset1, -offset1);
        Vector2 sp_br = new Vector2(max.x, min.y) + new Vector2(offset1, -offset1);
        Vector2 sp_tl = new Vector2(min.x, max.y) + new Vector2(-offset1, offset1);
        Vector2 sp_tr = new Vector2(max.x, max.y) + new Vector2(offset1, offset1);

        // 四个角落的结束位置
        float offset2 = 0.1f;
        Vector2 ep_bl = new Vector2(min.x + offset2, min.y + offset2);
        Vector2 ep_br = new Vector2(max.x - offset2, min.y + offset2);
        Vector2 ep_tl = new Vector2(min.x + offset2, max.y - offset2);
        Vector2 ep_tr = new Vector2(max.x - offset2, max.y - offset2);

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
        // 不跟随父类缩放
        if (transform.parent.localScale.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // 不跟随父类旋转
        transform.rotation = Quaternion.identity;
    }

}
