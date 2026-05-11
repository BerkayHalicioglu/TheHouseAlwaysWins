using UnityEngine;

public class SuspicionIndicator : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main == null)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetRenderersEnabled(true);
    }

    public void Hide()
    {
        SetRenderersEnabled(false);
        gameObject.SetActive(false);
    }

    private void SetRenderersEnabled(bool isEnabled)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isEnabled;
        }
    }
}
