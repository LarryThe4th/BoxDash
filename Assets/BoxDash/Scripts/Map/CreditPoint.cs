using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CreditPoint : MonoBehaviour
{
    #region Private variables
    private AudioSource m_AudioSource;
    #endregion

    public void EnableCreditPoint(bool enable) {
        this.gameObject.SetActive(enable);
    }

    public void OnGetPoint() {
        if (!m_AudioSource) m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.Play();
        // Disable the object.
        this.gameObject.SetActive(false);
    }
}
