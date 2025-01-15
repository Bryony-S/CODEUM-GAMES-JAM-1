using System.Collections;
using System.Collections.Generic;

public static class DialogueGenerator
{
    public static string InDistrict(District district)
    {
        string dialogue = $"You are in the {district.name}ern district of a {district.parent.name}. " +
            $"There is a {district.GetShelters()[0].name}";
        if (district.GetShelters().Count > 1)
        {
            dialogue += $" and a {district.GetShelters()[1].name}. ";
        }
        else { dialogue += ". "; }
        if (district.GetOtherDistricts().Count > 1)
        {
            dialogue += "There are roads leading to the other districts ";
        }
        else { dialogue += "There is a road leading to the other district "; }
        dialogue += $"or a road leading out of the {district.parent.name}.";
        return dialogue;
    }

    public static string InRoad(Road road)
    {
        string dialogue = $"At the end of the road is a {road.GetConnectingRoads().Count + 1}-way junction. " +
            $"You can continue down one of the other roads or go back the way you came. " +
            $"There is an Abandoned Car you can take shelter in.";        
        return dialogue;
    }

    public static string Overburdened()
    {
        return "<b>You are carrying too much to travel.</b>";
    }

    public static string GameEnd(int days)
    {
        return $"You finally succumbed to the bleakness of an unending winter.\nYou survived for {days} days.";
    }

    #region Shelter dialogue
    public static string InShelter(Shelter shelter)
    {
        string dialogue = $"You are in a {shelter.name}. ";
        // Display unsearched area
        if (shelter.GetUnsearchedArea() > 0)
        {
            if (shelter.GetUnsearchedArea() == shelter.searchSize)
            {
                dialogue += "You have not searched this area.";
            }
            else
            {
                float unsearchedPercentage = ((float)shelter.GetUnsearchedArea() / (float)shelter.searchSize) * 100f;
                dialogue += $"You have searched {(int)unsearchedPercentage}% of the area.";
            }
        }
        else { dialogue += "You have fully searched the area."; }
        // Unique dialogue
        if (shelter is VisitorCentre)
        {
            //dialogue += " You may find information on the local area here.";
        }
        else if (shelter is ResidentialEstate)
        {
            dialogue += " You should be able to find a bed to rest in here."; // and a fireplace to warm up with here.";
        }
        return dialogue;
    }

    public static string InAbandonedCar()
    {
        return "You are in an abandoned car.";
    }
    #endregion
    #region Opening dialogue
    public static string Opening()
    {
        return "You wake up in the dilapidated old house you broke into last night. " +
            "Light shines through the windows, the worn curtains doing nothing to block the morning sun. " +
            "If feels slightly colder this morning than it did yesterday, but you've noticed the temperature dropping for a while now. " +
            "You remember - from before everything went to shit - " +
            "that the world's scientists were saying that when the Earth started getting cold, " +
            "it would not be warm again for a very, very long time.\n" +
            "You climb out of bed and go outside. " +
            "You shouldn't stay here too long, you have to keep moving and scavenge what few resources are left in order to survive.";
    }

    public static string HowToPlay()
    {
        return "You must monitor three levels to survive:\n" +
            "\u2022 Satiation - how hungry you are\n" +
            "\u2022 Energy - how tired you are\n" +
            "\u2022 Warmth - how cold you are\n" +
            "You levels are displayed at the top of the screen. If all levels drop to 0, game over.\n" +
            "You must search the world for resources to help you stay alive. " +
            "Food satiates your hunger; clothing prevents you from losing warmth whilst outside; and resting in a shelter regains energy.\n" +
            "The temperature changes at the beginning of each day and it is always colder at night. " +
            "As days go by the game will become more difficult, " +
            "with the temperature being consistently lower and resources becoming more scarce.\n" +
            "You have a backpack to store items you find, but the more you carry the more energy you lose whilst travelling. " +
            "You can carry up to 12kg. You will start off with some items to help you begin your journey. Good luck!";
    }
    #endregion
}
