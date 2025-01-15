using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldManagerScript : MonoBehaviour
{
    #region VARIABLES
    #region Unity editor fields
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private GameObject inventoryBtnObj;
    [SerializeField] private InventoryContainerScript inventoryContainer;
    [SerializeField] private Scrollbar dialogueScrollbar;
    [SerializeField] private TimeSelectionScript timeSelectionScript;
    [Header("Main dialogue")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueOptionsContainer;
    [Header("Dialogue options")]
    [SerializeField] private Button travelBtn;
    [SerializeField] private Button searchBtn;
    [SerializeField] private Button restBtn;
    [SerializeField] private Button sleepBtn;
    //[SerializeField] private Button lightFireBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button restartGameBtn;
    #endregion

    private List<Button> dialogueBtns = new List<Button>();
    //private List<ResidentialEstate> litFireplaces = new List<ResidentialEstate>();
    private SceneStatus status = SceneStatus.Opening;
    private string[] roadPlacements = { "first", "second", "third", "fourth" };
    // Difficulty modifier
    private const int DIFFICULTY_MODIFIER_MAX = 130;
    private const int DIFFICULTY_MODIFIER_DEPLETION = -2;
    private const int DIFFICULTY_MODIFIER_MIN = 50;

    #region Day/night cycle
    private const int HOURS_IN_DAY = 24;
    private const int DAYTIME_START_HOUR = 6;
    private const int NIGHTTIME_START_HOUR = 20;
    private int timeHour = DAYTIME_START_HOUR;
    private int timeDay = 1;
    #endregion
    #region Temperature
    private const int TEMPERATURE_DAYTIME_MAX = 10;
    private const int TEMPERATURE_DAYTIME_MIN = -10;
    private const int TEMPERATURE_NIGHTTIME_DROP = -5;
    private int temperature = TEMPERATURE_DAYTIME_MAX;
    //private const int FIREPLACE_WARMTH = 10;
    private const int BED_WARMTH = 5;
    #endregion
    #region Player info
    private Area currentLocation;
    /* INVENTORY
     *  Key = Item index; Value = Amount of item
     *  For Food the value is multiplied by 100g (e.g. If the amount of Food is 3, that is 300g of Food)
     *  For Matches the value is multiplied by 10g (e.g. If the amount of Matches is 10, that is 100g of Matches)
     */
    private Dictionary<int, int> inventory;
    private Dictionary<ClothingSlot, int> equippedClothing;
    private const int WEIGHT_MAX = 12000;
    #region Levels
    private const int LEVELS_MAX = 100;
    private const int LEVELS_MIN = 0;
    private const int ENERGY_SLEEP_MAX = 90;
    private int satiation = LEVELS_MAX;
    private int energy = LEVELS_MAX;
    private int warmth = LEVELS_MAX;
    private const int LEVEL_EMPTY_PENALTY = -2;
    private int emptyLevels = 0;
    // Depletion/restore rates
    private const int SATIATION_DEPLETION_RATE = -8;
    private const int ENERGY_DEPLETION_RATE = -5;
    private const int ENERGY_RESTORATION_RATE = 10;
    private const int WARMTH_INSIDE_GAIN_MIN = 5;
    private const int WARMTH_INSIDE_GAIN_MAX = 10;
    #endregion
    #endregion
    #endregion

    #region METHODS
    private void Start()
    {
        Helper.difficultyModifier = DIFFICULTY_MODIFIER_MAX;
        // Create starting inventory & equipment
        inventory = PopulateStartingInventory();
        equippedClothing = PopulateStartingEquipment();
        // Create starting village
        Village v = new Village();
        Travel(v.districts[Helper.random.Next(v.districts.Count)]);
    }

    /// <summary>
    /// Gets total weight of all items in inventory.
    /// </summary>
    /// <returns>The total weight in grams.</returns>
    private int GetTotalWeight()
    {
        int totalWeight = 0;
        foreach (int itemIndex in inventory.Keys)
        {
            if (WorldItems.itemData[itemIndex] is Food)
            {
                totalWeight += inventory[itemIndex] * WorldItems.FOOD_GRAMS_MULTIPLIER;
            }
            else if (WorldItems.itemData[itemIndex].name == "Matches")
            {
                //totalWeight += inventory[itemIndex] * WorldItems.MATCHES_GRAMS_MULTIPLIER;
            }
            else { totalWeight += WorldItems.itemData[itemIndex].weight * inventory[itemIndex]; }
        }
        return totalWeight;
    }

    /// <summary>
    /// Whether the player is carrying over their max weight.
    /// </summary>
    /// <returns>True if the player is overburdened.</returns>
    private bool IsOverburdened()
    {
        if (GetTotalWeight() > WEIGHT_MAX) return true;
        return false;
    }

    /// <summary>
    /// Checks whether it's daytime or nighttime.
    /// </summary>
    /// <returns>True if it's daytime, false if it's nighttime.</returns>
    private bool IsDaytime()
    {
        return (timeHour >= 6) && (timeHour < 20);
    }

    #region Update methods
    /// <summary>
    /// Updates info displayed on game screen.
    /// </summary>
    private void UpdateGame()
    {
        // Clear old dialogue
        dialogueText.text = "";
        foreach (Button b in dialogueBtns) Destroy(b.gameObject);
        dialogueBtns.Clear();
        // Update game
        if (status == SceneStatus.Playing)
        {
            UpdateLocation();
        }
        else { UpdateDialogueScene(); }
        UpdateStats();
        inventoryContainer.Refresh(inventory);
        UpdateTotalWeight();
    }

    /// <summary>
    /// Refreshes game to show current location; it's description and nearby places the player can travel to.
    /// </summary>
    private void UpdateLocation()
    {
        // Display search & rest options for Shelters
        if (currentLocation is Shelter)
        {
            Shelter s = (Shelter)currentLocation;
            if (s is AbandonedCar)
            {
                // Abandoned car dialogue
                dialogueText.text = DialogueGenerator.InAbandonedCar();
                // Dialogue options
                Button b = CreateDialogueOption("Rest", "", restBtn);
                b.GetComponent<RestButtonScript>().worldMgr = this;
                if (inventory[WorldItems.GetItemIndex("Sleeping Bag")] >= 1)
                {
                    b = CreateDialogueOption("Sleep", "", sleepBtn);
                    b.GetComponent<SleepButtonScript>().worldMgr = this;
                }
            }
            else // Shelter dialogue
            {
                dialogueText.text = DialogueGenerator.InShelter(s);
                // Search option
                if (s.GetUnsearchedArea() > 0)
                {
                    string btnText = "Search (1 hour)";
                    if (s.GetUnsearchedArea() > 1) btnText = $"Search (1-{s.GetUnsearchedArea()} hours)";
                    Button b = CreateDialogueOption("Search", btnText, searchBtn);
                    b.GetComponent<SearchButtonScript>().worldMgr = this;
                }
                // Rest options
                if (energy < LEVELS_MAX)
                {
                    if (s is ResidentialEstate)
                    {
                        // Rest/sleep in bed
                        Button b = CreateDialogueOption("Rest", "", restBtn);
                        b.GetComponent<RestButtonScript>().worldMgr = this;
                        if (energy < ENERGY_SLEEP_MAX)
                        {
                            b = CreateDialogueOption("Sleep", "", sleepBtn);
                            b.GetComponent<SleepButtonScript>().worldMgr = this;
                        }
                    }
                    /*
                    else if (inventory[WorldItems.GetItemIndex("Sleeping Bag")] >= 1)
                    {
                        // Rest/sleep in sleeping bag
                        Button b = CreateDialogueOption("Rest", "", restBtn);
                        b.GetComponent<RestButtonScript>().worldMgr = this;
                        if (energy < ENERGY_SLEEP_MAX)
                        {
                            b = CreateDialogueOption("Sleep", "", sleepBtn);
                            b.GetComponent<SleepButtonScript>().worldMgr = this;
                        }
                    }
                    */
                    else
                    {
                        // Rest
                        Button b = CreateDialogueOption("Rest", "", restBtn);
                        b.GetComponent<RestButtonScript>().worldMgr = this;
                    }
                }
                // Fireplace option
                /*
                if (currentLocation is ResidentialEstate)
                {
                    ResidentialEstate re = (ResidentialEstate)currentLocation;
                    if (re.CanLightFireplace() && (inventory[WorldItems.GetItemIndex("Matches")] > 0))
                    {
                        // Can light fireplace
                        Button b = CreateDialogueOption("Light fire", "", lightFireBtn);
                        b.GetComponent<LightFireButtonScript>().worldMgr = this;
                    }
                    else if (re.fireplaceLit)
                    {
                        // Fireplace is already lit
                        dialogueText.text += "The fireplace is lit.";
                    }
                    else if (inventory[WorldItems.GetItemIndex("Matches")] <= 0)
                    {
                        // Player has no matches
                        dialogueText.text += "You do not have any matches to light the fireplace.";
                    } // Fireplace is out of fuel
                    else { dialogueText.text += "The fireplace is out of fuel."; }
                    
                }
                */
            }
            // Go back outside
            Area outside = s.GetConnections()[0];
            Button t = CreateDialogueOption(outside.name, "Go outside", travelBtn);
            t.GetComponent<TravelButtonScript>().worldMgr = this;
            t.GetComponent<TravelButtonScript>().destination = outside;
        }
        else if (currentLocation is District)
        {
            // District dialogue
            District d = (District)currentLocation;
            dialogueText.text = DialogueGenerator.InDistrict(d);
            // Dialogue options
            foreach (Area area in d.GetConnections())
            {
                string btnText = "";
                if (area is Shelter)
                {
                    btnText = $"Enter {area.name}";
                    Button b = CreateDialogueOption(area.name, btnText, travelBtn);
                    b.GetComponent<TravelButtonScript>().worldMgr = this;
                    b.GetComponent<TravelButtonScript>().destination = area;
                }
                else if (!IsOverburdened())
                {
                    if (area is District)
                    {
                        btnText = $"Go to {area.name}ern district (1 hour)";
                    }
                    else if (area is Road)
                    {
                        Road r = (Road)area;
                        btnText = $"Take the road out of the {d.parent.name} ({r.length} hour(s))";
                    }
                    Button b = CreateDialogueOption(area.name, btnText, travelBtn);
                    b.GetComponent<TravelButtonScript>().worldMgr = this;
                    b.GetComponent<TravelButtonScript>().destination = area;
                }
            }
        }
        else if (currentLocation is Road)
        {
            // Road dialogue
            Road r = (Road)currentLocation;
            dialogueText.text = DialogueGenerator.InRoad(r);
            // Dialogue options
            int roadPlacement = 0;
            foreach (Area area in r.GetConnections())
            {
                string btnText = "";
                if (area is AbandonedCar)
                {
                    btnText = "Open abandoned car";
                    Button b = CreateDialogueOption(area.name, btnText, travelBtn);
                    b.GetComponent<TravelButtonScript>().worldMgr = this;
                    b.GetComponent<TravelButtonScript>().destination = area;
                }
                else if (!IsOverburdened())
                {
                    if (area is Road)
                    {
                        Road cr = (Road)area;
                        btnText = $"Continue down road the {roadPlacements[roadPlacement]} road ({cr.length} hour(s))";
                        roadPlacement++;
                    }
                    else if (area is District)
                    {
                        btnText = $"Go back the way you came ({r.length} hour(s))";
                    }
                    Button b = CreateDialogueOption(area.name, btnText, travelBtn);
                    b.GetComponent<TravelButtonScript>().worldMgr = this;
                    b.GetComponent<TravelButtonScript>().destination = area;
                }
            }
        }
        // Player is overburdened
        if (IsOverburdened()) dialogueText.text += $"\n{DialogueGenerator.Overburdened()}";
    }

    /// <summary>
    /// Refresh stats display text.
    /// </summary>
    private void UpdateStats()
    {
        // Check whether it's daytime or nighttime
        string timeOfDay = "Daytime";
        if (!IsDaytime()) timeOfDay = "Nighttime";
        // Display stats
        statsText.text = $"Satiation: {satiation}%      Energy: {energy}%      Warmth: {warmth}%\n" +
            $"{timeOfDay} {timeHour}:00      Day {timeDay}      {temperature}°C";
    }

    /// <summary>
    /// Refresh total weight text.
    /// </summary>
    private void UpdateTotalWeight()
    {
        float totalWeight = (float)GetTotalWeight() / 1000f;
        weightText.text = $"Total Weight: {totalWeight}kg";
        if (GetTotalWeight() > WEIGHT_MAX) weightText.text += " - <b>Overburdened<b>";
    }

    /// <summary>
    /// Refreshes game to display dialogue scene.
    /// </summary>
    private void UpdateDialogueScene()
    {
        Button b = null;
        switch (status)
        {
            case SceneStatus.Opening:
                // Display dialogue & options
                dialogueText.text = DialogueGenerator.Opening();
                b = CreateDialogueOption("Continue", "", continueBtn);
                b.GetComponent<ContinueButtonScript>().worldMgr = this;
                b.GetComponent<ContinueButtonScript>().newScene = SceneStatus.HowToPlay;
                break;
            case SceneStatus.HowToPlay:
                // Display dialogue & options
                dialogueText.text = DialogueGenerator.HowToPlay();
                b = CreateDialogueOption("Continue", "Start", continueBtn);
                b.GetComponent<ContinueButtonScript>().worldMgr = this;
                b.GetComponent<ContinueButtonScript>().newScene = SceneStatus.Playing;
                // Enable stats and inventory button UI
                statsCanvas.SetActive(true);
                inventoryBtnObj.SetActive(true);
                //BUGFIX: For some reason the scrollbar always starts in the middle on this screen? So I've manually set it to 1.
                dialogueScrollbar.value = 1;
                break;
            case SceneStatus.GameOver:
                inventoryBtnObj.SetActive(false);
                // Display dialogue & options
                dialogueText.text = DialogueGenerator.GameEnd(timeDay);
                b = CreateDialogueOption("Restart Game", "", restartGameBtn);
                break;
        }
    }

    /// <summary>
    /// Continues to next dialogue scene.
    /// </summary>
    /// <param name="newScene">The new scene.</param>
    public void ContinueToNextScene(SceneStatus newScene)
    {
        status = newScene;
        UpdateGame();
    }

    /// <summary>
    /// Creates a dialogue option button and adds it to the dialogue buttons list.
    /// </summary>
    /// <param name="name">The name of the button as it will be displayed in the editor.</param>
    /// <param name="text">The text the button should display. Use "" to display default text.</param>
    /// <param name="btnPrefab">The dialogue option button prefab to use.</param>
    /// <returns>The created dialogue option button</returns>
    private Button CreateDialogueOption(string name, string text, Button btnPrefab)
    {
        Button b = Instantiate(btnPrefab, dialogueOptionsContainer.transform);
        b.name = $"{name} button";
        if (text != "") b.GetComponent<TMP_Text>().text = text;
        dialogueBtns.Add(b);
        return b;
    }
    #endregion
    #region Player actions methods
    /// <summary>
    /// Time advances.
    /// </summary>
    /// <param name="hours">The number of hours to advance time by.</param>
    /// <param name="playerAction">The action the player is doing whilst time passes.</param>
    private void PassTime(int hours, Action playerAction)
    {
        // Advance time 1 hour at a time
        for (int i = 0; i < hours; i++)
        {
            // Deplete/restore levels
            satiation = Mathf.Clamp(satiation + SatiationChange(playerAction) + (emptyLevels * LEVEL_EMPTY_PENALTY), LEVELS_MIN, LEVELS_MAX);
            energy = Mathf.Clamp(energy + EnergyChange(playerAction) + (emptyLevels * LEVEL_EMPTY_PENALTY), LEVELS_MIN, LEVELS_MAX);
            warmth = Mathf.Clamp(warmth + WarmthChange(playerAction) + (emptyLevels * LEVEL_EMPTY_PENALTY), LEVELS_MIN, LEVELS_MAX);
            /*
            // Burn fireplace fuel
            if (litFireplaces.Count > 0)
            {
                for (int j = litFireplaces.Count - 1; j >= 0; j--)
                {
                    if (!litFireplaces[j].BurnFireplaceFuel()) litFireplaces.RemoveAt(j);
                }
            }
            */
            emptyLevels = GetEmptyLevels();
            // Advance time by 1 hour
            timeHour = Helper.NormalizeLoopingInt(timeHour + 1, HOURS_IN_DAY);
            // Check if new day
            if (timeHour == DAYTIME_START_HOUR)
            {
                timeDay++;
                if (Helper.difficultyModifier > 100)
                {
                    temperature = Helper.random.Next(TEMPERATURE_DAYTIME_MIN + 5, TEMPERATURE_DAYTIME_MAX);
                }
                else if (Helper.difficultyModifier > 60)
                {
                    temperature = Helper.random.Next(TEMPERATURE_DAYTIME_MIN, TEMPERATURE_DAYTIME_MAX);
                }
                else { temperature = Helper.random.Next(TEMPERATURE_DAYTIME_MIN, TEMPERATURE_DAYTIME_MAX - 5); }
                if (Helper.difficultyModifier > DIFFICULTY_MODIFIER_MIN) Helper.difficultyModifier += DIFFICULTY_MODIFIER_DEPLETION;
            } // Check if nighttime
            else if (timeHour == NIGHTTIME_START_HOUR) temperature += TEMPERATURE_NIGHTTIME_DROP;
            // Check if game over
            if (emptyLevels >= 3)
            {
                status = SceneStatus.GameOver;
            }
        }
    }

    /// <summary>
    /// Player travels to a new location.
    /// </summary>
    /// <param name="destination">The location to travel to.</param>
    public void Travel(Area destination)
    {
        // Check if time needs to pass
        if (destination is District)
        {
            if (currentLocation is District)
            {
                // Travelling to a district from another district = 1 hour travel time
                PassTime(1, Action.Travelling);
            }
            else if (currentLocation is Road)
            {
                // Travelling to a district from a road = X hours of travel time, where X = road length
                Road r = (Road)currentLocation;
                PassTime(r.length, Action.Travelling);
            }
        }
        else if (destination is Road)
        {
            // Travelling to a road from a district = X hours of travel time, where X = road length
            if (currentLocation is District)
            {
                Road r = (Road)destination;
                PassTime(r.length, Action.Travelling);
            }
            else if (currentLocation is Road)
            {
                // Travelling to a road from another road - skip destination road and go straight to connected disrtict
                // X hours of travel time, where X = 'destination' road length
                Road r = (Road)destination;
                PassTime(r.length, Action.Travelling);
                foreach (Area a in r.GetConnections())
                {
                    if (a is District)
                    {
                        destination = a;
                        break;
                    }
                }
            }
        }
        currentLocation = destination;
        UpdateGame();
    }

    /// <summary>
    /// Player has confirmed they wish to perform action for specified hours.
    /// </summary>
    /// <param name="playerAction">The action the player wants to perform.</param>
    /// <param name="hours">The number of hours the player selected.</param>
    public void ConfirmTimeSelection(Action playerAction, int hours)
    {
        switch (playerAction)
        {
            case Action.Searching:
                SearchShelter(hours);
                break;
            case Action.Resting:
                Rest(hours);
                break;
            case Action.Sleeping:
                Sleep(hours);
                break;
        }
    }

    /// <summary>
    /// Player attempts to search shelter.
    /// </summary>
    public void AttemptSearch()
    {
        if (currentLocation is Shelter)
        {
            Shelter s = (Shelter)currentLocation;
            if (s.GetUnsearchedArea() > 1)
            {
                // Display search time selection popup
                timeSelectionScript.SelectTime(Action.Searching, s.GetUnsearchedArea());
            }
            else { SearchShelter(1); }
        }
    }

    /// <summary>
    /// Player attempts to rest in shelter.
    /// </summary>
    public void AttemptRest()
    {
        if (currentLocation is Shelter) timeSelectionScript.SelectTime(Action.Resting, 24);
    }

    public void AttemptSleep()
    {
        if ((currentLocation is ResidentialEstate) && (energy < ENERGY_SLEEP_MAX))
        {
            timeSelectionScript.SelectTime(Action.Sleeping, (LEVELS_MAX - energy) / ENERGY_RESTORATION_RATE);
        }
    }

    /// <summary>
    /// Players searches current Shelter for items and adds them to their inventory.
    /// </summary>
    /// <param name="searchTime">The amount of time in hours to search.</param>
    private void SearchShelter(int searchTime)
    {
        if (currentLocation is Shelter)
        {
            Shelter s = (Shelter)currentLocation;
            List<int> foundItems = s.Search(searchTime);
            foreach (int i in foundItems)
            {
                // Add found items to inventory
                if (WorldItems.itemData[i] is Food)
                {
                    inventory[i] += WorldItems.itemData[i].weight / WorldItems.FOOD_GRAMS_MULTIPLIER;
                }
                else if (WorldItems.itemData[i].name == "Matches")
                {
                    //inventory[i] += WorldItems.itemData[i].weight / WorldItems.MATCHES_GRAMS_MULTIPLIER;
                }
                else { inventory[i]++; }

            }
            PassTime(searchTime, Action.Searching);
            UpdateGame();
        }
    }

    /// <summary>
    /// Player rests in shelter.
    /// </summary>
    /// <param name="restTime">The amount of time in hours to rest for.</param>
    public void Rest(int restTime)
    {
        PassTime(restTime, Action.Resting);
        UpdateGame() ;
    }

    /// <summary>
    /// Player sleeps in shelter.
    /// </summary>
    /// <param name="sleepTime">The amount of time in hours to sleep for.</param>
    public void Sleep(int sleepTime)
    {
        PassTime(sleepTime, Action.Sleeping);
        UpdateGame();
    }

    /*
    /// <summary>
    /// Player attempts to light fireplace.
    /// </summary>
    public void LightFireplace()
    {
        if (currentLocation is ResidentialEstate)
        {
            ResidentialEstate re = (ResidentialEstate)currentLocation;
            inventory[WorldItems.GetItemIndex("Matches")]--;
            if (re.TryLightFireplace()) litFireplaces.Add(re);
            UpdateGame();
        }
    }
    */

    /// <summary>
    /// Player ingests 100g of Food item.
    /// </summary>
    /// <param name="foodIndex">The index of the Food item.</param>
    public void IngestFood(int foodIndex)
    {
        // Check player is not fully satiated
        if ((satiation < LEVELS_MAX) && (WorldItems.itemData[foodIndex] is Food))
        {
            Food f = (Food)WorldItems.itemData[foodIndex];
            satiation = Mathf.Clamp(satiation + f.satiation, LEVELS_MIN, LEVELS_MAX);
            inventory[foodIndex]--;
            UpdateGame();
        }
    }

    /// <summary>
    /// Player drops 1 of item.
    /// </summary>
    /// <param name="itemIndex">The index of the Item to drop.</param>
    public void DropItem(int itemIndex)
    {
        if (WorldItems.itemData[itemIndex] is Clothing)
        {
            // Check if item is equipped and if it is, unequip it if it is the last 1 of that item
            Clothing c = (Clothing)WorldItems.itemData[itemIndex];
            if ((equippedClothing[c.clothingType] == itemIndex) && (inventory[itemIndex] == 1)) equippedClothing[c.clothingType] = -1;
        }
        inventory[itemIndex]--;
        UpdateGame();
    }
    #endregion
    #region Clothing methods
    /// <summary>
    /// Check whether Clothing item is currently equipped.
    /// </summary>
    /// <param name="clothing">The item of Clothing to check.</param>
    /// <returns>True if the Clothing item is equipped.</returns>
    public bool IsClothingEquipped(Clothing clothing)
    {
        if (equippedClothing[clothing.clothingType] == WorldItems.GetItemIndex(clothing.name)) return true;
        return false;
    }

    /// <summary>
    /// (Un)Equips clothing item.
    /// </summary>
    /// <param name="clothingIndex">The item of clothing to (un)equip.</param>
    public void UnEquipClothing(int clothingIndex)
    {
        // Check if the clothing item is being equipped or unequipped
        Clothing c = (Clothing)WorldItems.itemData[clothingIndex];
        if (IsClothingEquipped(c))
        {
            // Unequip
            equippedClothing[c.clothingType] = -1;
        } // Equip
        else { equippedClothing[c.clothingType] = clothingIndex; }
        UpdateGame();
    }

    /// <summary>
    /// Gets total warmth value provided by equipped clothing.
    /// </summary>
    /// <returns>Total warmth value of clothes.</returns>
    public int GetTotalWarmth()
    {
        int totalWarmth = 0;
        foreach (int clothingIndex in equippedClothing.Values)
        {
            // Check that slot is equipped with a clothing item
            if (clothingIndex != -1)
            {
                // Add clothing item's warmth to total
                Clothing c = (Clothing)WorldItems.itemData[clothingIndex];
                totalWarmth += c.warmth;
            }
        }
        return totalWarmth;
    }
    #endregion
    #region Levels methods
    /// <summary>
    /// Returns how much satiation the player loses.
    /// </summary>
    /// <param name="playerAction">The action the player is currently doing.</param>
    /// <returns>The amount of satiation the player loses.</returns>
    private int SatiationChange(Action playerAction)
    {
        if ((playerAction == Action.Resting) || (playerAction == Action.Sleeping)) return SATIATION_DEPLETION_RATE / 2;
        return SATIATION_DEPLETION_RATE;
    }

    /// <summary>
    /// Returns how much energy the player loses/gains.
    /// </summary>
    /// <param name="playerAction">The action the player is currently doing.</param>
    /// <returns>The amount of energy the player loses/gains.</returns>
    private int EnergyChange(Action playerAction)
    {
        switch (playerAction)
        {
            case Action.Resting:
                // Player regains energy
                return ENERGY_RESTORATION_RATE / 2;
            case Action.Sleeping:
                // Player regains energy
                return ENERGY_RESTORATION_RATE;
            case Action.Travelling:
                // Player loses energy + weight
                return ENERGY_DEPLETION_RATE - (GetTotalWeight() / 1000);
            case Action.Searching:
                // Player loses energy
                return ENERGY_DEPLETION_RATE;
        }
        return 0;
    }

    /// <summary>
    /// Returns how much warmth the player loses/gains.
    /// </summary>
    /// <param name="playerAction">The action the player is currently doing.</param>
    /// <returns>The amount of warmth the player loses/gains.</returns>
    private int WarmthChange(Action playerAction)
    {
        /*
        // If next to lit fireplace, gain 10 warmth unless travelling
        if ((playerAction != Action.Travelling) && (currentLocation is ResidentialEstate))
        {
            ResidentialEstate r = (ResidentialEstate)currentLocation;
            if (r.fireplaceLit) return FIREPLACE_WARMTH;
        }
        */
        switch (playerAction)
        {
            case Action.Travelling:
                // Player may lose warmth
                if (temperature < TEMPERATURE_DAYTIME_MAX)
                {
                    int warmthLoss = GetTotalWarmth() - (TEMPERATURE_DAYTIME_MAX - temperature);
                    return Mathf.Clamp(warmthLoss, (TEMPERATURE_DAYTIME_MIN - 5) - TEMPERATURE_DAYTIME_MAX, 0);
                }
                break;
            case Action.Searching:
                // Player may gain warmth
                if (GetTotalWarmth() > WARMTH_INSIDE_GAIN_MIN)
                {
                    return Mathf.Clamp(GetTotalWarmth() - WARMTH_INSIDE_GAIN_MIN, 1, WARMTH_INSIDE_GAIN_MAX - WARMTH_INSIDE_GAIN_MIN);
                }
                break;
            case Action.Sleeping:
                // Player gains warmth
                return BED_WARMTH;
            case Action.Resting:
                // Player may gain warmth
                if ((inventory[WorldItems.GetItemIndex("Sleeping Bag")] > 0) || (currentLocation is ResidentialEstate)) return BED_WARMTH;
                if (GetTotalWarmth() > WARMTH_INSIDE_GAIN_MIN)
                {
                    return Mathf.Clamp(GetTotalWarmth() - WARMTH_INSIDE_GAIN_MIN, 1, WARMTH_INSIDE_GAIN_MAX - WARMTH_INSIDE_GAIN_MIN);
                }
                break;
        }
        return 0;
    }

    /// <summary>
    /// Checks whether the player is hungry or not.
    /// </summary>
    /// <returns>True if the player is hungry.</returns>
    public bool IsPlayerHungry()
    {
        if (satiation < LEVELS_MAX) return true;
        return false;
    }

    /// <summary>
    /// Gets number of levels that are empty.
    /// </summary>
    /// <returns>The number of empty levels.</returns>
    private int GetEmptyLevels()
    {
        int emptyLevels = 0;
        if (satiation <= LEVELS_MIN) emptyLevels++;
        if (energy <= LEVELS_MIN) emptyLevels++;
        if (warmth <= LEVELS_MIN) emptyLevels++;
        return emptyLevels;
    }
    #endregion
    #region Start game methods
    /// <summary>
    /// Creates inventory and populates it with starting items.
    /// Starting items include:
    /// - T-Shirt (equipped)
    /// - Shorts (equipped)
    /// - Trainers (equipped)
    /// - Matches (100g = 10)
    /// - Water (600g = 6)
    /// - Chocolate (300g = 3)
    /// - Fruit (100g = 1)
    /// </summary>
    /// <returns>The inventory the player starts with.</returns>
    private Dictionary<int, int> PopulateStartingInventory()
    {
        // Create empty inventory
        Dictionary<int, int> startingInventory = new Dictionary<int, int>();
        for (var i = 0; i < WorldItems.itemData.Count; i++) startingInventory.Add(i, 0);
        // Add starting items
        startingInventory[WorldItems.GetItemIndex("T-Shirt")] = 1;
        startingInventory[WorldItems.GetItemIndex("Shorts")] = 1;
        startingInventory[WorldItems.GetItemIndex("Trainers")] = 1;
        //startingInventory[WorldItems.GetItemIndex("Matches")] = 10;
        startingInventory[WorldItems.GetItemIndex("Water")] = 6;
        startingInventory[WorldItems.GetItemIndex("Chocolate")] = 3;
        startingInventory[WorldItems.GetItemIndex("Fruit")] = 1;
        return startingInventory;
    }

    /// <summary>
    /// Creates equipped clothing inventory and populates it with starting clothes that are already equipped.
    /// Starting clothes that are equipped:
    /// - T-Shirt
    /// - Shorts
    /// - Trainers
    /// A clothing slot with -1 means no Clothing is equipped to that slot.
    /// </summary>
    /// <returns></returns>
    private Dictionary<ClothingSlot, int> PopulateStartingEquipment()
    {
        Dictionary<ClothingSlot, int> startingEquipment = new Dictionary<ClothingSlot, int>();
        startingEquipment[ClothingSlot.Trousers] = WorldItems.GetItemIndex("Shorts");
        startingEquipment[ClothingSlot.Shirt] = WorldItems.GetItemIndex("T-Shirt");
        startingEquipment[ClothingSlot.Shoes] = WorldItems.GetItemIndex("Trainers");
        startingEquipment[ClothingSlot.Coat] = -1;
        startingEquipment[ClothingSlot.Hat] = -1;
        return startingEquipment;
    }
    #endregion
    #endregion
}
