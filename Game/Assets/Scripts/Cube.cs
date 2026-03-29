using UnityEngine;

public class Cube : MonoBehaviour
{
    private bool _detouched;

    public int Id { get; set; }

    public void Detach()
    {
        if (_detouched)
            return;

        _detouched = true;
        GetComponentInParent<Entity>().DetachCube(this);
    }

    public void DestroyCube()
    {
        Detach();

        // Get Score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(1);

        // Turn off 'Physics'
        GetComponent<Rigidbody>().isKinematic = true;
        // Turn off 'Collider'
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 0.2f);
    }
}