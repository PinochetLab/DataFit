using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelLauncher : MonoBehaviour
{
    [SerializeField] private GraphBuilder graphBuilder;
    [SerializeField] private TEXDraw texDraw;
    [SerializeField] private TMP_InputField similarityInputField;
    [SerializeField] private TMP_InputField mseInputField;

    [SerializeField] private Image diodImage;
    [SerializeField] private Sprite blackDiodSprite;
    [SerializeField] private Sprite greenDiodSprite;
    [SerializeField] private Sprite redDiodSprite;

    private Dictionary<string, ParameterBlock> parameterBlocks;

    private static int percent = 0;

    private void Start()
    {
        LaunchLevel(Level.Levels[0]);
    }

    private void LaunchLevel(Level level)
    {
        texDraw.text = level.Expression.ToTex();
        var initialParameters = level.ParameterValues.ToDictionary(
            p => p.Key,
            p => p.Value.Initial
        );
        
        var rightParameters = level.ParameterValues.ToDictionary(
            p => p.Key,
            p => p.Value.Right
        );
        
        graphBuilder.SetUp(level.Expression, level.ViewRect, rightParameters);
        var similarity = graphBuilder.UpdateParameters(initialParameters);
        UpdateSimilarity(similarity);

        /*parameterBlocks = level.ParameterValues.ToDictionary(
            p => p.Key,
            p =>
            {
                var parameterBlock = ParameterBlockBox.CreateOne();
                parameterBlock.SetUp(p.Key, p.Value, UpdateValues);
                return parameterBlock;
            }
        );*/
        var names = level.ParameterValues.Keys.ToList();
        for (int i = 'A'; i < 'Z' + 1 && names.Count < 4; i++)
        {
            var s = "" + (char)i;
            if (!names.Contains(s))
            {
                names.Add(s);
            }
        }

        parameterBlocks = names.OrderBy(s => s).ToDictionary(
            s => s,
            s =>
            {
                var parameterBlock = ParameterBlockBox.CreateOne();
                if (level.ParameterValues.TryGetValue(s, out var value))
                {
                    parameterBlock.SetUp(s, value, UpdateValues);
                }
                else
                {
                    parameterBlock.SetUp(s, ValueStruct.Random(), delegate {  });
                }
                return parameterBlock;
            }
            );
        /*for (var i = parameterBlocks.Count; i < 4; i++)
        {
            ParameterBlockBox.CreateEmpty();
        }
        ParameterBlockBox.Shuffle();*/
    }

    private void UpdateSimilarity(Similarity similarity)
    {
        percent = similarity.Percent;
        similarityInputField.text = similarity.Percent.ToString();
        mseInputField.text = similarity.MSE.ToString(CultureInfo.InvariantCulture);
    }

    private void UpdateValues()
    {
        var values = parameterBlocks.ToDictionary(
            p => p.Key,
            p => p.Value.Value
        );
        var similarity = graphBuilder.UpdateParameters(values);
        UpdateSimilarity(similarity);
    }

    public void TryPassLevel()
    {
        StopCoroutine(WrongAnswer());
        StartCoroutine(percent == 100 ? RightAnswer() : WrongAnswer());
    }

    private IEnumerator RightAnswer()
    {
        diodImage.sprite = greenDiodSprite;
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator WrongAnswer()
    {
        const float dt = 0.1f;
        diodImage.sprite = redDiodSprite;
        yield return new WaitForSeconds(dt);
        diodImage.sprite = blackDiodSprite;
        yield return new WaitForSeconds(dt);
        diodImage.sprite = redDiodSprite;
        yield return new WaitForSeconds(dt);
        diodImage.sprite = blackDiodSprite;
    }
}
