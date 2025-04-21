using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NotebookController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject notebookCanvas;
    public Button leftArrowButton;   // Keep button references for visual feedback
    public Button rightArrowButton;  // and clicking option

    [Header("Page Setup")]
    public TextMeshProUGUI leftPageTitle;
    public TextMeshProUGUI rightPageTitle;
    public TextMeshProUGUI leftPageContent;
    public TextMeshProUGUI rightPageContent;

    [Header("Input Settings")]
    public KeyCode toggleKey = KeyCode.N;
    public KeyCode previousPageKey = KeyCode.LeftArrow;
    public KeyCode nextPageKey = KeyCode.RightArrow;

    [System.Serializable]
    public class NotebookHint
    {
        public string hintText = "";
        public bool isEnabled = false;
        public string hintID = "";
    }

    [System.Serializable]
    public class NotebookPage
    {
        public string pageTitle = "";
        public List<NotebookHint> pageHints = new List<NotebookHint>();
    }

    [Header("Pages Configuration")]
    public List<NotebookPage> pages = new List<NotebookPage>();

    private int currentPageIndex = 0;

    private void Start()
    {
        // Hide notebook at start
        notebookCanvas.SetActive(false);

        // Setup button listeners
        leftArrowButton.onClick.AddListener(PreviousPage);
        rightArrowButton.onClick.AddListener(NextPage);
    }

    private void Update()
    {
        // Check for key press to toggle notebook
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleNotebook();
        }

        // Only process page navigation when notebook is open
        if (notebookCanvas.activeSelf)
        {
            // Check for left/right arrow keys
            if (Input.GetKeyDown(previousPageKey))
            {
                PreviousPage();
            }
            else if (Input.GetKeyDown(nextPageKey))
            {
                NextPage();
            }
        }
    }

    public void ToggleNotebook()
    {
        notebookCanvas.SetActive(!notebookCanvas.activeSelf);

        if (notebookCanvas.activeSelf)
        {
            UpdatePages();
        }
    }

    public void NextPage()
    {
        if (currentPageIndex < pages.Count - 2)
        {
            currentPageIndex += 2;
            UpdatePages();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex >= 2)
        {
            currentPageIndex -= 2;
            UpdatePages();
        }
    }

    private void UpdatePages()
    {
        // Update left page
        leftPageTitle.text = pages[currentPageIndex].pageTitle;

        // Combine all enabled hints for left page
        string leftContent = "";
        foreach (NotebookHint hint in pages[currentPageIndex].pageHints)
        {
            if (hint.isEnabled)
            {
                leftContent += hint.hintText + "\n\n";
            }
        }
        leftPageContent.text = leftContent.TrimEnd();

        // Update right page if available
        if (currentPageIndex + 1 < pages.Count)
        {
            rightPageTitle.text = pages[currentPageIndex + 1].pageTitle;

            // Combine all enabled hints for right page
            string rightContent = "";
            foreach (NotebookHint hint in pages[currentPageIndex + 1].pageHints)
            {
                if (hint.isEnabled)
                {
                    rightContent += hint.hintText + "\n\n";
                }
            }
            rightPageContent.text = rightContent.TrimEnd();

            rightPageTitle.gameObject.SetActive(true);
            rightPageContent.gameObject.SetActive(true);
        }
        else
        {
            // No right page available
            rightPageTitle.gameObject.SetActive(false);
            rightPageContent.gameObject.SetActive(false);
        }

        // Update arrow button states (visual feedback)
        leftArrowButton.interactable = (currentPageIndex >= 2);
        rightArrowButton.interactable = (currentPageIndex < pages.Count - 2);
    }

    // Enable a hint by its unique ID
    public void EnableHint(string hintID)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            foreach (NotebookHint hint in pages[i].pageHints)
            {
                if (hint.hintID == hintID)
                {
                    hint.isEnabled = true;

                    // If the notebook is open and this page is currently displayed, update it
                    if (notebookCanvas.activeSelf &&
                        (i == currentPageIndex || i == currentPageIndex + 1))
                    {
                        UpdatePages();
                    }

                    return;
                }
            }
        }

        Debug.LogWarning("Hint ID not found: " + hintID);
    }

    // Disable a hint by its unique ID
    public void DisableHint(string hintID)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            foreach (NotebookHint hint in pages[i].pageHints)
            {
                if (hint.hintID == hintID)
                {
                    hint.isEnabled = false;

                    // If the notebook is open and this page is currently displayed, update it
                    if (notebookCanvas.activeSelf &&
                        (i == currentPageIndex || i == currentPageIndex + 1))
                    {
                        UpdatePages();
                    }

                    return;
                }
            }
        }

        Debug.LogWarning("Hint ID not found: " + hintID);
    }
}