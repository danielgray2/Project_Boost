using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [Range(0,1)]
    [SerializeField]
    float movementFactor;
    [SerializeField] float period = 2f;

    private Vector3 startingPos;
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon){
            period = 0.00001f;
        }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2f;
        float rawSineWave = Mathf.Sin(cycles * tau);
        float movementFactor = rawSineWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
