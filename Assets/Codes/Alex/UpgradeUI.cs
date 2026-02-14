using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UpgradeUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button damageBtn;
    public Button speedBtn;
    public Button attackBtn;
    public Button regenBtn;
    public Button hpBtn;

    [Header("Level Texts")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI regenText;
    public TextMeshProUGUI hpText;

    [Header("Title (Live Points Counter)")]
    public TextMeshProUGUI titleText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }

    // Public methods – wire these in Inspector OnClick
    public void UpgradeDamage() => UpgradeAndRefresh(UpgradeType.Damage);
    public void UpgradeSpeed()  => UpgradeAndRefresh(UpgradeType.PlayerSpeed);
    public void UpgradeAttackSpeed() => UpgradeAndRefresh(UpgradeType.AttackSpeed);
    public void UpgradeRegeneration() => UpgradeAndRefresh(UpgradeType.Regeneration);
    public void UpgradePlayerHp() => UpgradeAndRefresh(UpgradeType.PlayerHp);

    public void Show()
    {
        RefreshUI();
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    private void UpgradeAndRefresh(UpgradeType type)
    {
        UpgradeManager.Instance.Upgrade(type);
        RefreshUI();

        if (UpgradeManager.Instance.upgradePoints <= 0)
        {
            StartCoroutine(AutoFadeOut());
        }
    }

    public void RefreshUI()
    {
        if (damageText) damageText.text = $"Damage\nLv. {UpgradeManager.Instance.GetLevel(UpgradeType.Damage)}";
        if (speedText)  speedText.text  = $"MoveSpeed\nLv. {UpgradeManager.Instance.GetLevel(UpgradeType.PlayerSpeed)}";
        if (attackText) attackText.text = $"Attack Speed\nLv. {UpgradeManager.Instance.GetLevel(UpgradeType.AttackSpeed)}";
        if (regenText)  regenText.text  = $"Regen\nLv. {UpgradeManager.Instance.GetLevel(UpgradeType.Regeneration)}";
        if (hpText)     hpText.text     = $"HP\nLv. {UpgradeManager.Instance.GetLevel(UpgradeType.PlayerHp)}";

        if (titleText)
        {
            titleText.text = $"Points: {UpgradeManager.Instance.upgradePoints}";
        }
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }

    private IEnumerator AutoFadeOut()
    {
        yield return new WaitForSeconds(0.6f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}