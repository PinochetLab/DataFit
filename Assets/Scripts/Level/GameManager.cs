using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager inst;

    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform levelButtonParent;

    [SerializeField] private TMP_InputField percentField;
    [SerializeField] private TMP_InputField mseInputField;
    
    [SerializeField] private CanvasGroup screenCanvasGroup;
    [SerializeField] private CanvasGroup formulaCanvasGroup;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private AudioSource labNoise;

    private bool _isMenu = true;

    public static bool IsMenu => inst._isMenu;

    private void Start()
    {
        StartMenu();
    }

    public static void Switch()
    {
        inst.StopAllCoroutines();
        inst._isMenu = !inst._isMenu;
        inst.StartCoroutine(inst._isMenu ? inst.SwitchToMenu() : inst.SwitchToLevel(-1));
    }

    private IEnumerator SwitchScreen(bool game)
    {
        var finalAlpha = game ? 0 : 1;
        var startAlpha = screenCanvasGroup.alpha;
        const float d = 0.5f;
        const int n = 50;
        for (var i = 0; i <= n; i++)
        {
            var t = (float)i / n;
            screenCanvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, t);
            formulaCanvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, t);
            yield return new WaitForSeconds(d / n);
        }
    }

    private void StartMenu()
    {
        mainMenu.SetActive(true);
        percentField.text = "";
        mseInputField.text = "";
        ParameterBlockBox.Clear();
        for (var i = 0; i < 4; i++)
        {
            ParameterBlockBox.CreateEmpty();
        }
        var levelCount = Level.Levels.Count;
        while (levelButtonParent.childCount > 0)
        {
            DestroyImmediate(levelButtonParent.GetChild(0).gameObject);
        }
        for (var i = 0; i < levelCount; i++)
        {
            var levelButton = Instantiate(levelButtonPrefab, levelButtonParent).GetComponent<LevelButton>();
            levelButton.SetUp(i, ProgressStorage.IsLevelOpened(i));
        }
    }

    private IEnumerator SwitchToMenu()
    {
        GlobalSoundPlayer.Play("PowerOff");
        mainMenu.SetActive(true);
        StartMenu();
        yield return SwitchScreen(false);
        mainMenuPanel.SetActive(true);
        labNoise.mute = true;
    }

    private IEnumerator SwitchToLevel()
    {
        GlobalSoundPlayer.Play("PowerOn");
        labNoise.mute = false;
        mainMenuPanel.SetActive(false);
        yield return SwitchScreen(true);
        inst.mainMenu.SetActive(false);
    }

    private void Awake()
    {
        inst = this;
    }

    public static void SwitchLevel(int level)
    {
        inst.StartCoroutine(inst.SwitchToLevel(level));
    }

    private IEnumerator SwitchToLevel(int level)
    {
        if (level == -1) level = ProgressStorage.NextLevel();
        LevelLauncher.LaunchLevel(level);
        yield return SwitchToLevel();
    }
}
