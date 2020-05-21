using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip levelChangeJingle;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] float delay = 1f;
    Rigidbody rigidBody;
    bool collisionsOn = true;
    AudioSource audioSource;

    enum State {Alive, Dying, Transcending};
    State currentState = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision){
        if(currentState != State.Alive || !collisionsOn){return;}

        switch(collision.gameObject.tag){
            case "Friendly":
                break;
            case "Landing":
                currentState = State.Transcending;
                successParticles.Play();
                audioSource.Stop();
                Invoke("LoadNextScene", delay);
                break;
            default:
                currentState = State.Dying;
                explosionParticles.Play();
                audioSource.Stop();
                audioSource.PlayOneShot(explosionSound);
                Invoke("LoadFirstLevel", delay);
                break;
        }
    }

    private void LoadNextScene(){
        audioSource.PlayOneShot(levelChangeJingle);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        sceneIndex = (sceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(sceneIndex);
    }

    private void LoadFirstLevel(){
        audioSource.PlayOneShot(levelChangeJingle);
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput(){
        if(this.currentState == State.Alive){
            RespondToThrustInput();
            RespondToRotateInput();
            if(Debug.isDebugBuild){
                CheckForDebugKeys();
            }
        }
    }

    private void CheckForDebugKeys(){
        if(Input.GetKey(KeyCode.L)){
            LoadNextScene();
        }else if(Input.GetKey(KeyCode.C)){
            collisionsOn = !collisionsOn;
        }
    }

    private void RespondToThrustInput(){
        if(Input.GetKey(KeyCode.Space)){
            ApplyThrust();
        }else{
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(){
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if(!audioSource.isPlaying){
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput(){
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        rigidBody.freezeRotation = true;
        if(Input.GetKey(KeyCode.LeftArrow)){
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }else if (Input.GetKey(KeyCode.RightArrow)){
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }
}
