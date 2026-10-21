using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EyeMoT
{
    public class PopupUI : Singleton<PopupUI>
    {
        [Header("Resources")]
        [SerializeField] private Animator _panelAnimator;
        [SerializeField] private GameObject _bgPanel;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite[] _typeIcons;
        [SerializeField] private Button okButton;
        [SerializeField] private Button noButton;

        string panelFadeIn = "Panel In";
        string panelFadeOut = "Panel Out";


        public static void OnVisible(string title, string description, Type type, UnityEngine.Events.UnityAction onClose = null)
        {
            Instance.noButton.gameObject.SetActive(type == Type.Confirm);
            Instance.title.text = title;
            Instance.description.text = description;
            Instance.icon.sprite = Instance._typeIcons[(int)type];

            Instance._panelAnimator.Play(Instance.panelFadeIn);
            Instance._bgPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            Instance.okButton.onClick.RemoveAllListeners();
            Instance.okButton.onClick.AddListener(() =>
            {
                CloseButton();
                if (onClose != null)
                {
                    onClose.Invoke();
                }
            });
        }


        public static void CloseButton()
        {
            Instance._panelAnimator.Play(Instance.panelFadeOut);
            Instance._bgPanel.SetActive(false);
        }

        public enum Type
        {
            Confirm,
            Alert,
        }
    }
}