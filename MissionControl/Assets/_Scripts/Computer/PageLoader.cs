using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageLoader : MonoBehaviour
{
    [SerializeField] List<string> pageTexts;

    [Header("Animation")]
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    [SerializeField] ScrambleMode scrambleMode;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI text_TMP;

    [Header("Game Objects")]
    [SerializeField] Button nextPageButton;
    [SerializeField] Button previousPageButton;

    int currentPage;

    Sequence sequence;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPage(0);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPreviousPage()
        => LoadPage(currentPage - 1);

    public void LoadNextPage()
        => LoadPage(currentPage + 1);

    /// <summary>
    ///     Loads the page number text, starting from 0
    /// </summary>
    void LoadPage(int newPage)
    {
        if (newPage < 0 || newPage >= pageTexts.Count)
            return;

        previousPageButton.gameObject.SetActive(newPage != 0);
        nextPageButton.gameObject.SetActive(newPage != pageTexts.Count - 1);

        currentPage = newPage;

        sequence?.Kill();
        sequence = DOTween.Sequence();

        var newText = pageTexts[newPage];

        text_TMP.DOText(newText, duration, true, scrambleMode, null);
    }
}
