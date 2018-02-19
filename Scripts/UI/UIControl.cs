using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    [SerializeField] UIData _uiData;
    public static UIData uiData;
    // Use this for initialization
    void Awake()
    {
        uiData = _uiData;
    }

    public static void SetWarningTerritory(float dist)
    {
        string message = UIControl.uiData.textWarning.Replace("#x#", dist.ToString("0.00"));
        UIControl.uiData.warningTerritory.text = message;
    }

    public static void SetWarningTerritory()
    {
        UIControl.uiData.warningTerritory.text = "";
    }

    public static void SetWeapon(Sprite sprite, string description)
    {
        UIControl.uiData.weaponIcon.sprite = sprite;
        UIControl.uiData.descriptionWeapon.text = description;
    }

    public static void SetHp(int _hp, int _maxHP)
    {
        string message = "x / y".Replace("x", _hp.ToString()).Replace("y", _maxHP.ToString());
        UIControl.uiData.hp.text = message;
    }

    [System.Serializable]
    public class UIData
    {
        [Header("Warning Territory")]
        public TMP_Text warningTerritory;
        public string textWarning;
        [Header("Weapon Data")]
        public Image weaponIcon;
        public TMP_Text descriptionWeapon;
        [Header("HP")]
        public TMP_Text hp;
    }
}
