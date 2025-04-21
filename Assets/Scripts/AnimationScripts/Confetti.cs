using UnityEngine;

public class Confetti : MonoBehaviour
{
    //public float speed = 1000.0f;
    void Update()
    {
        transform.Translate(new Vector2(0, 1) * 100 * Time.deltaTime);
        Destroy(gameObject, 3f);
    }


}
