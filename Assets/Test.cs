using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform c;

    // Update is called once per frame
    void Update()
    {
        if (a == null || b == null || c == null) return;

        float angle = CalculateAngleOffset(a, b);

        c.localRotation = Quaternion.Euler(0f, 0f, angle);

        c.localScale = new Vector3(1, (a.position.x <= b.position.x) ? 1 : -1, 1);
    }

    private static float CalculateAngleOffset(Transform a, Transform b)
    {
        Vector3 directionToEnemy = b.position - a.position;
        float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
        return angle;
    }
}
