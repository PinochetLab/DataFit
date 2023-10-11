using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private TMP_Text _text;
    private Image _image;

    private int _level;
    private bool _opened;

    [SerializeField] private Image lockImage;

    public void SetUp(int level, bool opened = false)
    {
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();
        _text.text = (level + 1).ToString();
        _text.color = Color.green;
        lockImage.color = Color.green;
        _text.gameObject.SetActive(opened);
        lockImage.gameObject.SetActive(!opened);
        _level = level;
        _opened = opened;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = Color.black;
        lockImage.color = Color.black;
        _image.color = Color.green;
        GlobalSoundPlayer.Play("MenuButtonHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = Color.green;
        lockImage.color = Color.green;
        _image.color = Color.clear;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_opened) return;
        //GlobalSoundPlayer.Play("MenuButtonClick");
        GameManager.Switch();
        GameManager.SwitchLevel(_level);
    }
}
