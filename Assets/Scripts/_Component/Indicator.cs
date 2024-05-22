using UnityEngine;

public class Indicator : MonoBehaviour 
{
    /// <summary>
    /// 获取 gameObject 其中的 indicator，如果没有会创建一个
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="indicator"></param>
    /// <returns></returns>
    public static bool TryGetIndicator(GameObject gameObject, out Component indicator)
    {
        indicator = null;
        if (gameObject.transform.Find("Indicator"))
        {
            indicator = gameObject.transform.Find("Indicator").GetComponent<Indicator>();
            indicator.gameObject.SetActive(true);
        }
        if (indicator == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
            indicator = go.AddComponent<Indicator>();
            indicator.gameObject.name = "Indicator";
            indicator.gameObject.SetActive(true);
            indicator.GetComponentInChildren<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
        }
        if (indicator == null)
        {
            Debug.LogError("Failed to find or create the Indicator GameObject.");
            return false;
        }
        return true;
    }
}
