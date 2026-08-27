using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject faceDownOverlay;

    [SerializeField] private Color playableColor = Color.white;
    [SerializeField] private Color unplayableColor = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private float selectedLiftPixels = 30f;

    public event Action<CardView> OnClicked;
    public Card_DataSO CardData { get; private set; }

    private RectTransform rectTransform;
    private Vector2 basePosition;
    private float baseRotationZ;
    private bool interactable;
    private bool isSelected;

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

    public void SetCardData(Card_DataSO card, bool faceUp)
    {
        CardData = card;

        if (faceDownOverlay != null) faceDownOverlay.SetActive(!faceUp);

        if (!faceUp)
        {
            if (valueText != null) valueText.text = string.Empty;
            if (iconImage != null) iconImage.sprite = null;
            return;
        }

        if (valueText != null)
            valueText.text = card.cardType == CardType.Number ? card.numberValue.ToString() : card.cardName;

        if (iconImage != null) iconImage.sprite = card.cardIcon;
    }

    public void SetLayout(Vector2 anchoredPosition, float rotationZ)
    {
        basePosition = anchoredPosition;
        baseRotationZ = rotationZ;
        ApplyTransform();
    }

    public void SetInteractable(bool value) => interactable = value;

    public void SetPlayable(bool playable)
    {
        if (background != null) background.color = playable ? playableColor : unplayableColor;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        ApplyTransform();
    }

    private void ApplyTransform()
    {
        if (rectTransform == null) return;
        Vector2 pos = basePosition + (isSelected ? Vector2.up * selectedLiftPixels : Vector2.zero);
        rectTransform.anchoredPosition = pos;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, baseRotationZ);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        OnClicked?.Invoke(this);
    }
}
