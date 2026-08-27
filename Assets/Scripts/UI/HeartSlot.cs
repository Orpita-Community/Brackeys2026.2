using UnityEngine;
using UnityEngine.UI;

public class HeartSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    public void SetFilled(bool filled)
    {
        if (image == null) return;
        image.sprite = filled ? fullHeartSprite : emptyHeartSprite;
        image.color = filled ? Color.white : new Color(1f, 1f, 1f, 0.25f);
    }
}
