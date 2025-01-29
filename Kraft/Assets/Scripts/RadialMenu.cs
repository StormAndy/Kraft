using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    public GameObject menuCanvas;           // The GameObject containing the radial menu and panels
    public List<Button> menuButtons;        // List of the buttons representing the radial menu options
    public Color defaultColor = Color.white;     // Default color for buttons
    public Color highlightColor = Color.yellow;  // Highlighted color for buttons
    public Color inactiveColor = Color.gray;     // Inactive color for buttons when not selected
    private bool isMenuActive = false;      // Whether the menu is currently active
    private Vector3 initialClickPosition;   // The position of the mouse when the player first clicks
    private Vector3 centerScreen;           // The center of the screen for angle calculation
    private float anglePerSection = 45f;    // Angle per section for the 8 options (360/8)

    private int currentSelection = -1;     // Track the current selected option (initially no selection)

    public bool alwaysCentered = true;     // Toggle option to always center the menu or not
    public float screenMargin = 50f;       // Buffer margin to allow slight overflow beyond screen edges
    public float holdThreshold = 0.33f;    // Time threshold to hold the mouse button (in seconds)

    private float mouseHoldTime = 0f;      // Tracks how long the right mouse button has been held
    private bool isHoldingRightClick = false; // Whether the right mouse button is being held down
    private bool holdClick = false;        // Whether the mouse button is held long enough for menu
    public bool useRightClickOnly = true;  // Option to force the menu to appear with a simple right-click (no hold)

    private RectTransform menuRectTransform; // Reference to the menu's RectTransform

    void Start()
    {
        // Initially, set all buttons to inactive color
        SetButtonColorState(inactiveColor);
        menuCanvas.SetActive(false);  // Make sure the menu is initially hidden
        menuRectTransform = menuCanvas.GetComponent<RectTransform>(); // Get the RectTransform of the menu
        centerScreen = new Vector3(Screen.width / 2, Screen.height / 2); // Set the center of the screen
    }

    void Update()
    {
        // Handle right mouse button (Mouse Button 2) down and up
        if (Input.GetMouseButtonDown(1)) // Right-click mouse button down (Mouse Button 2)
        {
            isHoldingRightClick = true;
            mouseHoldTime = 0f; // Reset the timer when the right-click is first pressed
            holdClick = false;  // Reset the flag
        }

        if (useRightClickOnly)
        {
            // For right-click only (no hold), immediately activate the menu on right-click down
            if (Input.GetMouseButtonDown(1) && !isMenuActive)
            {
                isMenuActive = true;
                menuCanvas.SetActive(true); // Show the radial menu
                initialClickPosition = Input.mousePosition;

                // Set the center of the menu based on the `alwaysCentered` flag
                if (!alwaysCentered)
                {
                    PositionMenuAtClick(initialClickPosition); // Use the initial click position for menu placement
                    Debug.Log("Menu positioned at mouse click: " + centerScreen);  // Debugging the position
                }
                else
                {
                    centerScreen = new Vector3(Screen.width / 2, Screen.height / 2); // Center at the screen
                    Debug.Log("Menu centered on screen: " + centerScreen);  // Debugging the center
                }
            }

            // Right-click release hides the menu immediately
            if (Input.GetMouseButtonUp(1) && isMenuActive)
            {
                menuCanvas.SetActive(false); // Hide the menu
                isMenuActive = false;
            }
        }
        else
        {
            // Handle the behavior if holding right-click is required
            if (isHoldingRightClick)
            {
                mouseHoldTime += Time.deltaTime; // Increment timer while right-click is held down

                // Check if the right mouse button has been held for longer than the threshold
                if (mouseHoldTime >= holdThreshold && !holdClick)
                {
                    holdClick = true;
                    isMenuActive = true;
                    menuCanvas.SetActive(true); // Show the radial menu
                    initialClickPosition = Input.mousePosition;

                    // Set the center of the menu based on the `alwaysCentered` flag
                    if (!alwaysCentered)
                    {
                        PositionMenuAtClick(initialClickPosition); // Use the initial click position for menu placement
                        Debug.Log("Menu positioned at mouse click: " + centerScreen);  // Debugging the position
                    }
                    else
                    {
                        centerScreen = new Vector3(Screen.width / 2, Screen.height / 2); // Center at the screen
                        Debug.Log("Menu centered on screen: " + centerScreen);  // Debugging the center
                    }
                }
            }

            if (Input.GetMouseButtonUp(1)) // Right-click mouse button up (Mouse Button 2)
            {
                isHoldingRightClick = false;
                mouseHoldTime = 0f; // Reset the timer on mouse button release

                if (!holdClick)
                {
                    // If right-click was not held long enough, do not show the menu
                    Debug.Log("Right-click clicked, not held enough for menu activation.");
                }

                if (isMenuActive)
                {
                    menuCanvas.SetActive(false); // Hide the menu when right-click is released
                    isMenuActive = false; // Reset menu state
                }
            }
        }

        if (isMenuActive)
        {
            Vector3 currentMousePosition = Input.mousePosition;

            // **Use initialClickPosition for direction calculation if alwaysCentered is false**
            Vector3 direction = currentMousePosition - initialClickPosition; // Calculate direction relative to initial click
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Normalize the angle to be between 0 and 360
            if (angle < 0) angle += 360;

            // Adjust the angle so that 0° is at the top (12 o'clock) and angle increases clockwise
            angle = (360 - angle) % 360; // Invert the angle to match clockwise direction

            // Calculate which section of the radial menu is selected
            int selectedIndex = Mathf.FloorToInt(angle / anglePerSection);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, 7); // Ensure it's between 0 and 7

            // Only update the selection if it's different from the current one
            if (selectedIndex != currentSelection)
            {
                currentSelection = selectedIndex;
                HighlightButton(currentSelection);
            }

            if (Input.GetMouseButtonUp(0)) // When mouse button is released (left click)
            {
                // When the mouse button is released, confirm the selection
                OnOptionSelected(currentSelection);
                isMenuActive = false; // Hide the radial menu after selection
                menuCanvas.SetActive(false); // Hide the menu
            }
        }
    }

    // <summary> Position the menu at the click position and ensure it doesn't go off the screen </summary>
    private void PositionMenuAtClick(Vector3 clickPosition)
    {
        Vector2 menuSize = menuRectTransform.sizeDelta; // Get the size of the menu panel
        Vector3 menuScale = menuRectTransform.localScale; // Get the scale of the menu

        // Apply inverse scale to the screen margin (making margin increase when scaled down)
        float scaledMarginX = screenMargin * (1 / menuScale.x); // Scale margin by inverse of x-axis scale
        float scaledMarginY = screenMargin * (1 / menuScale.y); // Scale margin by inverse of y-axis scale

        // Calculate the desired position of the menu
        float xPos = clickPosition.x;
        float yPos = clickPosition.y;

        // Allow the menu to slightly overflow based on the scaled margin
        if (xPos + menuSize.x / 2 > Screen.width + scaledMarginX) xPos = Screen.width + scaledMarginX - menuSize.x / 2; // Adjust to the right edge
        if (yPos + menuSize.y / 2 > Screen.height + scaledMarginY) yPos = Screen.height + scaledMarginY - menuSize.y / 2; // Adjust to the bottom edge

        // Ensure the menu doesn't go off the left or top edges of the screen
        if (xPos - menuSize.x / 2 < -scaledMarginX) xPos = -scaledMarginX + menuSize.x / 2; // Adjust to the left edge
        if (yPos - menuSize.y / 2 < -scaledMarginY) yPos = -scaledMarginY + menuSize.y / 2; // Adjust to the top edge

        // Apply the calculated position to the menu
        menuRectTransform.position = new Vector2(xPos, yPos);
    }

    // <summary> Highlight the currently selected button </summary>
    private void HighlightButton(int index)
    {
        SetButtonColorState(inactiveColor);  // Reset all to inactive color
        if (index >= 0 && index < menuButtons.Count)
            menuButtons[index].GetComponent<Image>().color = highlightColor; // Set the highlighted button to the highlight color
    }

    // <summary> Set the color state for all buttons </summary>
    private void SetButtonColorState(Color color)
    {
        foreach (Button button in menuButtons)
            button.GetComponent<Image>().color = color;
    }

    // <summary> Handle the selection of an option </summary>
    private void OnOptionSelected(int selectedOption)
    {
        if (selectedOption >= 0)
            Debug.Log("Option " + selectedOption + " selected!");
        

        //Options logic here
    }
}
