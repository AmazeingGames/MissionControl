using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PageDisplayer
{
    readonly List<IPageButton> pageButtons = new();
    readonly Dictionary<List<IPageData>, int> PageDataToLastPageNumber = new(); 
    readonly List<IPageData> selectedPageData = new();

    // The page data that is currently being displayed
    List<IPageData> currentlyDisplayedData = null;

    LogsManager Logs => LogsManager.Instance;

    public PageDisplayer(List<IPageButton> pageButtons)
    {
        this.pageButtons = pageButtons;
    }

    List<IPageData> previouslyDisplayedData;

    public void DisplayPage<Data, Button>(int pageNumber, List<Data> pageDataToDisplay, bool multiplyByCount) where Data : IPageData where Button : IPageButton
    {
        if (pageDataToDisplay == null)
        {
            LogsManager.LogWarning(Logs.PageManagerLoggingObject, "Page to display is null");
            return;
        }

        if (pageNumber < 0 || (pageNumber * pageButtons.Count > pageDataToDisplay.Count && multiplyByCount))
        {
            LogsManager.LogWarning(Logs.PageManagerLoggingObject, "Page number is out of list range");
            return;
        }

        List<IPageData> pageData = pageDataToDisplay.Cast<IPageData>().ToList();

        Assert.IsNotNull(pageData, "Page data should not be null");

        if (TryGetMatchingKey(PageDataToLastPageNumber, tryMatch: pageData, match: out List<IPageData> lastPage))
            PageDataToLastPageNumber[lastPage] = pageNumber;
        else
            PageDataToLastPageNumber.Add(pageData, pageNumber);

        currentlyDisplayedData = pageData;

        for (int i = 0; i < pageButtons.Count; i++)
        {
            Button button = (Button)pageButtons[i];

            int pageDataToSelect = i + (pageNumber * pageButtons.Count);
            if (pageDataToSelect < pageDataToDisplay.Count)
            {
                IPageData data = pageDataToDisplay[pageDataToSelect];
                button.Initialize(data);
            }
            else
                button.Initialize<FateData>(null);
        }

        LogsManager.Log(Logs.PageManagerLoggingObject, $"Displayed page {pageNumber} for fates list {pageDataToDisplay}");
        LogsManager.Log(Logs.PageManagerLoggingObject, $"fatesToPageNumber dictionary count: {PageDataToLastPageNumber.Count}");
        LogsManager.Log(Logs.PageManagerLoggingObject, $"Selected Fates Count on Display Fates: {selectedPageData.Count}");
    }

    public void DisplayNext<Data, Button>(bool multiplyByCount) where Data : IPageData where Button : IPageButton
    {
        int nextPageNumber = LastViewedPage(currentlyDisplayedData) + 1;

        DisplayPage<IPageData, Button>(nextPageNumber, currentlyDisplayedData, multiplyByCount);
    }

    public void DisplayPrevious<Data, Button>(bool multiplyByCount) where Data : IPageData where Button : IPageButton
    {
        int previousPageNumber = LastViewedPage(currentlyDisplayedData) - 1;
        
        DisplayPage<IPageData, Button>(previousPageNumber, currentlyDisplayedData, multiplyByCount);
    }
    public int LastViewedPage<T>(List<T> pageDataToView) where T : IPageData
    {
        if (pageDataToView == null)
        {
            LogsManager.Log(Logs.PageManagerLoggingObject, "Trying to view a null list of fates");
            return -1;
        }

        if (TryGetMatchingKey(PageDataToLastPageNumber, tryMatch: pageDataToView.Cast<IPageData>().ToList(), out List<IPageData> match))
            return PageDataToLastPageNumber[match];
        else
        {
            LogsManager.Log(Logs.PageManagerLoggingObject, "Page data is not contained in Data To Page Number dictionary");
            return 0;
        }
        
    }

    bool TryGetMatchingKey<key, value>(Dictionary<List<key>, value> keyValuePairs, List<key> tryMatch, out List<key> match)
    {
        bool foundMatch = false;
        match = default;
        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            List<List<key>> keys = keyValuePairs.Keys.ToList();

            if (keys[i].SequenceEqual(tryMatch))
            {
                foundMatch = true;
                match = keys[i];
            }
        }
        return foundMatch;
    }
}
